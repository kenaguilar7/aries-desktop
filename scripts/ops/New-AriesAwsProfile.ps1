# Crea o actualiza perfiles AWS CLI para Aries. Las claves quedan en
# %USERPROFILE%\.aws\credentials - nunca en el repo.
#
#   .\scripts\ops\New-AriesAwsProfile.ps1 -ProfileName aries-staging
#   .\scripts\ops\New-AriesAwsProfile.ps1 -ProfileName aries-prod
#
# RDS / MySQL no van aqui. Eso vive en App.Production.local.config (gitignored).
<#
.SYNOPSIS
    Guarda un perfil AWS nombrado para subir el feed Squirrel a S3.
#>
param(
    [ValidateSet('aries-staging', 'aries-prod')]
    [string]$ProfileName = 'aries-staging',
    [string]$Region = 'us-east-2',
    [string]$AccessKeyId,
    [string]$SecretAccessKey
)

$ErrorActionPreference = 'Stop'

$aws = Get-Command aws -ErrorAction SilentlyContinue
if (-not $aws) {
    throw "AWS CLI no esta instalado. Instala awscliv2 y vuelve a correr este script."
}

Write-Host "Perfil: $ProfileName  region: $Region"
Write-Host "Las claves se guardan en el AWS CLI local, no en git."

if (-not $AccessKeyId) {
    $AccessKeyId = Read-Host "AWS Access Key ID"
}
if (-not $SecretAccessKey) {
    $secure = Read-Host "AWS Secret Access Key" -AsSecureString
    $SecretAccessKey = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
    )
}

if ([string]::IsNullOrWhiteSpace($AccessKeyId) -or [string]::IsNullOrWhiteSpace($SecretAccessKey)) {
    throw "Access Key y Secret son obligatorios."
}

& aws configure set aws_access_key_id $AccessKeyId --profile $ProfileName
if ($LASTEXITCODE -ne 0) { throw "aws configure (access key) fallo" }
& aws configure set aws_secret_access_key $SecretAccessKey --profile $ProfileName
if ($LASTEXITCODE -ne 0) { throw "aws configure (secret) fallo" }
& aws configure set region $Region --profile $ProfileName
if ($LASTEXITCODE -ne 0) { throw "aws configure (region) fallo" }
& aws configure set output json --profile $ProfileName

$AccessKeyId = $null
$SecretAccessKey = $null

Write-Host "Probando sts get-caller-identity..."
& aws sts get-caller-identity --profile $ProfileName
if ($LASTEXITCODE -ne 0) {
    throw "El perfil $ProfileName no pudo autenticar. Revisa la clave IAM (necesita s3:ListBucket y s3:PutObject sobre ariescontadorcr)."
}

$prefix = if ($ProfileName -eq 'aries-prod') { 'updates' } else { 'updates-test' }
Write-Host "Listando s3://ariescontadorcr/$prefix (puede estar vacio la primera vez)..."
& aws s3 ls "s3://ariescontadorcr/$prefix/" --profile $ProfileName
if ($LASTEXITCODE -ne 0) {
    Write-Host "WARN: no se pudo listar el prefijo. El usuario IAM necesita s3:ListBucket en ariescontadorcr y s3:ListBucket/Get/Put en $prefix/*." -ForegroundColor Yellow
}

Write-Host "Perfil $ProfileName listo. Para usarlo: `$env:AWS_PROFILE='$ProfileName'"
exit 0
