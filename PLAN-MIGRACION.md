# Plan de migración — Aries Contador

Documento de trabajo. Complementa [`ARQUITECTURA.md`](ARQUITECTURA.md) (mapa actual) y [`MODELOS-BD.md`](MODELOS-BD.md) (esquema MySQL y SPs).

**Objetivo principal:** la aplicación de escritorio (`CapaPresentacion`) debe quedar **correcta y estable**. API, Blazor y reorganizaciones de carpetas son secundarios hasta que el exe de producción no se rompa.

---

## 1. Reglas no negociables

Estas reglas mandan sobre cualquier “mejora” de 2.0, Clean Architecture o Blazor.

### 1.1 El modelo de base de datos se respeta

La fuente de verdad del esquema es el dump `aries` documentado en [`MODELOS-BD.md`](MODELOS-BD.md).

| Se hace | No se hace |
|---|---|
| Hablar con las tablas, columnas, enums y SPs **existentes** | Rediseñar tablas “porque en C# se vería mejor” |
| Completar SPs **faltantes** que el C# ya llama (`SP_UpdateCompany`, `SP_InsertUser`, `SP_UpdateUser`) | Sustituir SPs por SQL ad hoc o por otro motor |
| Conservar nombres de SP **incluyendo typos** (`SP_GetJournalEntyLineDeletedByDateRange`, etc.) | Renombrar SPs y romper Dapper |
| Conservar el cruce de `SP_InsertAccount` (`AccountType` ↔ `account_guide`, `AccountTag` ↔ `account_type`) | “Corregir” el cruce sin adaptar todo el C# |
| Soft delete (`active = 0`) y restore | `DELETE` físico de asientos/líneas |
| Unificar `company_id` varchar(4/5) **solo** con script de migración explícito y prueba en copia | Cambiar anchos a ciegas en RDS |

Excepciones permitidas (bugs reales del dump, no rediseño):

- Corregir `SP_InsertCompany`: `SET NewCompanyId = CompanyId` (variable inexistente); debe devolver el código.
- Añadir unique `(company_id, month_report)` en `accounting_months` **después** de verificar que no hay duplicados.
- Hashear `users.password` es un cambio de seguridad **posterior** a la estabilidad del escritorio; no entra en las primeras fases.

### 1.2 La lógica que se respeta es la de Windows Forms

Cuando 1.x (WinForms + `CapaLogica`) y 2.0 (API/Blazor) discrepan, **gana el escritorio**. El API 2.0 se adaptará al escritorio, no al revés.

La lógica viva está en:

| Módulo | Dónde está hoy | Qué hay que preservar |
|---|---|---|
| Plan de cuentas | `CuentaCL` + `CuentaDao` + forms `FrameMaestroCuenta` / `FrameNuevaCuenta` | No borrar cuentas de sistema ni no-auxiliares. Nombre único por compañía. Al crear auxiliar bajo auxiliar, **heredar saldos**. Ordenar árbol título → hijas (recursivo). Roll-up de débitos/créditos a padres. `VerificarSiEsApta` avisa si la auxiliar tiene movimientos. `QuitarCuentasSinSaldos` deja títulos aunque no tengan saldo. |
| Compañías | `CompañiaCL` + `CompañiaDao` + `FrameMaestroCompañia` | Validar cédula (`VerificaString.VerificarID`), nombre no vacío, email. Insert/update **local**. Copiar plan de cuentas desde `copiarDe` **completo**, no el filtro `Id <= 57` del API 2.0. |
| Usuarios | `UsuarioCL` + `UsuarioDao` | Username único, nombre no en blanco. |
| Periodos (maestro viejo / reportes) | `FechaTransaccionCL` | No duplicar mes. `FechaAbrirMes`: mes anterior al más viejo + mes siguiente al más nuevo (si no hay meses, mes actual). `FechaCerrarMes`: extremos abiertos. |
| Periodos (asientos) | `FinancialService.CreatePostingPeriod` | `PeriodExist` — misma regla de no duplicar. |
| Asientos / líneas / restore / cierre | `FinancialService` + forms `FrameAsientos`, `RestoreJournalEntry`, `FrameAsientoCierre` | Este camino **ya es** el del escritorio. No reactivar `AsientoCL`/`TransaccionCL` (están comentados). |
| Permisos | `PermisoCL` + `Guachi` + `FormPermisoUsuario` | Admin bypasea. `modules` / `windows` / `windows_permission` / `companies_permission`. El API 2.0 **no** tiene esto; no se pierde. |
| Correo | `CorreoCL` | Log `usuarios_correo`. Fuera de Core; se mantiene en legacy. |
| Reportes clásicos | `CapaEntidad.Reportes` + `CuentaCL.LLenarConSaldos` | Auxiliares, balance de situación, movimientos, maestro de cuentas. No sustituir por stubs de `AccountingReportService` 2.0. |
| Reportes nuevos | `FinancialReportService` + forms comprobación / P&G / asientos | Ya en el stack Dapper. Conservar. |
| Login | `HttpAdministrationService` + `LoginForm` | Contrato `POST auth/login`. En fases tempranas se puede **añadir** login in-process como fallback para no depender del API caído. |

