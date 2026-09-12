# Ambientes Local y QA (laptop Docker)

Hay **tres** sitios: tu PC de desarrollo, la **laptop/servidor** con Docker (LAN o VPN), y S3 de producción (aún no lo usa este compose).

| | Escritorio | MySQL + API + Squirrel |
|---|---|---|
| **Local (esta PC)** | **Debug** → `app.config` (Docker `:3307` / `aries`). Otra base: `local-db.json`. | `docker compose` en esta misma máquina |
| **QA (laptop servidor)** | **Release** → copia `App.Production.config` a `App.Production.local.config` (gitignored): `Server=<laptop>`, puerto **3307**, `UpdateUrl=http://<laptop>:5088/updates/`, `EnvironmentName=Qa`. | El mismo `docker compose` en la laptop. Clientes por VPN/LAN. |
| **Production (S3)** | `App.Production.config` + pack con secrets. Feed: bucket `ariescontadorcr/updates` | No es este Docker |

El título del menú muestra `[Local]`, `[Qa]` o `[Production]` según `EnvironmentName`.

## Un solo Docker: BD, API y actualizaciones

Squirrel no necesita S3. Pregunta por HTTP a `UpdateUrl` (appSettings; el nombre viejo `UpdateServerString` en connectionStrings sigue leyéndose). En QA esa URL es el **mismo** contenedor del API:

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

Desde un tag `vX.Y.Z` (mismo número que [`version.props`](../../version.props) y `AssemblyFileVersion`), el workflow **cd-prod** empaqueta el canal de clientes. **cd-test** sube el artefacto `squirrel-feed-test` (y opcionalmente S3 `updates-test`). En la laptop QA:

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

Eso escribe `publish/updates/` (bind mount). Las PCs con `UpdateUrl=http://<laptop>:5088/updates/` se enteran al **siguiente arranque** del exe (splash: busca updates, pide reinicio si hay paquete nuevo). Override: variable `ARIES_UPDATE_URL`.

Log de arranque: `%LocalAppData%\AriesContador\startup.log` (ambiente, migraciones, Squirrel).

**Debug y RDS:** F5 no aplica migraciones contra un host que no sea `localhost`/`127.0.0.1`. Para forzar (aries-test): `$env:ARIES_APPLY_MIGRATIONS=1`.

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

Proyecto de inicio **Aries.Desktop** (`src/desktop/Aries.Desktop`, output `CapaPresentacion.exe`), configuración **Debug**. Login, maestros y asientos van **in-process** a MySQL. `HttpBaseUrl` es solo diagnóstico.

**Base de datos en F5:** el primer Debug crea `src/desktop/Aries.Desktop/local-db.json` (gitignored) desde [`local-db.json.example`](local-db.json.example). Cambia solo `"use"`:

| `"use"` | Destino |
|---|---|
| `"docker"` | `127.0.0.1:3307`, base `aries` (bloque `docker` o `app.config`) |
| `"aries-test"` | el RDS que pongas en ese bloque, base `aries-test` |

Rellena `Server` y `Password` una vez. No uses variables de entorno en local. El título del menú muestra la base (`[Local] aries` o `[AriesTest] aries-test`).

Tras fase 7, el primer login de un usuario en plano deja la fila hasheada (`pbkdf2$...`).

### Escritorio en otras PCs (QA)

Copia `App.Production.config` → `App.Production.local.config` (junto al csproj, gitignored). En esta laptop: `Server=127.0.0.1;Port=3307;...` y `UpdateUrl=http://127.0.0.1:5088/updates/`. En otras PCs: `Server=<laptop>` y `UpdateUrl=http://<laptop>:5088/updates/`. `EnvironmentName=Qa`. No uses el bucket S3 de producción.

## Production (máquina / pipeline S3)

`App.Production.config` **no** lleva password. En la máquina de release: copia ese archivo a `App.Production.local.config` (gitignored) y rellena `DBconnectionString`, o define `ARIES_MYSQL_CONNECTION` al empaquetar. JWT del API: `Jwt__Key` (mín. 32 bytes), nunca el placeholder.

**No aplicar SPs de fase 1/3/7/11 sobre RDS** desde este workspace. Eso es una ventana de operación aparte.

## Archivos

| Archivo | Rol |
|---|---|
| `.env.example` → `.env` | Docker (MySQL, puerto API, JWT) |
| `docker-compose.yml` | MySQL 8 + `Aries.WebAPI` + volumen `publish/updates` |
| `publish/updates/` | Feed Squirrel servido en `/updates/` |
| `src/desktop/Aries.Desktop/app.config` | Debug / Docker (`:3307`, base `aries`). Solo `DBconnectionString` en connectionStrings; `UpdateUrl` / `HttpBaseUrl` / `IsBeta` en appSettings |
| `src/desktop/Aries.Desktop/App.Production.config` | Release (sin secretos). `UpdateUrl` = S3 `updates/` |
| `src/desktop/Aries.Desktop/local-db.json` | F5: `"use": "docker"` o `"aries-test"` (gitignored) |
| `scripts/local/local-db.json.example` | Plantilla de `local-db.json` |
| `src/desktop/Aries.Desktop/App.Production.local.config` | Secretos de un Release local (gitignored) |
| `src/hosts/Aries.WebAPI/appsettings.Development.json` | Fallback local |
| `src/hosts/Aries.WebAPI/appsettings.Production.json` | Vacío; secretos por env |
