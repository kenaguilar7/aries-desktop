# Plan de mejoras — Aries Contador (fases 6+)

Documento de trabajo **después** de [`PLAN-MIGRACION.md`](PLAN-MIGRACION.md). Complementa [`ARQUITECTURA.md`](ARQUITECTURA.md) y [`MODELOS-BD.md`](MODELOS-BD.md).

**Punto de partida:** fases **0–5 cerradas**. El exe WinForms 4.8 habla MySQL in-process (`AriesContador.Services` → `AriesContador.Data` → SPs). Login, compañías, usuarios, cuentas, periodos y asientos ya no mezclan HTTP + `*CL` en el mismo botón. Hay un host `Aries.WebAPI` en Minimal APIs con JWT y las mismas rutas que el exe ya conocía.

**Objetivo de este plan:** endurecer lo que ya funciona (secretos, hash, integridad) y apagar el resto de legacy **sin cambiar el comportamiento contable**. Blazor, `net8.0-windows` y mover carpetas a `src/` siguen fuera hasta que estas fases cierren.

---

## 1. Qué ya no se discute

Siguen las reglas del plan de migración:

| Se hace | No se hace |
|---|---|
| Hablar con las tablas, columnas, enums y SPs **existentes** | Rediseñar el esquema “porque en C# se vería mejor” |
| Conservar nombres de SP **incluyendo typos** | Renombrar SPs y romper Dapper |
| Soft delete (`active = 0`) y restore | `DELETE` físico de asientos/líneas |
| Cuando 1.x y 2.0 discrepan, **gana el escritorio** | Importar `Id <= 57` u otras reglas del API viejo |
| Scripts de migración en **copia** / Docker (`aries_mysql_local :3307`) | ALTER a ciegas en RDS producción |
| Exe de producción en **.NET Framework 4.8** | Rewrite in-place a `net8.0-windows` |

Nuevas reglas de este plan:

1. **Un solo camino de autenticación.** `AdministrationService.Login` es la comparación de credenciales (escritorio y API). El API solo añade JWT encima.
2. **Secretos fuera de git.** Connection strings, JWT y passwords de RDS no viven en archivos versionados.
3. **CapaLogica no crece.** Toda feature nueva va a Services + Data. Legacy solo se toca para apagarlo.
4. **No se despliega el API nuevo a producción** hasta que fases 6 y 7 estén verdes (secretos rotados + hash). El exe de producción hoy **no necesita** ese API para login/maestros/asientos.

---

## 2. Estado actual (post fase 5)

```text
  CapaPresentacion ──┬── Services ── Data ── MySQL     ← camino vivo (login, maestros, cuentas, asientos, reportes nuevos)
                     └── CapaLogica ── CapaDatos       ← camino residual (permisos, correo, cierre viejo, reportes clásicos)
```

| Pieza | Estado |
|---|---|
| Login escritorio | `IAdministrationService.Login` in-process. PBKDF2 + rehash perezoso. Token local `"local"`. |
| Login API | Mismo servicio + JWT real. Rutas `/auth/login`, `/company/*` cubiertas por `Aries.WebAPI.Tests`. |
| Compañías / usuarios | Services + `SP_Insert*` / `SP_Update*` (fase 1). Copia de plan **completa**. |
| Cuentas / periodos / asientos | `FinancialService`. Asiento + líneas en transacción MySQL (`JournalEntryRepository.Add`). |
| Reportes nuevos | `FinancialReportService` (comprobación, P&G, asientos). |
| Reportes clásicos | Siguen `CuentaCL.LLenarConSaldos` + `CapaEntidad.Reportes`. |
| Permisos | `IPermissionService` + Dapper. `Guachi` sigue siendo helper de UI. |
| Correo | `IEmailService` + `usuarios_correo`. SMTP por appSettings/env. |
| Cierre de periodo | `FrameAsientoCierre` y `FrameAdministrarMeses` usan `ClosePostingPeriod`. |
| DI WinForms | `IConnectionString` singleton; UoW y servicios **transient**. Transacción en el repo, no `Commit()`. |
| `HttpAdministrationService` / `HttpFinancialService` | Borrados. |
| `AriesWebApi/` (git anidado) | Sigue parqueado. El host canónico es `Aries.WebAPI/` en la raíz. |
| CI | [`.github/workflows/ci.yml`](../.github/workflows/ci.yml) (workflow **CI**): Windows (MSBuild Release + Core/Desktop/WebAPI + `Verify-DesktopPublish`) y Ubuntu+MySQL (`Aries.Data.Tests`). Pack Squirrel de prueba: [`cd-test.yml`](../.github/workflows/cd-test.yml) (canal `updates-test`, no producción). |
| Config producción | `App.Production.config` y `appsettings.Production.json` tienen **connection string de RDS** y JWT placeholder. `HttpBaseUrl` apunta al Elastic Beanstalk viejo. |