**Anti-patrón a no importar desde 2.0**

`Aries.WebServices.AdministrationService.CreateCompany` copia solo cuentas con `Id <= 57`. Eso no es la regla del escritorio. Al unificar compañías, se porta `CompañiaDao.Insert(..., copiarDe)` (o su equivalente Dapper que copie **todo** el árbol de la compañía origen).

### 1.3 El escritorio primero

Orden de prioridad:

1. Compila, arranca, login, elegir compañía, asientos, cuentas, reportes, permisos — contra MySQL real (o copia).
2. Un solo `AriesContador.Core` y un solo `AriesContador.Data` sin conflictos de merge.
3. Un solo camino de datos por operación (dejar de mezclar HTTP + `*CL` en el mismo botón).
4. Tests de las reglas de `CapaLogica` que hoy no tienen cobertura.
5. Recién entonces: API **Minimal APIs** unificado, Blazor, mover carpetas, net8-windows.

No se migra WinForms a `net8.0-windows` en este plan. Hay un intento huérfano (`AriesContador.WindowsUI`, restos net8 sin `.csproj` útil). **El exe de producción sigue en .NET Framework 4.8.**

---

## 2. Estado actual (punto de partida)

Hay **dos productos** en `C:\Aries`:

| Producto | Solución | Estado respecto a este plan |
|---|---|---|
| Escritorio 1.x | `Aries Contador.sln` | **En el centro.** WinForms 4.8, dos stacks de datos, login HTTP. |
| Contabilidad 2.0 | `AriesWebApi/aries.contabilidad.2.0.sln` (git anidado) | **Parqueado.** Se usa como cantera de código (async, servicios de API), no como dueño de reglas. |

El escritorio habla tres caminos a MySQL a la vez (ADO.NET, Dapper, HTTP). Hay conflictos de merge sin resolver en `AriesContador.Data` y en `Aries.WebServices` de la raíz.

Detalle en [`ARQUITECTURA.md`](ARQUITECTURA.md) secciones 1–10.

---

## 3. Inventario de duplicados y plan de merge

Regla de merge: **una copia canónica**. La otra se borra o pasa a `ProjectReference` de la canónica. Si 2.0 tiene un método útil (p. ej. `AddAsync`, `GetCompanyConsecutive`), se **porta** a la canónica; no se dejan dos ensamblados con el mismo `AssemblyName`.

### 3.1 Matriz de decisión

| Pieza | Copia A (raíz) | Copia B | Canónica | Qué hacer con la otra |
|---|---|---|---|---|
| `AriesContador.Core` | `AriesContador.Core/` — la usa el escritorio | `AriesWebApi/src/Core/AriesContador.Core` | **Raíz** | Diff archivo a archivo. Portar a la raíz lo que 2.0 tenga de más (contratos async, etc.). Luego el `.csproj` 2.0 apunta a la raíz (o se elimina la carpeta 2.0). |
| `AriesContador.Data` | `AriesContador.Data/` — **conflicto de merge** en `CompanyRepository` | `AriesWebApi/src/Core/AriesContador.Data` | **Raíz**, tras resolver conflictos | Igual: un solo proyecto. MySql.Data: alinear versión (raíz 8.0.22 / 2.0 8.3.0) **después** de probar el escritorio. |
| Casos de uso in-process | `AriesContador.Services` (`FinancialService`, `FinancialReportService`, `HttpAdministrationService`) | — | **Raíz Services** | Es el cerebro del escritorio. Aquí se **inyectan** las reglas de `CapaLogica` cuando se apague un `*CL`. |
| Casos de uso API | `Aries.WebServices/` raíz — **rota** (`<<<<<<<`) | `AriesWebApi/src/Services/Aries.WebServices` — completa, sin conflicto | **2.0** para el host HTTP, más adelante | Borrar la copia de la raíz. No mezclar con `AriesContador.Services` hasta la fase de API. |
| API HTTP | `Aries.WebAPI/` y `SuperAPI/` — ya borrados del working tree | `AriesWebApi/src/Services/Aries.WebAPI` net8 **con Controllers** | **Reescribir como Minimal APIs** (fase 5) | Mismas rutas que hoy (`auth/login`, `company/*`, …). Sin `Controllers/`. El host solo mapea HTTP → `AriesContador.Services`. |
| UI escritorio | `CapaPresentacion` 4.8 — **producción** | `AriesContador.WindowsUI/` — restos net8 (bin/obj, sin proyecto usable) | **CapaPresentacion** | Borrar `AriesContador.WindowsUI`. |
| UI web | — | `Aries.Contabilidad` Blazor | Fuera de alcance inmediato | No se toca hasta que el escritorio esté estable. |
| Cliente HTTP | `HttpAdministrationService` en Services 1.x | `AriesWebApi.Client` net9 + HttpServices de Blazor | 1.x para el exe | No adoptar el Client net9 en WinForms 4.8. |
| Stub | `Aries.Core` (`"Hello word"`) | — | Ninguna | **Borrar.** No confundir con `AriesContador.Core`. |
| Tests vacíos | `TestingXUnix`, `Test/` (C++), `AriesContador.UnitTest/` (solo bin/obj) | Tests 2.0 (rutas de csproj dudosas) | `AriesContador.Tests` (ampliar) | Borrar muertos de la raíz. Arreglar tests 2.0 en fase API. |
| CapaDatos “port” | `CapaDatos` 4.8 — **en uso** | `AriesContador.CapaDatos/` — solo bin/obj | `CapaDatos` | Borrar la carpeta vacía. |
| Entidad / lógica | `CapaEntidad`, `CapaLogica` | No hay equivalente rico en 2.0 | Legacy, se encoge | No duplicar. Permisos y correo se quedan aquí hasta tener repos Dapper equivalentes. |

