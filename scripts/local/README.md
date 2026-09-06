# Ambientes Local y Production

Hay **dos** configuraciones. F5 / Debug queda en Local. Production es Release (escritorio) o `ASPNETCORE_ENVIRONMENT=Production` (API).

| | Escritorio | API |
|---|---|---|
| **Local** | configuración **Debug** → `app.config` | perfil **Aries.WebAPI (Local)** / `Development` → `appsettings.Development.json` |
| **Production** | configuración **Release** → `App.Production.config` | perfil **Aries.WebAPI (Production)** → `appsettings.Production.json` |

Override opcional (gitignored): `App.Local.config` (gana sobre `app.config` en Debug) y `appsettings.Local.json` (solo si el ambiente no es Production).

El título del menú muestra `[Local]` o `[Production]`. Menú Token info enseña ambiente y host MySQL.

## Local (Docker :3307)

El Debug del escritorio y `dotnet run` (Development) apuntan a **127.0.0.1:3307**, base `aries`, usuario `kenneth` / `1234`. No usan RDS.

En esta máquina ya puede existir el contenedor `aries_mysql_local` en 3307; el script no levanta un segundo MySQL si el puerto está ocupado.

### 1. Comprobar MySQL

```powershell
.\scripts\local\start-local.ps1
docker ps --filter "publish=3307"
```

Si no hay nada en 3307: Docker Desktop + `docker compose up -d` (credenciales en `.env`).

### 2. Restaurar dump (solo primera vez, copia — nunca RDS)

Orden: `users` → `companies` → `accounts_names` → `accounts` → meses / asientos → `aries_routines.sql` → `scripts/mysql/fase1` y `fase3`.

### 3. API local (no arranca con F5 del escritorio)

El WinForms Debug **no necesita** el API: login, compañías, cuentas y asientos van directo a MySQL. F5 en **CapaPresentacion** no levanta `Aries.WebAPI`.

Si quieres Swagger o el HTTP junto al exe, arranca el API aparte. Los dos deben usar **http://localhost:5088/** (no el 5000 de Kestrel ni el 44320 viejo).

```powershell
dotnet run --project Aries.WebAPI
```

O en Visual Studio: perfil de inicio **Escritorio + API (Local)**.

Al arrancar debe loguear `Ambiente Development: MySQL 127.0.0.1:3307 / aries` y `Aries.WebAPI escuchando en http://localhost:5088`.

### 4. Escritorio local

Visual Studio / Cursor: proyecto de inicio **CapaPresentacion**, configuración **Debug**.

`CapaPresentacion\app.config`:

- `DBconnectionString` → 127.0.0.1:3307 / aries / kenneth
- `HttpBaseUrl` → http://localhost:5088/
- `UpdateServerString` vacío (no llama a S3)
- `EnvironmentName` → Local

Otra clave o puerto: copia `app.config` a `App.Local.config` y edita. El build Debug lo aplica al exe.

## Production (RDS)

Release del escritorio y el perfil Production de la API usan RDS `ariescontrol...rds.amazonaws.com:3306`, usuario `kenneth`, base `aries`.

- Escritorio: `HttpBaseUrl` es Elastic Beanstalk (API desplegada), `UpdateServerString` es S3, `IsBeta=false`.
- API local con perfil Production: misma cadena RDS. **No aplicar SPs de fase 1/3 sobre RDS.** Esa instancia es producción real.

```powershell
dotnet run --project Aries.WebAPI --launch-profile "Aries.WebAPI (Production)"
```

Al arrancar debe loguear `Ambiente Production: MySQL ariescontrol... / aries`.

Visual Studio: configuración **Release** en CapaPresentacion. El target `ApplyEnvironmentAppConfig` copia `App.Production.config` → `CapaPresentacion.exe.config`.

## Archivos

| Archivo | Rol |
|---|---|
| `.env.example` → `.env` | Docker local (root, kenneth, puerto 3307) |
| `docker-compose.yml` | MySQL 8 si no hay contenedor en 3307 |
| `CapaPresentacion/app.config` | Debug / Local |
| `CapaPresentacion/App.Production.config` | Release / Production (RDS) |
| `CapaPresentacion/App.Local.config.example` | Override Debug (gitignored al copiar) |
| `Aries.WebAPI/appsettings.Development.json` | API Local |
| `Aries.WebAPI/appsettings.Production.json` | API contra RDS |
| `Aries.WebAPI/appsettings.Local.json.example` | Override API (no se carga en Production) |