---

## 3. Inventario de deuda (lo que este plan ataca)

### 3.1 Seguridad (prioridad 0)

| Hallazgo | Dónde | Riesgo |
|---|---|---|
| Password de usuarios en texto plano, columna `varchar(50)` | `users.password`, `AdministrationService.Login` | Filtración de BD = todas las cuentas |
| Connection string de RDS versionada | `CapaPresentacion/app.config`, `App.Production.config`, `Aries.WebAPI/appsettings.Production.json` | Credenciales en git (y en historial) |
| JWT de producción es placeholder `change-me` | `appsettings.Production.json` | Token falsificable si el API se publica así |
| `HttpBaseUrl` de Release apunta al EBS viejo | `App.Production.config` | El exe ya no lo usa para login, pero confunde el release |
| Swagger en la raíz del API si el ambiente no es Testing | `Aries.WebAPI/Program.cs` | Superficie extra en un deploy descuidado |
| CORS con IP fija de un host | `Program.cs` | Orígenes de Blazor parqueado; revisar antes de prod |

`.env` y `appsettings.Local.json` **sí** están en `.gitignore`. El agujero es lo que quedó **commiteado** en configs de Production/Debug.

Rotar el password de RDS es parte de la fase 6, no un “después”. El secreto ya salió del repo: cambiarlo en AWS y en las máquinas, no solo borrarlo del working tree.

### 3.2 Legacy todavía en la UI

`new *CL()` que el usuario todavía puede disparar:

| Superficie | Clase | Sustituto ya existente o a crear |
|---|---|---|
| `GlobalConfig.User` setter | `PermisoCL.GetAllModules` | `IPermissionService` (fase 8) |
| `FormPermisoUsuario` | `UsuarioCL`, `CompañiaCL`, `PermisoCL` | Administration + Permission |
| `FrameAsientoCierre` | `FechaTransaccionCL` | `IFinancialService` (periodos + `ClosePostingPeriod`) |
| `ReporteCuenta`, `ReporteBalanceSituacion`, `ReporteMovimientosCuenta`, `FrameReporteAuxiliares` | `CuentaCL` | `IFinancialService.GetAccounts` + `FillAccountsWithBalances` |
| `Correo.cs` | `CorreoCL` | `IEmailService` o se deja legacy explícito |
| `MaestroCuentas/LGCuenta` + `MaestroCuenta` | `CuentaCL` | **Muerto:** el menú abre `FrameMaestroCuenta`. Borrar. |

`AsientoCL` / `TransaccionCL` siguen en el proyecto CapaLogica pero no los usa la UI viva.

### 3.3 Doble modelo

`GlobalConfig` traduce `User` (Core) → `Usuario` (CapaEntidad) en cada login para permisos y reportes Excel. Mientras exista ese puente, CapaLogica no puede morir.