### 3.2 Cómo mergear Core y Data (procedimiento)

No es un git merge a ciegas (son dos repos). Es un **diff dirigido**:

1. Resolver conflictos en la raíz (`CompanyRepository`, `AdministrationService` de `Aries.WebServices` raíz). En `CompanyRepository.Add`:
   - Conservar el `Add` **síncrono** (lo usa el escritorio).
   - El `AddAsync` de 2.0 se puede portar **después**, sin el SQL concatenado `WHERE name = '{account.Name}'` (inyección). Usar el mismo flujo que `Add`: `SP_InsertAccount` + remap de `FatherAccount`.
2. `git diff --no-index AriesContador.Core AriesWebApi/src/Core/AriesContador.Core` (y lo mismo en Data).
3. Lista de archivos solo-en-2.0 → copiar a la raíz si el escritorio o el API futuro los necesitan.
4. Lista de archivos solo-en-raíz → se quedan; 2.0 los pierde.
5. Archivos en ambos con diff: **comportamiento del escritorio gana**. Extra async se añade como overload, no como reemplazo.
6. Cambiar `ProjectReference` en `AriesWebApi` para apuntar a `../../AriesContador.Core` (rutas relativas desde el git anidado: decidir en fase 5; mientras tanto las copias 2.0 pueden seguir, pero **el escritorio no las referencia**).
7. Compilar `Aries Contador.sln`. Correr el exe.

### 3.3 Proyectos a eliminar (fase 0)

Sin tocar el exe:

- `Aries.Core/`
- `Aries.WebServices/` (raíz, rota)
- `TestingXUnix/`
- `Test/` (C++)
- `AriesContador.WindowsUI/`
- `AriesContador.CapaDatos/` (restos)
- `AriesContador.UnitTest/` (restos)
- Confirmar que `Aries.WebAPI/` y `SuperAPI/` siguen fuera del árbol
- `apprunner.yaml` (referencia un ensamblado que no existe) — quitarlo de la solución o reescribirlo más adelante

No eliminar `AriesWebApi/` en esta fase. Es el otro git; se trata en la fase API.

### 3.4 Git

| Repo | Remote | Qué hacer ahora |
|---|---|---|
| Padre `C:\Aries` | `github.com/kenaguilar7/aries-desktop` | Aquí vive el plan y el escritorio. |
| Anidado `AriesWebApi/` | Azure DevOps / `kenaguilar7/ariesv2` | **No fusionar repos** hasta que el escritorio esté estable. Submódulo o merge de historiales es fase 5. |

---

## 4. Arquitectura recomendada

### 4.1 Principio

Un dominio, una persistencia, una capa de casos de uso. La UI de escritorio es un adaptador. El API (luego) es otro adaptador de los **mismos** casos de uso.

El escritorio **sigue hablando MySQL en proceso** para contabilidad. HTTP se reduce a lo que hoy ya es HTTP (login / listado de compañías) o se sustituye por in-process para quitar un punto de fallo. No se convierte el exe en un cliente delgado de Blazor.

