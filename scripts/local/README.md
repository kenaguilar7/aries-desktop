# Ambientes Local y QA (laptop Docker)

Hay **tres** sitios: tu PC de desarrollo, la **laptop/servidor** con Docker (LAN o VPN), y S3 de producción (aún no lo usa este compose).

| | Escritorio | MySQL + API + Squirrel |
|---|---|---|
| **Local (esta PC)** | **Debug** → `app.config` (`127.0.0.1:3307`, API `http://localhost:5088/`). `UpdateServerString` vacío: F5 no se auto-actualiza. | `docker compose` en esta misma máquina |
| **QA (laptop servidor)** | **Release** → `App.Production.local.config` según [`App.Qa.local.config.example`](../../src/desktop/Aries.Desktop/App.Qa.local.config.example). MySQL y updates apuntan al **host de esa laptop**. | El mismo `docker compose` en la laptop. Clientes por VPN/LAN. |
| **Production (S3)** | `App.Production.config` sigue con el bucket `ariescontador/updates` | No es este Docker |

El título del menú muestra `[Local]`, `[Qa]` o `[Production]` según `EnvironmentName`.

## Un solo Docker: BD, API y actualizaciones

Squirrel no necesita S3. Pregunta por HTTP a `UpdateServerString`. En QA esa URL es el **mismo** contenedor del API:

```text
PCs con Aries Release  ──MySQL :3307──►  laptop Docker  (aries_mysql_local)
                       ──HTTP  :5088──►  aries_api_local
                            /health
                            /auth/...
                            /updates/RELEASES   ← feed Squirrel (publish/updates)
```

Al arrancar, el API aplica migraciones SQL pendientes. El exe Release también las aplica si llega primero. GitHub Actions **no** toca esta MySQL.

### Arranque en la laptop

```powershell
.\scripts\local\start-local.ps1
```

| Contenedor | Puerto en el host | Quién lo usa |
|---|---|---|
| `aries_mysql_local` | **3307** → 3306 | Escritorio (in-process) y el API |
| `aries_api_local` | **5088** → 8080 | Salud, Swagger, feed `/updates/` |

Salud: http://localhost:5088/health  
Lista del feed: http://localhost:5088/updates  
Archivos Squirrel: http://localhost:5088/updates/RELEASES (404 hasta que publiques un feed)

Firewall/VPN: abrir **3307** y **5088** hacia las PCs cliente. No exponer esto a internet sin VPN.

Si ya tienes `aries_mysql_local` en 3307, el script **no** crea otro MySQL; solo construye y arranca el API.

### Publicar una versión al feed (no es el build)

El build (Actions o `msbuild`) solo deja `bin/Release`. El feed es **otro paso**: empaquetar con Squirrel y copiar el resultado a `publish/updates`.

Desde un tag `vX.Y.Z` (mismo número que `AssemblyFileVersion`), Actions sube el feed como artefacto `squirrel-feed` y lo adjunta al GitHub Release. En la laptop QA:

1. Baja el zip del Release (o el artefacto `squirrel-feed`) y descomprímelo.
2. Cópialo al bind mount:

```powershell
.\scripts\local\publish-updates.ps1 -SourceDir C:\ruta\al\directorio-con-RELEASES
```

Para empaquetar en esta máquina (tras un build Release y `nuget restore`):

```powershell
.\scripts\Pack-Squirrel.ps1
.\scripts\local\publish-updates.ps1 -SourceDir .\publish\squirrel
```

Eso escribe `publish/updates/` (bind mount). Las PCs con `UpdateServerString=http://<laptop>:5088/updates/` se enteran al **siguiente arranque** del exe. Override: variable `ARIES_UPDATE_URL`.

`bin/Release` **no** es un feed: tiene que existir el archivo `RELEASES`.

Comprobar el feed (sin S3, sin PCs reales):

```powershell
# 1) El archivo RELEASES es coherente con los nupkg (SHA1 + tamaño)
.\scripts\Verify-SquirrelFeed.ps1 -FeedDir .\publish\updates

# 2) El API sirve lo mismo que Squirrel pide (CI también: Aries.WebAPI.Tests)
dotnet test tests\Aries.WebAPI.Tests\Aries.WebAPI.Tests.csproj --filter SquirrelFeedTests --nologo
```

Si el API ya está arriba: `http://localhost:5088/updates` debe listar `RELEASES`; `http://localhost:5088/updates/RELEASES` no debe ser 404.

### Restaurar dump (copia — nunca RDS producción)

1. Tener `aries_mysql_local` arriba (`docker compose up -d mysql` o `start-local.ps1`).
2. Restaurar en este orden: `users` → `companies` → `accounts_names` → `accounts` → meses / asientos → `aries_routines.sql`.
3. Aplicar scripts de app: `scripts/mysql/fase1`, `fase3`, **`fase7`** (ancho de password) y, si el dump está limpio, `fase11` (inventario de duplicados **antes** del unique).
4. Ejemplo (host → contenedor):

```powershell
Get-Content .\dump.sql -Raw | docker exec -i aries_mysql_local mysql -ukenneth -p1234 aries
```

No hay job automático contra RDS. Un backup de RDS se baja **fuera** de este repo y se restaura solo a Docker.

### Escritorio en esta PC (dev)

Proyecto de inicio **Aries.Desktop** (`src/desktop/Aries.Desktop`, output `CapaPresentacion.exe`), configuración **Debug**. Login, maestros y asientos van **in-process** a MySQL `:3307`. `HttpBaseUrl` es solo diagnóstico.

Tras fase 7, el primer login de un usuario en plano deja la fila hasheada (`pbkdf2$...`).

### Escritorio en otras PCs (QA)

Copia `App.Production.config` → `App.Production.local.config` y rellena como en [`App.Qa.local.config.example`](../../src/desktop/Aries.Desktop/App.Qa.local.config.example): `Server=<laptop>`, puerto **3307**, `UpdateServerString=http://<laptop>:5088/updates/`.

## Production (máquina / pipeline S3)

`App.Production.config` **no** lleva password. Copia `App.Production.local.config.example` → `App.Production.local.config` (gitignored) o define `ARIES_MYSQL_CONNECTION`. JWT del API: `Jwt__Key` (mín. 32 bytes), nunca el placeholder.

**No aplicar SPs de fase 1/3/7/11 sobre RDS** desde este workspace. Eso es una ventana de operación aparte.

## Archivos

| Archivo | Rol |
|---|---|
| `.env.example` → `.env` | Docker (MySQL, puerto API, JWT) |
| `docker-compose.yml` | MySQL 8 + `Aries.WebAPI` + volumen `publish/updates` |
| `publish/updates/` | Feed Squirrel servido en `/updates/` |
| `src/desktop/Aries.Desktop/app.config` | Debug / Local (Docker `:3307`) |
| `src/desktop/Aries.Desktop/App.Production.config` | Release; updates aún S3 (prod histórica) |
| `src/desktop/Aries.Desktop/App.Qa.local.config.example` | Cómo apuntar Release a la laptop |
| `src/desktop/Aries.Desktop/App.Production.local.config` | Secretos de máquina (gitignored) |
| `src/hosts/Aries.WebAPI/appsettings.Development.json` | Fallback local |
| `src/hosts/Aries.WebAPI/appsettings.Production.json` | Vacío; secretos por env |
