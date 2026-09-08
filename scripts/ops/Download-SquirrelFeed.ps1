# Baja un feed Squirrel publico (HTTP) a una carpeta local.
# El bucket de clientes es publico; no hace falta perfil AWS para leerlo.
#
#   .\scripts\ops\Download-SquirrelFeed.ps1
#   .\scripts\ops\Download-SquirrelFeed.ps1 -FeedUrl https://ariescontadorcr.s3.us-east-2.amazonaws.com/updates-test/ -OutDir .\publish\squirrel-test-copy
<#
.SYNOPSIS
    Copia RELEASES + nupkgs de un feed HTTP para ensayar la cadena de update.
#>
param(
    [string]$FeedUrl = 'https://ariescontadorcr.s3.us-east-2.amazonaws.com/updates/',
    [string]$OutDir
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if (-not $OutDir) {
    $OutDir = Join-Path $root 'publish\squirrel-previous'
}

if (-not $FeedUrl.EndsWith('/')) {
    $FeedUrl += '/'
}

if (Test-Path -LiteralPath $OutDir) {
    Remove-Item -LiteralPath $OutDir -Recurse -Force
}
New-Item -ItemType Directory -Path $OutDir | Out-Null

$releasesUrl = $FeedUrl + 'RELEASES'
$releasesPath = Join-Path $OutDir 'RELEASES'
Write-Host "GET $releasesUrl"
Invoke-WebRequest -Uri $releasesUrl -OutFile $releasesPath -UseBasicParsing

$lines = Get-Content -LiteralPath $releasesPath | Where-Object { $_.Trim().Length -gt 0 }
if ($lines.Count -eq 0) {
    throw "RELEASES remoto esta vacio: $releasesUrl"
}

foreach ($line in $lines) {
    if ($line -notmatch '^(?i)[0-9a-f]{40}\s+(\S+)\s+\d+\s*$') {
        throw "RELEASES remoto mal formado: $line"
    }
    $name = $Matches[1]
    $dest = Join-Path $OutDir $name
    $url = $FeedUrl + $name
    Write-Host "GET $url"
    Invoke-WebRequest -Uri $url -OutFile $dest -UseBasicParsing
}

$verify = Join-Path $root 'scripts\Verify-SquirrelFeed.ps1'
& $verify -FeedDir $OutDir -ExpectedPackageId AriesUpdater
if ($LASTEXITCODE -ne 0) {
    throw "El feed descargado no es un AriesUpdater valido."
}

Write-Host "Download-SquirrelFeed OK -> $OutDir"
exit 0