```text
                    ┌─────────────────────────────────────┐
                    │  CapaPresentacion  (WinForms 4.8)   │
                    │  Forms + GlobalConfig + DI          │
                    └──────────────┬──────────────────────┘
                                   │ solo casos de uso
                                   ▼
                    ┌─────────────────────────────────────┐
                    │  AriesContador.Services             │
                    │  Financial / Admin / Report / Auth  │
                    │  + reglas portadas de CapaLogica    │
                    └──────────────┬──────────────────────┘
                                   │ IUnitOfWork + repos
                                   ▼
                    ┌─────────────────────────────────────┐
                    │  AriesContador.Data                 │
                    │  Dapper → stored procedures MySQL   │
                    └──────────────┬──────────────────────┘
                                   ▼
                         MySQL `aries` (RDS)
                    tablas + SPs + funciones + vistas
```

Legacy (`CapaLogica` / `CapaDatos` / `CapaEntidad`) no desaparece de golpe. Se **congela** y se va vaciando módulo a módulo cuando Services+Data cubren el mismo comportamiento, verificado con el form correspondiente.

```text
  CapaPresentacion ──┬── Services ── Data ── MySQL     ← camino objetivo
                     └── CapaLogica ── CapaDatos ── MySQL  ← camino a apagar
                            └── CapaEntidad (Excel, permisos, validaciones)
```

### 4.2 Capas y responsabilidades (objetivo)

| Capa | Proyecto | Puede referenciar | No puede |
|---|---|---|---|
| UI | `CapaPresentacion` | Services, Core, Entidad (Excel/permisos mientras existan) | `CapaDatos`, `AriesContador.Data` directo, `new UnitOfWork` en el form |
| Aplicación | `AriesContador.Services` | Core, Data | WinForms, `CapaDatos` |
| Dominio | `AriesContador.Core` | nada de infra | Dapper, MySQL, Forms |
| Persistencia nueva | `AriesContador.Data` | Core | Forms, `CapaLogica` |
| HTTP (adaptador) | `Aries.WebAPI` — **Minimal APIs** | Services, Core | Lógica de negocio, Dapper, `CapaLogica`, Controllers MVC |
| Legacy (temporal) | `CapaLogica`, `CapaDatos`, `CapaEntidad` | Core (ya lo hacen) | Crecer con features nuevas |

**DI en WinForms (objetivo de fase 2):** `Program.cs` registra `IConnectionString`, `IUnitOfWork`, `IFinancialService`, `IFinancialReportService`, `IAdministrationService`. Los forms reciben servicios; dejan de hacer `new UnitOfWork(GlobalConfig.ConnectionString)`.

### 4.3 Estructura de carpetas objetivo

No mover archivos en fase 0–2 (rompe csproj, ClickOnce, Squirrel). La estructura de abajo es el **norte** cuando el exe ya esté estable y se justifique un rename.

```text
Aries/                              # repo escritorio
├── Aries Contador.sln
├── docs/                           # ARQUITECTURA, MODELOS-BD, este plan
├── src/
│   ├── Desktop/
│   │   └── Aries.Desktop/          # hoy: CapaPresentacion (4.8)
│   ├── Domain/
│   │   └── AriesContador.Core/
│   ├── Application/
│   │   └── AriesContador.Services/
│   ├── Infrastructure/
│   │   ├── AriesContador.Data/
│   │   └── Legacy/                 # se borra cuando el último *CL muera
│   │       ├── CapaLogica/
│   │       ├── CapaDatos/
│   │       └── CapaEntidad/
│   └── Api/                        # fase 5 — Minimal APIs, no Controllers
│       └── Aries.WebAPI/
│           ├── Program.cs          # DI, JWT, CORS, Map*Endpoints()
│           ├── Endpoints/          # un archivo por recurso (MapGroup)
│           │   ├── AuthEndpoints.cs
│           │   ├── CompanyEndpoints.cs
│           │   ├── UserEndpoints.cs
│           │   ├── AccountEndpoints.cs
│           │   ├── PostingPeriodEndpoints.cs
│           │   └── JournalEntryEndpoints.cs
│           └── Infrastructure/     # JWT helper, ProblemDetails, UserId desde claims
└── tests/
    └── AriesContador.Tests/
```

Hasta entonces, **nombres actuales**. Lo que sí se hace ya: una solución limpia (`Aries Contador.sln`) sin proyectos muertos.

### 4.4 Módulos: camino actual → camino objetivo