| Core | CapaEntidad | Quién manda hoy |
|---|---|---|
| `User` | `Usuario` | Core en login; Entidad en permisos/Excel |
| `Account` | `Cuenta` | Core en maestro nuevo; Entidad en reportes clásicos |
| `PostingPeriod` | `FechaTransaccion` | Core en asientos; Entidad en `FrameAsientoCierre` y reportes viejos |
| `Company` | (casi alineado) | Core |

### 3.4 Data / DI

- `IUnitOfWork` y servicios registrados como **Singleton** en WinForms: una sola instancia para toda la vida del proceso. Hoy cada repo abre su propia conexión, así que no explota, pero impide un UoW real y es un pie para race conditions si algún día se comparte transacción.
- `Commit()` no hace nada; las transacciones viven en `JournalEntryRepository` / `CompanyRepository`. Documentar o implementar de verdad, no dejar las dos historias.
- `NotImplementedException` en repos async: **cerrado.** Contratos `IRepository` / servicios son async-only (`*Async` + `CancellationToken`).
- `AdministrationService` ya no usa `GetAwaiter().GetResult()`. El escritorio hace `await` en los event handlers.
- Transacciones MySQL (`JournalEntryRepository.AddAsync`, alta de compañía, cierre de periodo) usan `OpenAsync` / `BeginTransactionAsync`.
- Clientes HTTP (`HttpAdministrationService`, `HttpFinancialService`, `HttpClientService`) no están en el DI del exe.

### 3.5 MySQL (bugs reales del dump, no rediseño)

Los mismos pendientes de [`MODELOS-BD.md`](MODELOS-BD.md) §8, ahora sí en alcance:

1. Unique `(company_id, month_report)` en `accounting_months` **después** de verificar duplicados.
2. Unificar ancho de `company_id` varchar(4) vs (5) con script explícito.
3. FK reales en `companies_permission` y `windows_permission`.
4. `companies` permite `user` nulo (TODO en `Program.cs`).
5. `users.updated_by` sin FK; a veces guarda cédula, no `user_id`.
6. Ampliar `users.password` **antes** del hash (fase 7). `varchar(50)` no cabe un PBKDF2/bcrypt.

### 3.6 Tests y operación

- `AriesContador.Tests` está en **net8.0**; CI usa [`global.json`](../global.json) (`8.0.x`). `Aries.WebAPI.Tests` corre en Windows y Ubuntu. `Aries.Data.Tests` solo en Ubuntu+MySQL (en Windows sin MySQL era un verde falso).
- No hay test de hash, permisos, ni cierre de periodo contra el servicio nuevo.
- No hay runbook de backup RDS / restore a Docker.
- Squirrel/S3 no se tocó (correcto en 0–5); sigue siendo el canal de update del exe.

---

## 4. Norte de arquitectura (sin mover carpetas)

Sigue valiendo el diagrama de [`PLAN-MIGRACION.md`](PLAN-MIGRACION.md) §4. Lo que cambia en 6+:

```text
                    ┌─────────────────────────────────────┐
                    │  CapaPresentacion  (WinForms 4.8)   │
                    │  Forms + GlobalConfig + DI scoped   │
                    └──────────────┬──────────────────────┘
                                   │ solo casos de uso
                                   ▼
                    ┌─────────────────────────────────────┐
                    │  AriesContador.Services             │
                    │  Financial / Admin / Report         │
                    │  Permission / Auth (hash) / Email   │
                    └──────────────┬──────────────────────┘
                                   │ IUnitOfWork + repos
                                   ▼
                    ┌─────────────────────────────────────┐
                    │  AriesContador.Data                 │
                    │  Dapper → stored procedures MySQL   │
                    └─────────────────────────────────────┘

  Aries.WebAPI (adaptador) ──► los mismos servicios
  CapaEntidad ──► solo generadores Excel (sin *CL)
  CapaLogica / CapaDatos ──► se borran cuando el último form deja de referenciarlos
```

El API **no** se convierte en requisito del exe. El escritorio sigue in-process. El API existe para (a) contrato HTTP ya testeado, (b) un futuro Blazor o cliente delgado, (c) reemplazar el EBS viejo cuando se decida apagarlo.

