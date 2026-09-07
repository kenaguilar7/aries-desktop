# Ambientes Local y Production

Hay **dos** configuraciones. F5 / Debug del escritorio queda en Local. El API local corre **en Docker** junto a MySQL.

| | Escritorio | API + MySQL |
|---|---|---|
| **Local** | **Debug** → `app.config` (MySQL `127.0.0.1:3307`, API `http://localhost:5088/`) | `docker compose` → `aries_mysql_local` + `aries_api_local` |
| **Production** | **Release** → `App.Production.config` (sin password) + `App.Production.local.config` o `ARIES_MYSQL_CONNECTION` | secretos por env / user-secrets, no el JSON versionado |

El título del menú muestra `[Local]` o `[Production]`.

## Local (Docker)

```powershell
.\scripts\local\start-local.ps1
```

| Contenedor | Puerto en el host |
|---|---|
| `aries_mysql_local` | **3307** → MySQL 3306 |
| `aries_api_local` | **5088** → HTTP 8080 |

Salud (incluye ping a MySQL): http://localhost:5088/health  
Swagger solo en Development/Local: http://localhost:5088/

Si ya tienes `aries_mysql_local` en 3307, el script **no** crea otro MySQL; solo construye y arranca el API.

### Restaurar dump (copia — nunca RDS producción)

1. Tener `aries_mysql_local` arriba (`docker compose up -d mysql` o `start-local.ps1`).
2. Restaurar en este orden: `users` → `companies` → `accounts_names` → `accounts` → meses / asientos → `aries_routines.sql`.
3. Aplicar scripts de app: `scripts/mysql/fase1`, `fase3`, **`fase7`** (ancho de password) y, si el dump está limpio, `fase11` (inventario de duplicados **antes** del unique).
4. Ejemplo (host → contenedor):

```powershell
Get-Content .\dump.sql -Raw | docker exec -i aries_mysql_local mysql -ukenneth -p1234 aries
```

No hay job automático contra RDS. Un backup de RDS se baja **fuera** de este repo y se restaura solo a Docker.

### Escritorio

Proyecto de inicio **Aries.Desktop** (`src/desktop/Aries.Desktop`, output `CapaPresentacion.exe`), configuración **Debug**. Login, maestros y asientos van **in-process** a MySQL `:3307`. `HttpBaseUrl` es solo diagnóstico.

Tras fase 7, el primer login de un usuario en plano deja la fila hasheada (`pbkdf2$...`).

## Production (máquina / pipeline)

`App.Production.config` **no** lleva password. Copia `App.Production.local.config.example` → `App.Production.local.config` (gitignored) o define `ARIES_MYSQL_CONNECTION`. JWT del API: `Jwt__Key` (mín. 32 bytes), nunca el placeholder.

**No aplicar SPs de fase 1/3/7/11 sobre RDS** desde este workspace. Eso es una ventana de operación aparte.

## Archivos

| Archivo | Rol |
|---|---|
| `.env.example` → `.env` | Docker (MySQL, puerto API, JWT) |
| `docker-compose.yml` | MySQL 8 + `Aries.WebAPI` |
| `src/desktop/Aries.Desktop/app.config` | Debug / Local (Docker `:3307`) |
| `src/desktop/Aries.Desktop/App.Production.config` | Release sin secretos |
| `src/desktop/Aries.Desktop/App.Production.local.config` | Secretos de máquina (gitignored) |
| `src/hosts/Aries.WebAPI/appsettings.Development.json` | Fallback local |
| `src/hosts/Aries.WebAPI/appsettings.Production.json` | Vacío; secretos por env |