| Módulo | Hoy | Objetivo (escritorio estable) |
|---|---|---|
| Login | HTTP API | HTTP **o** `IAdministrationService` in-process (mismo contrato `WebToken`/`User`). Fallback local si el API no responde. |
| Compañías listar/borrar/código | HTTP | Un solo servicio. Si se queda HTTP, el host es `Aries.WebAPI` 2.0 con las **reglas de CompañiaCL**. Si se vuelve in-process, `AdministrationService` + `CompanyRepository` + SPs completos. |
| Compañías insert/update | `CompañiaCL` directo a MySQL | Mismo servicio que el listado. Copiar plan completo. Validaciones de `VerificaString`. |
| Usuarios | `UsuarioCL` | `AdministrationService` + completar `SP_InsertUser` / `SP_UpdateUser`. |
| Permisos / correo | `PermisoCL`, `CorreoCL` | Se quedan en legacy hasta tener repos. No bloquear estabilidad. |
| Plan de cuentas | `CuentaCL` | Portar reglas a `FinancialService` (o `AccountService` in-process) + SPs account*. El form no cambia de comportamiento. |
| Periodos (UI asientos) | `FinancialService` | Se queda. Alinear `FechaTransaccionCL` (reportes viejos) al mismo servicio cuando se toquen esos reportes. |
| Asientos | `FinancialService` | Se queda. Completar `IUnitOfWork.Commit` si un asiento+líneas debe ser atómico (hoy cada SP abre conexión). |
| Reportes nuevos | `FinancialReportService` | Se queda. |
| Reportes viejos | `CapaEntidad.Reportes` | Se quedan hasta paridad. No apagar `LLenarConSaldos` antes de comparar Excel. |
| Actualización | Squirrel → S3 | No tocar en la migración de código. |

### 4.5 Qué se comparte con 2.0 (más adelante)

Cuando el escritorio esté estable:

```text
                    WinForms 4.8              Blazor WASM
                           \                    /
                            \                  /
                         AriesContador.Services
                                  |
                         AriesContador.Data  →  MySQL aries
                                  |
                         AriesContador.Core
```

El host `Aries.WebAPI` es **solo** Minimal APIs: `MapGroup` + handlers que llaman a `AriesContador.Services`. **No** reimplementa compañías con `Id <= 57`. No hay `Controllers/`, ni `AriesBaseController`, ni `AddControllers()`. Blazor deja de tener DTOs propios; consume Core o el Client alineado a net8 (hoy el Client es net9 y Blazor no lo usa).

Eso es fase 5. Si se hace antes, el escritorio hereda bugs del API.

### 4.6 API: Minimal APIs (decisión)

El API 2.0 hoy es MVC (`AddControllers` + 8 controllers + `AriesBaseController` con `UserId` hardcodeado a `1`). En la migración **no se evoluciona ese estilo**: se sustituye por ASP.NET Core Minimal APIs (net8).

**Por qué**

- Encaja con “el API es un adaptador”: un endpoint es 5–15 líneas que delegan al servicio.
- Evita una jerarquía de controllers y el `UserId = 1` escondido en una clase base.
- OpenAPI/Swagger sigue (endpoint explorer). JWT y CORS no cambian.

**Cómo se organiza**

```csharp
// Program.cs — idea, no código de producción
builder.Services.AddScoped<IAdministrationService, AdministrationService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();
// ... IUnitOfWork, IConnectionString, JWT, CORS

var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapCompanyEndpoints();
app.MapUserEndpoints();
app.MapAccountEndpoints();
app.MapPostingPeriodEndpoints();
app.MapJournalEntryEndpoints();
```

Cada `Map*Endpoints` vive en `Endpoints/*.cs`:

```csharp
public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
{
    var g = app.MapGroup("/company").RequireAuthorization();
    g.MapGet("/getAll", async (IAdministrationService svc) =>
        Results.Ok(await svc.GetAllCompanies()));
    g.MapGet("/BuildCode", async (IAdministrationService svc) =>
        Results.Ok(new { Code = await svc.GetCompanyConsecutive() }));
    g.MapDelete("/delete/{code}", async (string code, IAdministrationService svc) =>
    {
        await svc.DeleteCompany(new Company { Code = code });
        return Results.Ok();
    });
    return app;
}
```

**Contrato HTTP que el escritorio ya usa** (no romper casing ni paths):

| Método | Ruta actual (Controllers) | Minimal API |
|---|---|---|
| POST | `/Auth/login` | `MapGroup("/auth").MapPost("/login", …)` `[AllowAnonymous]` |
| GET | `/company/getAll` | igual |
| GET | `/company/BuildCode` | igual |
| DELETE | `/company/delete/{code}` | igual |
| POST | `/Company/Create` | igual (cuando el maestro deje de ir por `CompañiaCL`) |
| GET/POST/… | Account, PostingPeriod, JournalEntry, JournalEntryLine, User | mismos paths que los controllers 2.0 |

ASP.NET Core es case-insensitive en rutas por defecto; el cliente WinForms (`auth/login`, `company/getAll`) sigue válido.

**Reglas del host**

| Se hace | No se hace |
|---|---|
| Handlers delgados: bind → llamar servicio → `Results.Ok/Unauthorized/Problem` | Validaciones de cédula, árbol de cuentas, copia de plan, etc. en el endpoint |
| `UserId` desde `ClaimTypes.NameIdentifier` / claim `"UserId"` (helper, no clase base) | `return 1;` como hoy en `AriesBaseController` |
| `Results.Problem` / `TypedResults` para errores | `Console.WriteLine` + 500 con stack al cliente |
| Mismos JSON que hoy (incl. `JsonStringEnumConverter` si el cliente lo espera) | Cambiar nombres de propiedades que ya consume WinForms (`WebToken`, `User`, `Code`) |
| Un `AriesContador.Services` compartido con el exe | Un segundo `Aries.WebServices` con reglas distintas (`Id <= 57`) |