---

## 5. Fases

Cada fase termina con un **criterio de salida** medible. No se empieza la siguiente si el criterio falla, salvo 12 (tests/CI) que puede avanzar en paralelo desde el día 1.

### Fase 6 — Secretos fuera del código y rotación

**Qué**

1. Quitar connection strings y JWT reales de archivos versionados (`app.config` de Debug no debe apuntar a RDS; Production no debe llevar password). Patrón ya empezado: `App.Local.config` / `appsettings.Local.json` gitignored + `*.example`.
2. Release: transform o archivo de máquina (`App.Production.local.config` no versionado) o variables de entorno leídas al arrancar. El binario publicado **no** lleva el password en texto si se puede evitar; si ClickOnce/Squirrel exige un `.config` al lado del exe, ese archivo se genera en el pipeline / en el servidor de update, no se commitea.
3. `Aries.WebAPI`: `appsettings.Production.json` sin secretos; `Jwt:Key` y `ConnectionStrings:MySQLDefault` solo por env / user-secrets / docker secrets.
4. **Rotar** el password de RDS y el JWT de cualquier ambiente que haya usado el placeholder. El valor viejo está en el historial de git: borrarlo del working tree no basta.
5. `HttpBaseUrl` de Release: o se quita, o se documenta como “solo diagnóstico” (`FrameMenu` lo muestra). No apuntar al EBS viejo.
6. Swagger: solo Development/Local. Producción: `/health` sí, UI de Swagger no.
7. Confirmar que `SquirrelTemp/` sigue fuera de git (ya está en `.gitignore`).

**Criterio de salida:** `git grep` del repo de trabajo no encuentra el host RDS + password. Un clone limpio arranca en Local con `App.Local.config.example` + Docker. RDS responde con la **nueva** credencial. El exe Release no usa el EBS viejo.

**No hacer:** hash de passwords (fase 7); desplegar el API nuevo a AWS.

### Fase 7 — Hash de passwords

**Qué**

1. Ampliar `users.password` (p. ej. `varchar(255)` o `text`) en **copia** Docker, luego RDS con ventana.
2. Algoritmo único, portable a net48 y netstandard2.0: **PBKDF2-HMAC-SHA256** (iteraciones altas, salt por usuario, formato propio tipo `pbkdf2$iter$salt$hash`). No bcrypt si implica native interop distinto en 4.8 vs net8.
3. `AdministrationService.Login`:
   - Si el valor parece hash → verificar.
   - Si es texto plano y coincide → **rehash y UPDATE** en el mismo login (migración perezosa).
   - Si no coincide → mismo resultado vacío de hoy (no enumerar usuarios).
4. `CreateUser` / `UpdateUser`: hashear siempre antes de persistir. Nunca devolver el hash al cliente (el `User` del `WebToken` no debe llevar `Password` en JSON del API).
5. Script SQL one-shot **opcional** para hashear offline; la migración perezosa cubre usuarios que no se toquen.
6. Tests: plano→hash en login; hash verifica; password incorrecto; usuario inactivo; el API no serializa `Password`.

**Criterio de salida:** un usuario de prueba en Docker hace login escritorio y API; la fila queda hasheada; un segundo login sigue funcionando; `varchar` viejo de 50 no trunca el hash.

**No hacer:** forzar reset masivo a los contadores; cambiar usernames; meter Identity/ASP.NET Identity.

### Fase 8 — Apagar CapaLogica en pantallas vivas

Orden (de más riesgo contable a menos):

