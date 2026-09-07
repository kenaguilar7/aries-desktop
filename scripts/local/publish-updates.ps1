# Copia un feed Squirrel (RELEASES + nupkg) a publish/updates.
# El contenedor aries_api_local lo sirve en http://<host>:5088/updates/
# No corre tests ni sube a S3. El bind mount de docker-compose refleja el cambio al instante.
param(
    [Parameter(Mandatory = $true)]
    [string]$SourceDir
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if (-not (Test-Path (Join-Path $root "docker-compose.yml"))) {
    $root = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
}

$SourceDir = (Resolve-Path -LiteralPath $SourceDir).Path
$dest = Join-Path $root "publish\updates"
if (-not (Test-Path $dest)) {
    New-Item -ItemType Directory -Path $dest | Out-Null
}

$releases = Join-Path $SourceDir "RELEASES"
if (-not (Test-Path -LiteralPath $releases)) {
    throw "No hay archivo RELEASES en $SourceDir. Esto no es un feed Squirrel (bin/Release no sirve). Empaqueta con Squirrel --releasify y vuelve a correr este script."
}

Copy-Item -Path (Join-Path $SourceDir '*') -Destination $dest -Force

$verify = Join-Path $root "scripts\Verify-SquirrelFeed.ps1"
& $verify -FeedDir $dest
if ($LASTEXITCODE -ne 0) {
    throw "El feed copiado no es un RELEASES Squirrel valido."
}

Write-Host "Feed copiado a $dest"
Write-Host "Las PCs Release deben tener UpdateServerString = http://<esta-laptop>:5088/updates/"
Write-Host "Lista: http://localhost:5088/updates"