`Aries.WebServices` 2.0 **no** es el destino. Sus métodos útiles se portan a `AriesContador.Services`; el proyecto API no referencia una capa de “web services” paralela.

Login (`POST /auth/login`) se queda anónimo. El resto de grupos usa `.RequireAuthorization()` (hoy `CompanyController` tiene `[Authorize]` comentado: al pasar a Minimal APIs se activa de forma explícita, salvo que el exe aún mande Bearer vacío en la primera petición — entonces login primero, y compañías con token).

---

## 5. Fases

Cada fase termina con un **criterio de salida** medible. No se empieza la siguiente si el criterio falla.

### Fase -1 — Caracterizar el escritorio (tests + contrato de UI)

**Qué**

1. Documentar las 23 forms en [`COMPORTAMIENTO-UI.md`](COMPORTAMIENTO-UI.md).
2. Tests sin UI de la lógica que usa cada form, con foco en asientos y reportes (`AriesContador.Tests`, `Aries.Desktop.Tests`).
3. Extraer fórmulas atrapadas en forms (`JournalEntryLineAmount`, roll-up de `CuentaCL`) para poder fijarlas.

**Criterio de salida:** `dotnet test` de ambos proyectos verde; P0 asientos + reportes cubiertos.

**No hacer:** automatizar clics WinForms; merge Core/Data.

### Fase 0 — Estabilizar el árbol (escritorio compilable y limpio)

**Qué**

1. Resolver conflictos de merge en `AriesContador.Data/Repositories/CompanyRepository.cs` (ganar el `Add` síncrono del escritorio).
2. No usar `Aries.WebServices` de la raíz; borrarlo.
3. Borrar proyectos muertos (sección 3.3).
4. Quitar de `Aries Contador.sln` cualquier referencia a muertos.
5. Confirmar que `CapaPresentacion` compila en Debug|AnyCPU y arranca contra la config actual.

**Criterio de salida:** `msbuild "Aries Contador.sln"` OK; exe abre; no quedan `<<<<<<<` en código de la solución.

**No hacer:** cambiar SPs, mover carpetas, apuntar `HttpBaseUrl` a otro host.

### Fase 1 — Un Core, un Data

**Qué**

1. Diff Core raíz vs Core 2.0; fusionar en la raíz (sección 3.2).
2. Diff Data raíz vs Data 2.0; fusionar en la raíz.
3. El escritorio sigue referenciando solo las copias de la raíz.
4. Completar en MySQL (en **copia** de BD, no a ciegas en RDS) los SPs que Data ya llama y no están en el dump: `SP_UpdateCompany`, `SP_InsertUser`, `SP_UpdateUser`. Contrato de parámetros alineado a los modelos Core / a lo que Dapper envía hoy.
5. Documentar en `MODELOS-BD.md` los SPs nuevos.

**Criterio de salida:** un solo comportamiento de `Company`/`Account`/repos en el exe; insert/update de compañía y usuario por Data no revienta por SP faltante.

### Fase 2 — Un camino por operación en el escritorio

**Qué**

1. Registrar servicios en `Program.cs` (sección 4.2). Forms de asientos/periodos/reportes nuevos dejan de instanciar `UnitOfWork`.
2. Compañías: dejar de mezclar HTTP + `CompañiaCL` en el mismo form. Elegir **un** backend:
   - **Recomendado para estabilidad:** todo in-process (`AdministrationService` + Data), login también in-process (consulta `users`, emite el mismo `WebToken` local o se deja de exigir Bearer).
   - Alternativa: todo HTTP contra `Aries.WebAPI`, pero **solo** cuando ese API ejecute las reglas de `CompañiaCL` (copia completa de cuentas, validaciones).
3. Usuarios: `UsuarioCL` → `AdministrationService` cuando los SP existan y las validaciones estén en el servicio (username único, nombre no vacío).
4. No mover aún cuentas ni permisos.

**Criterio de salida:** maestro de compañías y de usuarios no llaman a dos stacks. Login funciona sin Elastic Beanstalk o con fallback local documentado.

### Fase 3 — Portar reglas de `CapaLogica` que el escritorio aún necesita

Orden (de más riesgo contable a menos):