1. **Cierre de periodo** — `FrameAsientoCierre` pasa a `IFinancialService` (`GetPostingPeriods`, `ClosePostingPeriod`). Misma checklist de cierre que hoy. Tests del servicio **antes** de cambiar el form.
2. **Reportes clásicos** — `ReporteCuenta`, `ReporteBalanceSituacion`, `ReporteMovimientosCuenta`, `FrameReporteAuxiliares` dejan `CuentaCL`. Usan `GetAccounts` + `FillAccountsWithBalances` / `AccountRules` (ya en Core). Excel sigue saliendo de `CapaEntidad.Reportes` (solo layout). Comparar un mes cerrado conocido: mismos totales.
3. **Permisos** — portar `PermisoCL` + lecturas de `PermisoDAO` a `PermissionService` + repos Dapper (`modules`, `windows`, `windows_permission`, `companies_permission`). Admin bypasea igual. `Guachi` se queda como helper de UI o se mueve a Entidad; no duplicar reglas. `FormPermisoUsuario` y el setter de `GlobalConfig.User` usan el servicio.
4. **Correo** — `CorreoCL` → servicio fino + el mismo log `usuarios_correo`. No es bloqueante para 9 si el form se usa poco; sí hay que sacarlo para borrar CapaLogica.
5. Borrar `MaestroCuentas/LGCuenta` y `MaestroCuenta` (no los abre el menú).
6. Marcar `CuentaCL`, `FechaTransaccionCL`, `CompañiaCL`, `UsuarioCL` como `[Obsolete]` cuando cero referencias de UI. No borrar CapaLogica todavía (fase 9).

**Criterio de salida:** grep de `CapaPresentacion` sin `new CuentaCL` / `FechaTransaccionCL` / `PermisoCL` / `CompañiaCL` / `UsuarioCL`. Checklist §6 de migración (cierre, reportes clásicos, permisos admin vs usuario) verde en compañía de prueba.

**No hacer:** reescribir Excel; API-izar permisos; tocar Blazor.

### Fase 9 — Un modelo en la UI

**Qué**

1. Forms y reportes clásicos hablan `User` / `Account` / `PostingPeriod` / `Company`. Adaptadores puntuales hacia lo que Excel aún espera (`Usuario`, `Cuenta`, `FechaTransaccion`) viven **dentro** de `CapaEntidad.Reportes`, no en `GlobalConfig`.
2. `GlobalConfig.Usuario` desaparece o queda como wrapper obsoleto un release.
3. Quitar el puente User→Usuario del setter de `GlobalConfig.User`.
4. Cuando CapaPresentacion ya no referencie `CapaLogica` ni DAOs, quitar el `ProjectReference`. CapaDatos queda referenciada solo si algún Excel aún dispara SQL (no debería).

**Criterio de salida:** `CapaPresentacion` no referencia `CapaLogica`. Tests de Desktop que hoy instancian `*CL` se reescriben contra Services o se borran si duplican `AriesContador.Tests`.

**No hacer:** mover carpetas a `src/`; renombrar `CapaPresentacion`.

### Fase 10 — Higiene de Data, DI y código muerto

**Qué**

1. WinForms: `IUnitOfWork` y servicios **scoped/transient** por operación, o factory por form. Dejar de ser Singleton. `IConnectionString` sí puede ser singleton.
2. Decidir `Commit()`: o envuelve una transacción de verdad y los repos la usan, o se elimina de la interfaz y se documenta “transacción en el repo X”. No las dos.
3. **Hecho:** un solo contrato async (`*Async` + `CancellationToken`) en Core → Data → Services → API → WinForms. `MySqlDataAccess` unificado; TX con `OpenAsync`/`BeginTransactionAsync`. Sin `GetResult` en servicios ni forms.
4. Borrar `HttpAdministrationService`, `HttpFinancialService` y el cliente HTTP si nadie los referencia. El contrato HTTP vive en el host, no en un segundo servicio paralelo.
5. **Hecho:** ya no hay `GetAwaiter().GetResult()` en Administration ni en FinancialService.
6. **Hecho:** `FindByPostingPeriodIdAsync` carga líneas en la misma transacción async.

**Criterio de salida:** cero `NotImplementedException` alcanzables desde UI o Minimal APIs; DI del exe no es todo-Singleton; grep sin `HttpFinancialService`.

**No hacer:** reescribir Dapper a EF.

