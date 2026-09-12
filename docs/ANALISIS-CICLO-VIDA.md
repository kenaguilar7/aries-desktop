# Análisis: ciclo de vida de Aries Contador

**Fecha:** 7 sep 2026  
**Actualizado:** 7 sep 2026 (mejoras P0–P2 implementadas en código)  
**Alcance original:** cómo arranca, se configura, se actualiza y se entrega el producto (`Aries.sln`, escritorio **1.2.0**).  
**Complementa:** [`LAYOUT.md`](LAYOUT.md), [`scripts/local/README.md`](../scripts/local/README.md), [`PLAN-MEJORAS.md`](PLAN-MEJORAS.md).

**Estado de las mejoras:** el guardrail de migraciones en Debug, `GET_LOCK`, checksum en `__schema_migrations`, splash + Squirrel con reinicio, log `%LocalAppData%\AriesContador\startup.log`, `UpdateUrl` en appSettings, `cd-prod` + `Publish-Production.ps1`, logout, handlers de excepción, Dispose del DI y `version.props` ya están en el árbol. Lo que sigue siendo operación humana: rotar secretos AWS, crear el environment GitHub `production` y no aplicar migraciones a RDS prod desde F5 (el flag `ARIES_APPLY_MIGRATIONS=1` es explícito).

**Respuesta corta (tras las mejoras):** el proceso muestra splash (config → Squirrel → migraciones → menú), no auto-migra RDS desde Debug, registra arranque en `%LocalAppData%\AriesContador\startup.log`, y la entrega de clientes tiene `cd-prod` (tag + confirmar `production`) además de `cd-test`. La config por ambiente sigue siendo Local / QA / Test / Production; `connectionStrings` solo lleva MySQL.

---

## 1. Qué es “ciclo de vida” aquí

No hay un solo pipeline. El producto vive en cuatro capas que se cruzan:

```text
Código  ──►  CI (cada PR)  ──►  Pack Squirrel (a mano, canal test)
                                      │
                                      ▼
                              Feed HTTP (S3 o laptop :5088/updates)
                                      │
                                      ▼
PC cliente ── arranque ── migraciones MySQL ── login ── sesión ── salir
                 ▲
                 └── Squirrel pregunta al feed y puede reemplazar el exe
```

| Capa | Quién la corre | Dónde está |
|---|---|---|
| **Proceso (runtime)** | El exe WinForms y, si está arriba, el API | `Program.cs`, `GlobalConfig`, `DatabaseMigrator`, `FrameMenu` |
| **Configuración** | MSBuild + archivos de máquina | `app.config`, `App.Production.config`, `local-db.json`, env vars, overlay de CI |
| **Esquema MySQL** | El primer proceso que conecta (exe o API) | `AriesContador.Data/Migrations` (`M001`…`M013`) |
| **Entrega** | GitHub Actions + scripts | `ci.yml`, `cd-test.yml`, `Pack-Squirrel.ps1`, S3 `updates` / `updates-test` |

El escritorio de producción **no necesita** el API para login, maestros ni asientos. El API es host HTTP + feed Squirrel en QA local. Eso importa: el ciclo de vida del contador es el del **exe + MySQL**, no el de un cliente delgado.

---

## 2. Ciclo del proceso (lo que ve el usuario)

### 2.1 Arranque del escritorio

Orden real en `Aries.Desktop/Program.cs`:

```text
Main (STAThread)
  │
  ├─ EnableVisualStyles
  ├─ new GlobalConfig()
  │     ├─ HttpBaseUrl → EnvironmentVariable.ApiUrl  (diagnóstico; login no lo usa)
  │     ├─ valida que DBconnectionString tenga Server=
  │     └─ CheckForUpdates()  ← Task sin await (Squirrel en paralelo)
  │
  ├─ DatabaseMigrator.ApplyPendingAsync().GetResult()
  │     si falla → MessageBox y return (no hay UI)
  │
  ├─ ServiceCollection
  │     IConnectionString singleton
  │     UoW + Admin / Financial / Report / Permission / Email  → transient
  │     FrameMenu transient
  │
  ├─ GlobalConfig.Services = provider
  └─ Application.Run(FrameMenu)
        └─ LoginForm modal
              OK + usuario  → menú MDI
              cancel / User null → Application.Exit()
```

**Login** (`LoginForm` → `AdministrationService.LoginAsync`): busca usuario, verifica PBKDF2 (o plano y rehash), devuelve `WebToken { Token = "local" }`. Luego `GlobalConfig.SetUserAsync` carga módulos con `IPermissionService`.

