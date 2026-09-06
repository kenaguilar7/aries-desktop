param(
    [switch]$DockerOnly
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

$localApiExample = Join-Path $root "Aries.WebAPI\appsettings.Local.json.example"
$localApi = Join-Path $root "Aries.WebAPI\appsettings.Local.json"
if (-not (Test-Path $localApi) -and (Test-Path $localApiExample)) {
    Copy-Item $localApiExample $localApi
    Write-Host "Creado Aries.WebAPI\appsettings.Local.json"
}

$existing = docker ps --filter "publish=3307" --format "{{.Names}}" 2>$null
if ($existing) {
    Write-Host "MySQL ya esta en el puerto 3307 ($existing). No se levanta otro contenedor."
    Write-Host "Conexion de las apps: 127.0.0.1:3307  user=kenneth  database=aries"
    if ($DockerOnly) { exit 0 }
}
else {
    Write-Host "Levantando MySQL local (puerto 3307)..."
    docker compose up -d
    if ($LASTEXITCODE -ne 0) {
        throw "docker compose up fallo. Instala Docker Desktop y reintenta."
    }

    $ready = $false
    for ($i = 0; $i -lt 30; $i++) {
        docker compose exec -T mysql mysqladmin ping -h127.0.0.1 -uroot -paries_root_pwd --silent 2>$null | Out-Null
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

if ($DockerOnly) {
    exit 0
}

Write-Host ""
Write-Host "Siguiente:"
Write-Host "  1. Si es la primera vez, restaura el dump en la base aries (scripts/local/README.md)."
Write-Host "  2. API:       dotnet run --project Aries.WebAPI"
Write-Host "  3. Escritorio: F5 en CapaPresentacion (Debug). app.config apunta a 3307."
