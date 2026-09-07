param(
    [switch]$DockerOnly,
    [switch]$SkipApi
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if (-not (Test-Path (Join-Path $root "docker-compose.yml"))) {
    $root = Resolve-Path (Join-Path $PSScriptRoot "..\..")
}

Set-Location $root

$envExample = Join-Path $root ".env.example"
$envFile = Join-Path $root ".env"
if (-not (Test-Path $envFile)) {
    Copy-Item $envExample $envFile
    Write-Host "Creado .env desde .env.example."
}

$apiPort = "5088"
if (Test-Path $envFile) {
    $portLine = Select-String -LiteralPath $envFile -Pattern '^\s*API_PORT\s*=\s*(.+)$' | Select-Object -First 1
    if ($portLine) { $apiPort = $portLine.Matches[0].Groups[1].Value.Trim() }
}

$mysqlOn3307 = docker ps --filter "publish=3307" --format "{{.Names}}" 2>$null
if ($mysqlOn3307) {
    Write-Host "MySQL ya esta en el puerto 3307 ($mysqlOn3307). No se levanta otro contenedor."
}
else {
    Write-Host "Levantando MySQL local (puerto 3307)..."
    docker compose up -d mysql
    if ($LASTEXITCODE -ne 0) {
        throw "docker compose up mysql fallo. Instala Docker Desktop y reintenta."
    }

    $ready = $false
    for ($i = 0; $i -lt 30; $i++) {
        docker exec aries_mysql_local mysqladmin ping -h127.0.0.1 -uroot -paries_root_pwd --silent 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) {
            $ready = $true
            break
        }
        Start-Sleep -Seconds 2
    }

    if (-not $ready) {
        Write-Warning "MySQL aun no responde a ping. Espera unos segundos y prueba: docker compose ps"
    }
    else {
        Write-Host "MySQL listo en 127.0.0.1:3307  base=aries  user=kenneth"
    }
}

# El dump se restaura como root; sin SYSTEM_USER el usuario de app no puede DROP PROCEDURE.
$grantSql = Join-Path $root "scripts\mysql\grant_routine_replace.sql"
if (Test-Path $grantSql) {
    Get-Content -Raw $grantSql | docker exec -i aries_mysql_local mysql -uroot -paries_root_pwd 2>$null
}

if ($SkipApi) {
    Write-Host "API omitida (-SkipApi). Escritorio Debug no la necesita (login in-process)."
    exit 0
}

Write-Host "Construyendo y levantando Aries.WebAPI en http://localhost:$apiPort/ ..."
docker compose up -d --build --no-deps api
if ($LASTEXITCODE -ne 0) {
    throw "docker compose up api fallo."
}

$healthUrl = "http://localhost:$apiPort/health"
$healthy = $false
for ($i = 0; $i -lt 40; $i++) {
    try {
        $resp = Invoke-WebRequest -Uri $healthUrl -UseBasicParsing -TimeoutSec 3
        if ($resp.StatusCode -eq 200) {
            $healthy = $true
            break
        }
    }
    catch {
        Start-Sleep -Seconds 2
    }
}

if (-not $healthy) {
    Write-Host "API no respondio /health. Ultimos logs:"
    docker logs --tail 80 aries_api_local
    throw "Aries.WebAPI no arranco en $healthUrl"
}

Write-Host "API lista: $healthUrl  Swagger: http://localhost:$apiPort/"

if ($DockerOnly) {
    exit 0
}

Write-Host ""
Write-Host "Siguiente:"
Write-Host "  Escritorio: F5 en Aries.Desktop (Debug). MySQL :3307, HttpBaseUrl http://localhost:$apiPort/"
Write-Host "  Login HTTP de prueba: kenneth / 96321 contra POST http://localhost:$apiPort/auth/login"
