# Un comando para pack + (opcional) subida a s3://ariescontadorcr/updates/.
# No corre solo: exige -ConfirmProduction. Preferible: Actions workflow cd-prod.
#
#   $env:ARIES_MYSQL_CONNECTION = '...'
#   $env:ARIES_UPDATE_URL = 'https://ariescontadorcr.s3.us-east-2.amazonaws.com/updates/'
#   .\scripts\ops\Publish-Production.ps1 -ConfirmProduction
#   .\scripts\ops\Publish-Production.ps1 -ConfirmProduction -PublishToS3
param(
    [switch]$ConfirmProduction,
    [switch]$PublishToS3,
    [switch]$IncludeUpgradeChain,
    [string]$BinDir,
    [string]$ConnectionStringsConfig
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

if (-not $ConfirmProduction) {
    throw "Produccion exige -ConfirmProduction. Sin eso este script no empaqueta hacia el canal de clientes."
}

$updateUrl = $env:ARIES_UPDATE_URL
if ([string]::IsNullOrWhiteSpace($updateUrl)) {
    throw 'Falta ARIES_UPDATE_URL (https://ariescontadorcr.s3.us-east-2.amazonaws.com/updates/ con slash final).'
}
if ($updateUrl -like '*updates-test*') {
    throw 'ARIES_UPDATE_URL no puede apuntar a updates-test.'
}
if ([string]::IsNullOrWhiteSpace($env:ARIES_MYSQL_CONNECTION) -and -not $ConnectionStringsConfig) {
    throw 'Falta ARIES_MYSQL_CONNECTION o -ConnectionStringsConfig.'
}

$pack = Join-Path $root 'scripts\Pack-Squirrel.ps1'
$packArgs = @{
    PackageId = 'AriesUpdater'
    ExpectedUpdateUrlContains = 'amazonaws.com/updates/'
    ForbiddenUpdateUrlContains = 'updates-test'
}
if ($BinDir) { $packArgs.BinDir = $BinDir }

if ($ConnectionStringsConfig) {
    $packArgs.ConnectionStringsConfig = $ConnectionStringsConfig
}
else {
    function Escape-Xml([string]$s) {
        return ($s -replace '&', '&amp;' -replace '<', '&lt;' -replace '>', '&gt;' -replace '"', '&quot;')
    }
    $overlay = Join-Path $env:TEMP ('Aries.Prod.pack.' + [guid]::NewGuid().ToString('n') + '.config')
    $db = Escape-Xml $env:ARIES_MYSQL_CONNECTION
    $url = Escape-Xml $updateUrl.Trim()
    $overlayXml = '<?xml version="1.0" encoding="utf-8"?><configuration><connectionStrings>' +
        "<add name=`"DBconnectionString`" connectionString=`"$db`" />" +
        '</connectionStrings><appSettings>' +
        "<add key=`"UpdateUrl`" value=`"$url`" />" +
        '<add key="HttpBaseUrl" value="" />' +
        '<add key="IsBeta" value="false" />' +
        '<add key="EnvironmentName" value="Production" />' +
        '</appSettings></configuration>'
    [System.IO.File]::WriteAllText($overlay, $overlayXml, (New-Object System.Text.UTF8Encoding $false))
    $packArgs.ConnectionStringsConfig = $overlay
}

if ($IncludeUpgradeChain) {
    $prev = Join-Path $root 'publish\squirrel-previous'
    & (Join-Path $PSScriptRoot 'Download-SquirrelFeed.ps1') -OutDir $prev
    $packArgs.PreviousFeedDir = $prev
}

& $pack @packArgs
if ($LASTEXITCODE -ne 0) {
    throw "Pack-Squirrel fallo (exit $LASTEXITCODE)"
}

if ($PublishToS3) {
    & (Join-Path $PSScriptRoot 'Publish-SquirrelFeed.ps1') -Environment Production -ConfirmProduction
    if ($LASTEXITCODE -ne 0) {
        throw "Publish-SquirrelFeed fallo (exit $LASTEXITCODE)"
    }
}

Write-Host "Publish-Production OK. Feed en publish\squirrel"
exit 0