**Sesión:** estado estático en `GlobalConfig` (`User`, `Usuario`, `Company`, `Cuentas`, `Permisos`). Cambiar de compañía (`FrameSeleccionCompañia`) recorre forms `INeedValidatedForClose` y cierra los MDI abiertos salvo el menú. **Salir** es `Application.Exit()`; no hay logout / re-login sin reiniciar el proceso.

**Cierre:** no hay `Application.ApplicationExit` ni `Dispose` del `ServiceProvider`. Los `catch {}` de Squirrel y de carga de permisos tragan errores.

### 2.2 Arranque del API (`Aries.WebAPI`)

```text
CreateBuilder
  ├─ appsettings + (si no Production) appsettings.Local.json
  ├─ JWT obligatorio (en Production no puede ser "change-me")
  ├─ mismos servicios que el exe (scoped, no transient)
  │
Build
  ├─ si no Testing: DatabaseMigrator.ApplyPendingAsync()
  ├─ Swagger solo Development / Local
  ├─ estáticos /updates  (feed Squirrel)
  ├─ /health  (abre MySQL)
  └─ Minimal APIs (auth, company, …)
```

Docker local (`docker-compose.yml`): MySQL `:3307` + API `:5088`, JWT de desarrollo, `Updates__Root=/app/updates` montado en `publish/updates`.

### 2.3 Dos procesos pueden migrar la misma base

Exe y API ejecutan **el mismo** `DatabaseMigrator` al subir. En una laptop QA eso es conveniente (el que llegue primero deja el esquema al día). En un arranque simultáneo hay carrera: no hay lock distribuido; las migraciones son en su mayoría `CREATE/DROP PROCEDURE` e `IF NOT EXISTS`, así que suele ser idempotente, pero no está garantizado para un `ALTER` a medias.

---

## 3. Cómo está configurado

### 3.1 Ambientes

| Nombre (`EnvironmentName`) | Cómo se elige | MySQL | Update (Squirrel) | Quién lo usa |
|---|---|---|---|---|
| **Local** | Debug: `app.config` | `127.0.0.1:3307` / `aries` | vacío (no actualiza) | F5 en esta PC |
| **AriesTest** / `test` | Debug: `local-db.json` `"use": "aries-test"` | RDS `aries-test` (secretos en archivo gitignored) | el de `app.config` (vacío) | Dev contra copia RDS |
| **Qa** | Release + `App.Production.local.config` | laptop `:3307` | `http://<laptop>:5088/updates/` | PCs cliente → Docker LAN/VPN |
| **Production** | Release + overlay de pack / máquina | RDS prod (secret en CI o `App.Production.local.config`) | `https://ariescontadorcr.s3.us-east-2.amazonaws.com/updates/` | Clientes reales |
| **test (canal Squirrel)** | `cd-test.yml` overlay XML | secret `ARIES_MYSQL_CONNECTION` | secret `ARIES_UPDATE_URL` (`…/updates-test/`) | Ensayo de pack; `IsBeta=true` |

El título del menú muestra `v.{FileVersion} [{EnvironmentName}] {Database}`. Eso es la única señal visible de “a qué mundo estoy pegado”.

### 3.2 Fuentes de config (prioridad real)

**Escritorio Debug**

1. MSBuild copia `app.config` → `CapaPresentacion.exe.config`.
2. Si no existe `local-db.json`, lo crea desde `scripts/local/local-db.json.example`.
3. `Apply-LocalDbJson.ps1`: si `"use"` no es `docker`, **pisa** `DBconnectionString` y `EnvironmentName` en el exe.config.
4. En runtime **no** lee `ARIES_MYSQL_CONNECTION` (`#if !DEBUG` en `ConnectionString.cs`).

**Escritorio Release**

1. MSBuild sustituye el exe.config por `App.Production.local.config` si existe; si no, `App.Production.config` (sin password, `Server=SET_ON_MACHINE`).
2. En runtime: `ARIES_MYSQL_CONNECTION` o `ConnectionStrings__MySQLDefault` **ganan** sobre el archivo.
3. Squirrel: `ARIES_UPDATE_URL` o `UpdateServerString`.
4. El pack de CI (`cd-test`) genera un overlay XML temporal con DB + URL de update y se lo pasa a `Pack-Squirrel.ps1`. Ese overlay **sí** viaja dentro del nupkg.

**API**

`ConnectionStrings:MySQLDefault` y `Jwt:Key` por JSON de ambiente o env (`ConnectionStrings__MySQLDefault`, `Jwt__Key`). Production versionado está vacío a propósito.

### 3.3 Qué hay en `app.config` / `App.Production.config`