1. **Cuentas** — portar `CuentaCL` a `FinancialService` / servicio de cuentas:
   - delete / insert con herencia de saldo / unique name / `Ordernar` / `LLenarConSaldos` + roll-up / `VerificarSiEsApta` / `QuitarCuentasSinSaldos`
   - Tests unitarios de estas reglas **antes** de cambiar el form.
   - `FrameMaestroCuenta` y `FrameNuevaCuenta` pasan al servicio. Comparar árbol y saldos con la versión anterior (misma compañía de prueba).
2. **Periodos** — `FechaTransaccionCL.FechaAbrirMes` / `FechaCerrarMes` / no duplicar → `FinancialService` (ya tiene parte). Unificar las pantallas y los reportes viejos que aún usan `FechaTransaccion`.
3. Permisos y correo: **no portar** en esta fase salvo que bloqueen el exe.

**Criterio de salida:** crear/editar/borrar cuenta y abrir mes se comportan igual que hoy (checklist sección 6). `CuentaCL` y `FechaTransaccionCL` ya no los usa la UI, o están marcados obsoletos.

### Fase 4 — Reportes y unidad de trabajo real

**Qué**

1. Paridad de reportes: para cada Excel de `CapaEntidad.Reportes`, o se deja el generador viejo **sin romper**, o `FinancialReportService` produce el mismo archivo (columnas, redondeo, path de cuenta con `F_GetAccountPathForReport`).
2. Implementar transacción real en Data para: crear compañía + cuentas; crear asiento + líneas. Hoy `Commit()` lanza `NotImplementedException`.
3. Conservar nombres de SP; envolver en `StartTransaction` / `CommitTransaction` que ya existen en `MySqlDataAccess`.

**Criterio de salida:** asiento con N líneas no deja encabezado huérfano si falla una línea. Reportes de comprobación / P&G / asientos siguen iguales. Reportes clásicos no regresionan.

### Fase 5 — Minimal APIs y limpieza (después del escritorio estable)

**Qué** (solo entonces)

1. Nuevo host `Aries.WebAPI` en **Minimal APIs** (sección 4.6). No se “convierte” controller a controller: se reemplaza `MapControllers()` por `Map*Endpoints()`.
2. Borrar `Controllers/` (`Auth`, `Company`, `User`, `Account`, `PostingPeriod`, `JournalEntry`, `JournalEntryLine`, `AriesBaseController`).
3. El host referencia **un** `AriesContador.Core` + `AriesContador.Data` + `AriesContador.Services` (los canónicos de la raíz). `Aries.WebServices` 2.0 se vacía: lo útil se porta a Services; el resto se elimina.
4. `CreateCompany` en el servicio: copiar plan **completo**, no `Id <= 57`.
5. `UserId` desde JWT claims. Login deja de hacer `GetAllUsers()` + compare en texto plano (hash en un paso de seguridad posterior).
6. Contrato binario HTTP: Postman + el `HttpAdministrationService` del exe contra el host nuevo (`HttpBaseUrl` de prueba) **antes** de tocar producción.
7. Blazor: o usa el Client contra estas mismas rutas, o se aparca. No es el objetivo de este plan.
8. Decisión de git anidado (submódulo vs un solo repo).
9. Mover carpetas a la estructura 4.3 si el costo de csproj/Squirrel es aceptable.
10. Evaluar `net8.0-windows` **como proyecto nuevo** paralelo, no como rewrite in-place.

**Criterio de salida:** escritorio intacto; mismas rutas `/Auth/login` y `/company/*` responden igual; cero controllers; cero reglas distintas a WinForms.

---

## 6. Checklist de estabilidad del escritorio

Usar la misma compañía de prueba (copia de RDS, no producción) en cada fase.

### Arranque

- [ ] Exe arranca; Squirrel no tumba el proceso si S3 no responde (hoy un fallo en `UpdateApp` puede afectar el ctor de `GlobalConfig` — endurecer en fase 0/2).
- [ ] Login con usuario Administrador y con Usuario.
- [ ] Sin credenciales válidas, la app cierra (comportamiento actual de `FrameMenu`).

### Compañías

- [ ] Listar solo activas; Usuario ve solo las de `companies_permission`.
- [ ] Crear jurídica y física: validación de cédula y email igual que `CompañiaCL`.
- [ ] Crear copiando de otra: **mismo número de cuentas** que el origen (no 57).
- [ ] Editar y desactivar.
- [ ] Cambiar de compañía cierra forms que implementan `INeedValidatedForClose`.

### Cuentas

- [ ] Árbol ordenado título → mayor → auxiliar.
- [ ] No se puede borrar cuenta de sistema ni no-auxiliar.
- [ ] Nombre duplicado rechazado.
- [ ] Auxiliar bajo auxiliar hereda saldos y avisa si hay movimientos.
- [ ] Reportes de auxiliares roll-up a padres.

### Periodos y asientos