### Fase 11 — Integridad MySQL (copia → RDS)

Scripts versionados en `scripts/mysql/fase11/`, mismo estilo que `fase1` / `fase3`.

1. Inventario de duplicados `(company_id, month_report)`; unique si está limpio.
2. Plan de `company_id` varchar(4) vs (5): medir códigos reales (`C1000` ya cabe en 5); unificar en un ALTER con backup.
3. FKs en `companies_permission` y `windows_permission` (limpiar huérfanos antes).
4. `companies.user` NOT NULL si no hay filas nulas; si las hay, asignar un admin y luego NOT NULL.
5. No “arreglar” `updated_by` a ciegas (mezcla cédula y `user_id`); documentar el campo y dejar de escribir cédulas en código nuevo.

**Criterio de salida:** scripts corridos en Docker; app local verde; luego ventana en RDS. `MODELOS-BD.md` actualizado.

**No hacer:** tablas `actividades` / `tareas`; cambiar enums; hash (eso es fase 7).

### Fase 12 — Tests y CI (paralelo desde fase 6)

**Qué**

1. `AriesContador.Tests` ya está en **net8.0**; el workflow usa `global.json` (no instala 3.1.x).
2. `Aries.WebAPI.Tests` está en `ci.yml` (Windows y Ubuntu).
3. Tests nuevos: hash (fase 7), permisos (fase 8), cierre de periodo, que el JSON de login no lleva password.
4. Job `linux-mysql` en `ci.yml` corre `Aries.Data.Tests` contra MySQL 8 de servicio, **nunca** contra RDS.
5. Verify-publish: fallar si el `.config` de Release contiene host RDS + password (guardaespaldas de fase 6).

**Criterio de salida:** PR a `dev`/`main` corre desktop + core net8 + API tests + Data.Tests con MySQL; el job de secretos falla si alguien reintroduce la connection string.

### Fase 13 — API de producción (cuando se necesite)

Solo si se va a apagar el EBS viejo o a publicar el host nuevo. El escritorio **sigue in-process**.

1. Deploy del `Aries.WebAPI` de la raíz (Docker ya existe) con secretos por env (fase 6) y hash (fase 7).
2. HTTPS; JWT con key ≥ 32 bytes aleatoria, distinta de Local.
3. Rate limit en `POST /auth/login`.
4. CORS: orígenes reales, no la IP de desarrollo.
5. Health check para el load balancer (`/health` ya abre MySQL).
6. Contrato: mismos tests `HttpContractTests` contra el host desplegado (smoke).
7. Apagar o redirigir `arieswebapi7-dev.eba-32ctm9kr…` cuando nada lo use.

**Criterio de salida:** smoke login + `company/getAll` con Bearer contra el host nuevo; EBS viejo fuera de DNS/config; Swagger no público.

**No hacer:** hacer el exe cliente delgado del API; levantar Blazor.

### Fase 14 — Operación (después de 8)

1. Logging estructurado en API (`ILogger`); en WinForms, no `catch {}` vacíos en flujos contables (el de Squirrel sí puede seguir silencioso).
2. Auditoría mínima: `updated_by` = `user_id` real en writes nuevos.
3. Runbook: backup RDS → restore Docker; cómo recrear `aries_mysql_local`.
4. Squirrel: URL de update por ambiente; no mezclar Local y Production.
5. Decisión documentada sobre `AriesWebApi/` anidado: archivar el remote, no fusionar historiales en este plan.

**Criterio de salida:** un operador puede levantar Local desde el README y restaurar un dump de prueba sin preguntar en el chat.

---

## 6. Checklist de no-regresión (todas las fases)

Misma compañía de prueba (Docker, no RDS producción).

### Arranque y auth

- [x] Exe Debug apunta a `:3307` en `app.config` (sin RDS). Arranque GUI: prueba manual local.
- [x] Login hashea en el servicio (plano→PBKDF2). Falta confirmar en Docker si el ALTER fase 7 ya corrió.
- [x] Permisos pasan por `IPermissionService` (mismas reglas SQL que `Guachi`).
- [x] Sin credenciales válidas, la app cierra (mismo `LoginForm`).