| Clave | Tipo real | Uso hoy |
|---|---|---|
| `DBconnectionString` | connection string | Camino vivo (Dapper in-process) |
| `UpdateServerString` | URL, **mal alojada** como connectionString | Squirrel |
| `HttpBaseUrl` | URL, igual | Solo el diálogo “Token info”. Login no llama HTTP |
| `IsBeta` | flag, igual | **Nadie lo lee en C#** |
| `EnvironmentName` | appSettings | Título y `IsLocalEnvironment` |
| SMTP (`SmtpHost`…) | appSettings | `IEmailService` |
| `supportedRuntime` sku **v4.6.1** | startup | Mentira: el csproj es **net48** |
| `system.web` membership / trust | legado ClickOnce/CAS | Muerto |
| binding redirects | runtime | Vivos; `Verify-DesktopPublish` los coteja con las DLL |

SMTP vacío = correo desactivado sin crash (el servicio debe tolerarlo).

### 3.4 Secretos

Versionados **sin** host RDS ni password: lo refuerzan `Verify-NoSecrets.ps1` (CI) y `Verify-DesktopPublish.ps1`. Gitignore cubre `local-db.json`, `App.Production.local.config`, `.env`, `appsettings.Local.json`.

El agujero residual es de **máquina**, no de git: un Debug con `"use": "aries-test"` aplica migraciones al RDS de prueba en el arranque, sin confirmación.

---

## 4. Ciclo del esquema (migraciones)

Catálogo en `SchemaMigrations.All`: tipos `SqlMigration` ordenados por `Version`. Historial en MySQL `__schema_migrations`.

| Id | Qué hace |
|---|---|
| `001` | Crea `__schema_migrations` |
| `002` | SPs de compañía |
| `003` | SPs del maestro de cuentas |
| `004` | Ancho de password + SPs de usuario |
| `005` | `company_id` varchar(5) |
| `006` | FKs de permisos |
| `007` | `companies.user` NOT NULL (si se puede) |
| `008` | UNIQUE `(company_id, month_report)` **solo si no hay duplicados** |
| `009` | Funciones de path de cuenta |
| `010` | ensancha `company_id` en SPs del dump |
| `011` | vista `account_info` |
| `012` | vista `accounting_entries_info` |
| `013` | `SP_CopyChartOfAccounts` / `SP_InsertChartFromTemp` (copia de plan en bloque) |

Comportamiento:

- Se aplican **todas las pendientes** antes de mostrar login (exe) o de escuchar HTTP (API).
- Si un lote falla, esa migración **no** se registra; los lotes anteriores del mismo script **sí** quedaron ejecutados (no hay transacción alrededor del script completo). Los `DROP PROCEDURE IF EXISTS` mitigan el reintento.
- Timeout 180 s. Usuario MySQL sin `SYSTEM_USER` no puede reemplazar rutinas creadas por root (hint + `grant_routine_replace.sql` en Docker).
- No hay rollback ni “dry-run”. `M008` se marca aplicada aunque haya saltado el UNIQUE por duplicados: la historia dice “ok” y el índice puede no existir.

Esto es un migrador de app, no Flyway/EF. Para un exe 4.8 que comparte RDS con muchos clientes, **el riesgo operativo es aplicar de más**, no de menos.

---

## 5. Ciclo de entrega (dev → cliente)

```text
PR / push  master|main|dev
        │
        ▼
   ci.yml
   ├─ Windows: nuget restore, Verify-NoSecrets, MSBuild Release,
   │           tests Core / Desktop / WebAPI, Verify-DesktopPublish
   └─ Ubuntu + MySQL 8 :3307: Aries.Data.Tests (migraciones) + WebAPI.Tests

workflow_dispatch  cd-test.yml  (environment GitHub "test")
        │
        ├─ mismo build + tests Desktop
        ├─ overlay: RDS + URL updates-test  (también mete HttpBaseUrl del EBS viejo)
        ├─ Pack-Squirrel.ps1 -PackageId AriesUpdater
        ├─ artifact squirrel-feed-test
        └─ opcional: s3://ariescontadorcr/updates-test/

Producción S3  s3://ariescontadorcr/updates/
        │  Publish-SquirrelFeed.ps1 -Environment Production -ConfirmProduction
        │  (script local; NO hay workflow cd-prod)
        ▼
Cliente instalado (Squirrel, carpeta app-x.y.z)
        └─ al abrir: UpdateManager.UpdateApp() si hay UpdateServerString
```

