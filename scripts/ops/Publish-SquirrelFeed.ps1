# Sube un feed Squirrel ya verificado a S3.
# Por defecto SOLO test (updates-test). Staging y produccion se piden por nombre.
# Produccion exige -Environment Production -ConfirmProduction.
#
#   .\scripts\ops\Publish-SquirrelFeed.ps1
#   .\scripts\ops\Publish-SquirrelFeed.ps1 -Environment Test
#   .\scripts\ops\Publish-SquirrelFeed.ps1 -Environment Staging
#   .\scripts\ops\Publish-SquirrelFeed.ps1 -Environment Production -ConfirmProduction
#
# En GitHub Actions no pases -ProfileName: usa AWS_ACCESS_KEY_ID / AWS_SECRET_ACCESS_KEY.
<#
.SYNOPSIS
    Publica publish\squirrel a S3 usando perfil AWS o credenciales de entorno.
#>
param(
    [ValidateSet('Test', 'Staging', 'Production')]
    [string]$Environment = 'Test',
    [string]$FeedDir,
    [string]$Bucket = 'ariescontador',
    [string]$ProfileName,
    [switch]$ConfirmProduction
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

if (-not $FeedDir) {
    $FeedDir = Join-Path $root 'publish\squirrel'
}

switch ($Environment) {
    'Production' {
        if (-not $ConfirmProduction) {
            throw "Produccion exige -ConfirmProduction. Sin eso este script no toca s3://ariescontador/updates/."
        }
        $prefix = 'updates'
        if (-not $ProfileName -and -not $env:GITHUB_ACTIONS) { $ProfileName = 'aries-prod' }
    }
    'Staging' {
        $prefix = 'updates-staging'
        if (-not $ProfileName -and -not $env:GITHUB_ACTIONS) { $ProfileName = 'aries-staging' }
    }
    default {
        $prefix = 'updates-test'
        if (-not $ProfileName -and -not $env:GITHUB_ACTIONS) { $ProfileName = 'aries-staging' }
    }
}

$aws = Get-Command aws -ErrorAction SilentlyContinue
if (-not $aws) {
    throw "AWS CLI no esta instalado."
}

$verify = Join-Path $root 'scripts\Verify-SquirrelFeed.ps1'
& $verify -FeedDir $FeedDir -ExpectedPackageId AriesUpdater
if ($LASTEXITCODE -ne 0) {
    throw "El feed no es valido. No se sube nada a S3."
}

$dest = "s3://$Bucket/$prefix/"
Write-Host "destino=$dest"
if ($ProfileName) { Write-Host "Perfil=$ProfileName" } else { Write-Host "Credenciales: entorno / GitHub Actions" }
Write-Host "Archivos:"
Get-ChildItem -LiteralPath $FeedDir | ForEach-Object { Write-Host "  $($_.Name) $($_.Length) bytes" }

$awsArgs = @('s3', 'sync', $FeedDir, $dest, '--exact-timestamps')
if ($ProfileName) {
    $awsArgs += @('--profile', $ProfileName)
}
& aws @awsArgs
if ($LASTEXITCODE -ne 0) {
    throw "aws s3 sync fallo. Revisa credenciales e IAM sobre $dest"
}

Write-Host "Publish-SquirrelFeed OK -> $dest"
Write-Host "URL HTTP: https://s3.us-east-2.amazonaws.com/$Bucket/$prefix/"
exit 0