### Contabilidad

- [ ] Crear compañía copiando plan: mismo número de cuentas que el origen.
- [ ] Árbol de cuentas, no borrar sistema/no-auxiliar, auxiliar hereda saldos.
- [ ] Asiento cuadrado; soft delete y restore; consecutivo por periodo.
- [x] Cierre de periodo en `FrameAsientoCierre` llama `ClosePostingPeriod` (mismo servicio que administrar meses).

### Reportes

- [ ] Comprobación, estado de resultados, asientos (stack nuevo).
- [x] Reportes clásicos leen `GetAccounts` + `FillAccountsWithBalances` / movimientos Dapper. Comparar Excel de un mes cerrado queda como prueba manual.

### API (si está levantado)

- [ ] `POST /auth/login` anónimo; resto con Bearer.
- [ ] JSON PascalCase (Newtonsoft del WinForms).
- [x] Respuesta de login **sin** password (`JsonIgnore` + test HTTP).

---

## 7. Qué no entra en este plan

- Reescribir WinForms en Blazor, WPF o `net8.0-windows`.
- Cambiar MySQL por otro motor.
- Renombrar columnas/SPs al inglés.
- Fusionar el git anidado `AriesWebApi/` con el padre.
- Identity Server, OAuth de Google/Microsoft, MFA (se puede estudiar **después** de fase 7).
- Tablas `actividades` / `tareas`.
- Mover el árbol a `src/` / `tests/` (norte de migración §4.3; costo ClickOnce/Squirrel).

---

## 8. Riesgos

| Riesgo | Impacto | Mitigación |
|---|---|---|
| Rotar RDS y olvidar una máquina / Squirrel config | Nadie entra | Ventana corta; config local y prod separadas; lista de PCs |
| Hash con `varchar(50)` | Truncate silencioso, login roto | ALTER primero; test de longitud |
| Migración perezosa a medias (un usuario a veces plano) | Logins aleatorios | Un solo `Login()`; test de rehash; opcional script masivo |
| Portar reportes clásicos cambia redondeo | Balances distintos | Excel de referencia de un mes cerrado |
| Permisos mal portados | Usuario ve compañías ajenas | Tests de `companies_permission` + checklist Guachi |
| Unique en `accounting_months` con duplicados reales | ALTER falla | SELECT de duplicados **antes** del unique |
| Publicar API con JWT `change-me` | Tokens falsos | Fase 6 bloquea 13; CI busca el placeholder |
| Singleton → scoped rompe un form que cachea el servicio | Null/ObjectDisposed | Un form = resolver al abrir; no guardar UoW estático |

---

## 9. Orden resumido

```text
6  Secretos + rotar RDS/JWT                         → git limpio, Local ≠ RDS
7  Hash PBKDF2 + ampliar columna + rehash en login  → BD filtrada no entrega claves
8  Cierre, reportes clásicos, permisos, correo      → UI sin new *CL
9  Un modelo (Core) en forms; CapaLogica fuera      → un dominio
10 DI scoped, borrar HTTP muerto, Commit honesto    → menos trampas
11 Unique meses, FKs, company_id (copia → RDS)      → dump coherente
12 Tests net8 + API en CI + guardia de secretos     → paralelo desde 6
13 API prod (HTTPS, rate limit, apagar EBS)         → solo si se necesita
14 Runbook, logs, auditoría, destino de AriesWebApi → operar sin magia
```

**Estado local 2026-09:** fases **6–10** y **12** implementadas en este workspace. Fase **11** aplicada en Docker salvo unique de meses (hay duplicados en `C001`). Fase **13** no se hace aquí (no deploy). Fase **14** documentada en `scripts/local/README.md`. Rotar RDS/JWT en AWS queda fuera (solo local).
