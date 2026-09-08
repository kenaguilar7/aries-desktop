# Comprueba que esta maquina puede empaquetar y (opcional) hablar con S3 de test.
# No publica nada.
#
#   .\scripts\ops\Test-ReleasePrereqs.ps1
param(
    [string]$ProfileName = 'aries-staging'
)

$ErrorActionPreference = 'Continue'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$fail = 0

function Show-Ok([string]$msg) { Write-Host "OK   $msg" -ForegroundColor Green }
function Show-Warn([string]$msg) { Write-Host "WARN $msg" -ForegroundColor Yellow }
function Show-Fail([string]$msg) { Write-Host "FAIL $msg" -ForegroundColor Red; $script:fail++ }

$sln = Join-Path $root 'Aries.sln'
if (Test-Path -LiteralPath $sln) { Show-Ok "Solucion $sln" } else { Show-Fail "No esta Aries.sln" }

$infoPath = Join-Path $root 'src\desktop\Aries.Desktop\Properties\AssemblyInfo.cs'
if (Test-Path -LiteralPath $infoPath) {
    $raw = Get-Content -LiteralPath $infoPath -Raw
    if ($raw -match 'AssemblyFileVersion\("([^"]+)"\)') {
        Show-Ok "AssemblyFileVersion $($Matches[1])"
    }
    else {
        Show-Fail "AssemblyInfo sin AssemblyFileVersion"
    }
}
else {
    Show-Fail "Falta AssemblyInfo.cs"
}

$nuget = Get-Command nuget -ErrorAction SilentlyContinue
if ($nuget) { Show-Ok "nuget $($nuget.Source)" } else { Show-Warn "nuget.exe no esta en PATH (hace falta para Pack-Squirrel)" }

$squirrel = Join-Path $root 'packages\squirrel.windows.1.9.1\tools\Squirrel.exe'
if (Test-Path -LiteralPath $squirrel) { Show-Ok "Squirrel.exe" } else { Show-Warn 'Falta Squirrel.exe. Corre nuget restore Aries.sln' }

$aws = Get-Command aws -ErrorAction SilentlyContinue
if ($aws) {
    $awsVer = (aws --version 2>&1 | Out-String).Trim()
    Show-Ok "AWS CLI $awsVer"
    $ident = aws sts get-caller-identity --profile $ProfileName 2>&1
    if ($LASTEXITCODE -eq 0) {
        Show-Ok "Perfil $ProfileName autentica"
        aws s3 ls "s3://ariescontadorcr/updates-test/" --profile $ProfileName 2>&1 | Out-Host
        if ($LASTEXITCODE -eq 0) {
            Show-Ok "Listado s3://ariescontadorcr/updates-test/"
        }
        else {
            Show-Warn "El perfil autentica pero no lista updates-test/. Crea el prefijo en S3 o revisa IAM."
        }
    }
    else {
        Show-Warn "Perfil $ProfileName no autentica. Corre .\scripts\ops\New-AriesAwsProfile.ps1"
        Write-Host $ident
    }
}
else {
    Show-Warn "AWS CLI no instalado. El empaquetado local igual funciona; S3 espera."
}

$prodCfg = Join-Path $root 'src\desktop\Aries.Desktop\App.Production.config'
if (Test-Path -LiteralPath $prodCfg) {
    $rawCfg = Get-Content -LiteralPath $prodCfg -Raw
    if ($rawCfg -match 'ariescontadorcr\.s3\.') {
        Show-Ok "App.Production.config apunta al canal de produccion (el pack de test usa overlay updates-test)"
    }
    else {
        Show-Warn "App.Production.config no menciona el feed de produccion"
    }
}

$localCfg = Join-Path $root 'src\desktop\Aries.Desktop\App.Local.config'
$prodLocal = Join-Path $root 'src\desktop\Aries.Desktop\App.Production.local.config'
if (Test-Path -LiteralPath $localCfg) { Show-Ok "App.Local.config (no se commitea)" } else { Show-Warn "Opcional: App.Local.config para Debug local" }
if (Test-Path -LiteralPath $prodLocal) { Show-Ok "App.Production.local.config (secretos de release, no se commitea)" } else { Show-Warn "Opcional: App.Production.local.config con RDS para un exe Release usable" }

if ($fail -gt 0) {
    Write-Host "$fail problema(s) bloqueante(s)." -ForegroundColor Red
    exit 1
}
Write-Host "Test-ReleasePrereqs OK (los WARN no bloquean el empaquetado en GitHub Actions)"
exit 0