Versión del exe: `AssemblyFileVersion` en `Properties/AssemblyInfo.cs` (**hoy 1.2.0**). `Pack-Squirrel.ps1` lee ese número; no hay tag git automático. El id del paquete de clientes es `AriesUpdater`; el default del script local sigue siendo `CapaPresentacion` (el exe no se renombra).

`scripts/local/README.md` habla de un Release de GitHub por tag `vX.Y.Z`. **Ese workflow no existe** en `.github/workflows/`. Lo que existe es `cd-test` a mano.

### 5.1 Squirrel en runtime

`GlobalConfig.CheckForUpdates` usa `UpdateManager.UpdateApp()` y **traga cualquier excepción**. No espera el `Task` en el constructor. Consecuencias:

- El usuario entra mientras (o aunque) falle el download.
- Si Squirrel aplica archivos, **no hay prompt de reinicio**; la sesión sigue en el binario viejo hasta el próximo arranque.
- Un feed vacío o 404 es silencioso (correcto para Local; opaco en Production).

---

## 6. Qué está bien (no romper)

1. **Un camino de negocio in-process.** Login y maestros ya no mezclan HTTP + `*CL` en el mismo botón.
2. **Debug ≠ Release.** F5 por defecto es Docker `:3307`. Production config no lleva password.
3. **Guardias en CI.** Secretos prohibidos, redirects vs DLL, nada de `MySql.Data` de Oracle.
4. **Canal test aislado.** `cd-test` no puede escribir `s3://…/updates/` (producción).
5. **Migraciones versionadas en C#** con tests en Ubuntu+MySQL (`Aries.Data.Tests`).
6. **QA laptop** coherente: un compose para BD + API + feed HTTP, sin S3.
7. **DI del exe** con UoW/servicios transient (ya no todo-Singleton).
8. **Título de ventana** con ambiente + base: reduce “estoy en prod sin querer”.

---

## 7. Huecos y mejoras

Prioridad: **P0** riesgo de datos o de release; **P1** operación diaria; **P2** higiene.

### P0 — El arranque escribe esquema en la base que apunte el config

Hoy cualquier F5 o Release aplica `M001`–`M013` sin preguntar. Si `local-db.json` apunta a `aries-test` (o un Release mal copiado a RDS prod), el **primer arranque muta esa base**.

Mejoras:

1. **No migrar en Debug contra un host que no sea `127.0.0.1` / `localhost`**, salvo flag explícito (`ARIES_APPLY_MIGRATIONS=1` o checkbox).
2. En Release, log visible (archivo o MessageBox una vez) de *qué* migraciones se aplicaron, no solo el error.
3. Comando / script `dotnet`/exe de consola `aries-migrate` para ventanas de RDS, y dejar el auto-migrate del GUI solo para Local/QA Docker.
4. Lock MySQL (`GET_LOCK('aries_schema_migrate', 30)`) para exe+API a la vez.

### P0 — No hay entrega de producción en Actions

`cd-test` está bien. Producción depende de alguien con AWS CLI y `-ConfirmProduction`. Fácil equivocar bucket o version.

Mejoras:

1. Workflow `cd-prod` con `environment: production`, secrets distintos, y **nunca** el overlay de `updates-test`.
2. Publicar solo desde tag `vX.Y.Z` que coincida con `AssemblyFileVersion` (el README ya lo asume).
3. Quitar del overlay de `cd-test` el `HttpBaseUrl` del Elastic Beanstalk viejo (`eba-32ctm9kr`). Es config muerta y `Verify-NoSecrets` ya lo prohíbe en archivos versionados; el YAML lo reintroduce en el nupkg de test.

### P1 — Squirrel: esperar, informar, reiniciar

1. `await CheckForUpdates()` **después** de migrar o en un splash, no en el ctor fire-and-forget.
2. Si hay update aplicado: “Se instaló la versión X. Reinicie Aries” + `UpdateManager.RestartApp()`.
3. Si el feed falla en Production/Qa: log a archivo (`%LocalAppData%\AriesUpdater\…`), no `catch {}`.
4. Unificar `PackageId`: documentar que clientes = `AriesUpdater` y fallar el pack local si se usa el default `CapaPresentacion` contra un feed de clientes.

### P1 — Arranque percibido

Hoy migraciones bloquean el hilo STA con `GetResult()` y **sin ventana**. Un dump grande o un SP lento parece “no abre”.

1. Splash / `ApplicationContext` con “Actualizando base de datos…” y “Buscando actualizaciones…”.
2. Evitar `GetResult()` en STA: `async` + `SynchronizationContext` o migrar en `Task.Run` con UI de progreso.
3. Handler `Application.ThreadException` + `AppDomain.UnhandledException` que escriban log y muestren el mensaje; hoy un fallo post-login puede morir sin rastro.

