# Ambientes Local y Production

Hay **dos** configuraciones. F5 / Debug del escritorio queda en Local. El API local corre **en Docker** junto a MySQL.

| | Escritorio | API + MySQL |
|---|---|---|
| **Local** | **Debug** → `app.config` (MySQL `127.0.0.1:3307`, API `http://localhost:5088/`) | `docker compose` → `aries_mysql_local` + `aries_api_local` |
| **Production** | **Release** → `App.Production.config` (RDS + Elastic Beanstalk) | no usar este compose; es RDS real |

El título del menú muestra `[Local]` o `[Production]`.

## Local (Docker)

Topología igual que prod: un contenedor de API y uno de MySQL. El escritorio Debug sigue en el host y entra por los puertos publicados.

```powershell
.\scripts\local\start-local.ps1
```

Eso deja:

| Contenedor | Puerto en el host |
|---|---|
| `aries_mysql_local` | **3307** → MySQL 3306 |
| `aries_api_local` | **5088** → HTTP 8080 |

Swagger: http://localhost:5088/  
Salud (incluye ping a MySQL): http://localhost:5088/health

Si ya tienes `aries_mysql_local` en 3307, el script **no** crea otro MySQL; solo construye y arranca el API.

### Restaurar dump (solo primera vez, copia — nunca RDS)

Orden: `users` → `companies` → `accounts_names` → `accounts` → meses / asientos → `aries_routines.sql` → `scripts/mysql/fase1` y `fase3`.

### Escritorio

Proyecto de inicio **CapaPresentacion**, configuración **Debug**. Login, maestros y asientos van **in-process** a MySQL `:3307` (no necesitan el API). `HttpBaseUrl` apunta a `http://localhost:5088/` para cuando sí uses HTTP.

## Production (RDS)

Release del escritorio usa RDS y Elastic Beanstalk. **No aplicar SPs de fase 1/3 sobre RDS.**

```powershell
dotnet run --project Aries.WebAPI --launch-profile "Aries.WebAPI (Production)"
```

Eso es el API **en el host** contra RDS, no el contenedor local. Visual Studio: configuración **Release** en CapaPresentacion.

## Archivos

| Archivo | Rol |
|---|---|
| `.env.example` → `.env` | Docker (MySQL, puerto API, JWT) |
| `docker-compose.yml` | MySQL 8 + `Aries.WebAPI` |
| `Aries.WebAPI/Dockerfile` | Imagen del API |
| `CapaPresentacion/app.config` | Debug / Local |
| `CapaPresentacion/App.Production.config` | Release / Production (RDS) |
| `Aries.WebAPI/appsettings.Development.json` | Fallback si corres el API fuera de Docker |
| `Aries.WebAPI/appsettings.Production.json` | API contra RDS (no Docker local) |
