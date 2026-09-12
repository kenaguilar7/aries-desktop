# Aplica src/desktop/Aries.Desktop/local-db.json sobre el exe.config de Debug.
# Plantilla: scripts/local/local-db.json.example.
# "use": "docker" aplica el bloque docker si existe; si no, deja app.config intacto.
param(
    [Parameter(Mandatory = $true)]
    [string]$JsonPath,
    [Parameter(Mandatory = $true)]
    [string]$ExeConfigPath
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $JsonPath)) {
    throw "No existe $JsonPath"
}
if (-not (Test-Path -LiteralPath $ExeConfigPath)) {
    throw "No existe $ExeConfigPath"
}

$json = Get-Content -LiteralPath $JsonPath -Raw -Encoding UTF8 | ConvertFrom-Json
$use = [string]$json.use
if ([string]::IsNullOrWhiteSpace($use)) {
    $use = 'docker'
}

$profile = $json.PSObject.Properties[$use]
$isDocker = $use.Trim().Equals('docker', [StringComparison]::OrdinalIgnoreCase)
if ($isDocker -and ($null -eq $profile -or $null -eq $profile.Value)) {
    Write-Host "local-db.json: use=docker (sin bloque) -> app.config (127.0.0.1:3307 / aries)"
    exit 0
}
if ($null -eq $profile -or $null -eq $profile.Value) {
    throw "local-db.json: no hay un bloque '$use'. Crea uno (como docker o aries-test)."
}

$p = $profile.Value
$server = [string]$p.Server
$port = [string]$p.Port
if ([string]::IsNullOrWhiteSpace($port)) { $port = '3306' }
$user = [string]$p.User
if ([string]::IsNullOrWhiteSpace($user)) { $user = [string]$p.UserId }
$password = [string]$p.Password
$database = [string]$p.Database
$envName = [string]$p.EnvironmentName
if ([string]::IsNullOrWhiteSpace($envName)) { $envName = $use }

$bad = @('SET_RDS_HOST', 'SET_PASSWORD', 'REPLACE_RDS', 'REPLACE_PASSWORD')
foreach ($needle in $bad) {
    if ($server.IndexOf($needle, [StringComparison]::OrdinalIgnoreCase) -ge 0 `
        -or $password.IndexOf($needle, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        throw "local-db.json: rellena Server y Password del bloque '$use' (plantilla: scripts/local/local-db.json.example)."
    }
}

if ([string]::IsNullOrWhiteSpace($server) -or [string]::IsNullOrWhiteSpace($password) -or [string]::IsNullOrWhiteSpace($database)) {
    throw "local-db.json bloque '$use': hacen falta Server, Password y Database."
}

$cs = "Server=$server;Port=$port;User id=$user;Password=$password;Database=$database;Allow User Variables=True"

[xml]$cfg = Get-Content -LiteralPath $ExeConfigPath -Raw
$csNodes = @($cfg.configuration.connectionStrings.add)
$found = $false
foreach ($add in $csNodes) {
    if ($add.name -eq 'DBconnectionString' -or $add.name -eq 'DBconnectionstring') {
        $add.SetAttribute('connectionString', $cs)
        $found = $true
    }
}
if (-not $found) {
    throw "El exe.config no tiene connectionString DBconnectionString."
}

function Set-AppSetting([xml]$doc, [string]$key, [string]$value) {
    $found = $false
    foreach ($add in @($doc.configuration.appSettings.add)) {
        if ($add.key -eq $key) {
            $add.SetAttribute('value', $value)
            $found = $true
        }
    }
    if (-not $found) {
        $node = $doc.CreateElement('add')
        $node.SetAttribute('key', $key)
        $node.SetAttribute('value', $value)
        [void]$doc.configuration.appSettings.AppendChild($node)
    }
}

Set-AppSetting $cfg 'EnvironmentName' $envName

$updateUrlProp = $p.PSObject.Properties['UpdateUrl']
if ($null -ne $updateUrlProp) {
    Set-AppSetting $cfg 'UpdateUrl' ([string]$updateUrlProp.Value)
}

$httpBaseProp = $p.PSObject.Properties['HttpBaseUrl']
if ($null -ne $httpBaseProp) {
    Set-AppSetting $cfg 'HttpBaseUrl' ([string]$httpBaseProp.Value)
}

$cfg.Save($ExeConfigPath)
$updateNote = if ($null -ne $updateUrlProp) { " UpdateUrl=$([string]$updateUrlProp.Value)" } else { '' }
Write-Host "local-db.json: use=$use -> Database=$database @ $server$updateNote"
exit 0