### P1 — Sesión

1. Logout (volver a `LoginForm` sin matar el proceso) y limpiar `GlobalConfig.Company` / cuentas en memoria.
2. `SetUserAsync`: el `catch` que deja `Modulos` vacío convierte un error de permisos en “usuario sin menú” sin aviso.
3. `Dispose` del `ServiceProvider` al salir.

### P2 — Config limpia

1. Sacar `IsBeta`, `HttpBaseUrl`, `UpdateServerString` de `<connectionStrings>`. URL de update y flags → `appSettings` (o un JSON al lado del exe). Dejar connectionStrings **solo** para MySQL.
2. Corregir `supportedRuntime` a `v4.8` (o quitar el sku 4.6.1).
3. Borrar el bloque `system.web` membership/trust si no lo usa nada (reduce ruido y falsos positivos de auditoría).
4. SMTP: documentar “vacío = deshabilitado”; no dejar passwords en exe.config de Release (mismo patrón que MySQL: env o archivo de máquina).

### P2 — Versión y docs

1. Una sola fuente de versión (`Directory.Build.props` o `version.json`) que alimente `AssemblyInfo`, el nupkg Squirrel y el título.
2. Alinear `scripts/local/README.md` con los workflows reales (hoy menciona Releases por tag que no están en `.github`).
3. README del API aún dice que el login compara texto plano; el servicio ya hace PBKDF2 + rehash.

### P2 — Migrador

1. Transacción por migración cuando el SQL lo permita; si un lote no es transaccional (DDL), documentarlo en la clase.
2. No marcar `008_UniqueCompanyMonth` como éxito si `@dupes > 0`; o registrar un resultado `skipped` distinto.
3. Tabla de historial: guardar checksum / hash del SQL para detectar “cambiaron M010 después de aplicada”.

### P2 — API en el ciclo

El exe no depende del API, pero QA sí (feed). Mejoras menores:

1. Compose: `ASPNETCORE_ENVIRONMENT=Local` (o Development **sin** Swagger en LAN) si la laptop es accesible por VPN.
2. JWT de compose sigue siendo el placeholder de 32 bytes: vale para Docker; no reutilizar en ningún host público (fase 13 de mejoras).

---

## 8. Mapa de archivos (consulta rápida)

| Pieza | Archivo |
|---|---|
| Arranque exe | `src/desktop/Aries.Desktop/Program.cs` |
| Sesión + Squirrel | `src/desktop/Aries.Desktop/GlobalConfig.cs` |
| Login | `src/desktop/Aries.Desktop/Login/FrameLoginUsuario.cs` |
| Menú / salir | `src/desktop/Aries.Desktop/Menu/FrameMenu.cs` |
| Config Debug | `src/desktop/Aries.Desktop/app.config` |
| Config Release (sin secretos) | `src/desktop/Aries.Desktop/App.Production.config` |
| F5 otra base | `local-db.json` (gitignored) + `scripts/local/Apply-LocalDbJson.ps1` |
| Transform MSBuild | `Aries.Desktop.csproj` → `ApplyEnvironmentAppConfig` |
| Migrador | `src/infrastructure/AriesContador.Data/Migrations/` |
| Arranque API | `src/hosts/Aries.WebAPI/Program.cs` |
| Docker QA | `docker-compose.yml`, `scripts/local/start-local.ps1` |
| CI | `.github/workflows/ci.yml` |
| Pack test | `.github/workflows/cd-test.yml` |
| Pack local | `scripts/Pack-Squirrel.ps1` |
| Subir S3 | `scripts/ops/Publish-SquirrelFeed.ps1` |
| Versión exe | `src/desktop/Aries.Desktop/Properties/AssemblyInfo.cs` |

---

## 9. Orden sugerido de trabajo

```text
1. Guardrail: no auto-migrar hosts remotos desde Debug
2. Overlay cd-test sin EBS; workflow de prod por tag (o al menos runbook de un comando)
3. Splash + await Squirrel + reinicio si hay update
4. Log de arranque (migraciones aplicadas, feed, ambiente)
5. Limpieza de app.config (flags fuera de connectionStrings, sku 4.8)
6. Logout / handlers de excepción / Dispose DI
7. Una versión, README alineado, checksum en __schema_migrations
```

Nada de esto cambia el modelo contable. El ciclo de vida ya está **configurado por ambientes**; las mejoras son para que un arranque no mute la base equivocada, que un update no sea invisible, y que producción se publique con el mismo rigor que `cd-test`.