- [ ] No se puede crear un mes duplicado.
- [ ] Consecutivo de asiento por periodo (`SP_GetJournalEntryConsecutive`).
- [ ] Asiento cuadrado; líneas débito/crédito, colones/dólares, tipo de cambio.
- [ ] Soft delete y restore de asiento y de línea.
- [ ] Cierre de periodo genera el asiento de cierre esperado.

### Reportes

- [ ] Comprobación, estado de resultados, asientos (stack nuevo) abren Excel.
- [ ] Auxiliares, balance de situación, movimientos (stack viejo) abren Excel.
- [ ] Comparar un mes cerrado conocido: totales iguales a un Excel generado **antes** del cambio.

### Permisos

- [ ] Administrador entra a todos los módulos.
- [ ] Usuario restringido no ve compañías ni ventanas no asignadas (`Guachi`).

---

## 7. Tests mínimos (escritorio)

Hoy `AriesContador.Tests` casi no cubre nada. Antes de portar `CuentaCL` (fase 3), añadir tests **sin UI**:

| Test | Origen de la regla |
|---|---|
| No duplicar `PostingPeriod` en la misma compañía | `FinancialService` + `FechaTransaccionCL` |
| `Ordernar` produce preorden del árbol | `CuentaCL.Ordernar` |
| Roll-up: auxiliar con débito incrementa padre y título | `CuentaCL.LLenarConSaldos` |
| `QuitarCuentasSinSaldos` conserva títulos | `CuentaCL` |
| Delete rechaza no-auxiliar y `Editable == false` | `CuentaCL.Deleted` |
| Username duplicado rechazado | `UsuarioCL` |
| Cédula inválida rechazada | `CompañiaCL` / `VerificaString` |
| Consecutivo de código `C001` → `C002` | `CompañiaDao` / `GetCompanyConsecutive` — **sin** el bug de substring si hay `C1000` (varchar 5) |

Los tests de integración contra MySQL van a una base local (Docker 3307 o instancia de prueba), nunca a RDS de producción.

---

## 8. Qué no entra en este plan

- Reescribir WinForms en Blazor o WPF.
- Cambiar MySQL por SQL Server / PostgreSQL.
- Renombrar columnas ni SPs “para que se vean en inglés”.
- Unificar git de `AriesWebApi` en las fases 0–4.
- Dejar los Controllers MVC “un tiempo” junto a Minimal APIs (no hay dual stack HTTP).
- Hash de passwords y JWT real como **primer** entregable (sí en fase 5/seguridad, después de estable).
- Tablas vacías `actividades` / `tareas`.
- Proyecto `Aries.Core` stub, SuperAPI, App Runner fantasma.

---

## 9. Riesgos y mitigación

| Riesgo | Impacto | Mitigación |
|---|---|---|
| Merge 2.0 pisa reglas de cuentas/compañías | Datos contables mal copiados | WinForms gana siempre; test de conteo de cuentas al copiar compañía |
| Quitar HTTP de login sin fallback | Nadie entra | Fase 2: in-process o Minimal API verificado **antes** de cambiar `HttpBaseUrl` de producción |
| Minimal APIs cambian path/JSON | Login y compañías del exe fallan | Conservar las 4 rutas del cliente 1.x; prueba con `HttpAdministrationService` antes de producción |
| Completar SPs mal parametrizados | Inserts silenciosos / Dapper no mapea | Probar SPs con los mismos objetos que envía el repo; no inventar nombres de parámetro |
| `IUnitOfWork.Commit` de verdad cambia el lifetime de conexión | Deadlocks / conexiones abiertas | Un solo flujo (asiento+líneas) primero; el resto sigue conexión-por-SP |
| Mover carpetas / net8-windows | Squirrel, ClickOnce, csproj 4.8 | Fuera de fases 0–4 |
| Trabajar contra RDS producción | Corrupción | Copia de dump; Docker `AriesContabilidad_Local` |
| Conflictos `<<<<<<<` que se resuelven “tomando 2.0” | Escritorio deja de insertar compañías | En `CompanyRepository`, el `Add` síncrono es el default |

---

## 10. Orden de ejecución resumido

```text
-1 Caracterizar UI (docs + tests asientos/reportes) → no romper a ciegas
0  Limpiar conflictos y proyectos muertos     → exe verde
1  Un Core + un Data + SPs faltantes          → Data no miente
2  DI + un camino compañías/usuarios/login    → menos puntos de fallo
3  Portar CuentaCL / periodos con tests       → misma contabilidad, un stack
4  Reportes + transacción real                → no se rompe Excel ni asientos
5  Minimal APIs sobre los mismos servicios    → HTTP delgado, mismas rutas, sin Controllers
```

El trabajo “terminó” para el objetivo de este documento cuando las fases **0–4** cumplen sus criterios y el checklist de la sección 6 está en verde. La fase 5 es el host HTTP (Minimal APIs), no un requisito de estabilidad del escritorio.
