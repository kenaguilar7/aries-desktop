# Aplica src/desktop/Aries.Desktop/local-db.json sobre el exe.config de Debug.
# Plantilla: scripts/local/local-db.json.example. "use": "docker" deja app.config intacto.
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
if ([string]::IsNullOrWhiteSpace($use) -or $use.Trim().Equals('docker', [StringComparison]::OrdinalIgnoreCase)) {
    Write-Host "local-db.json: use=docker -> app.config (127.0.0.1:3307 / aries)"
    exit 0
}

$profile = $json.PSObject.Properties[$use]
if ($null -eq $profile -or $null -eq $profile.Value) {
    throw "local-db.json: no hay un bloque '$use'. Crea uno (como aries-test) o pon `"use`": `"docker`"."
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

$envFound = $false
foreach ($add in @($cfg.configuration.appSettings.add)) {
    if ($add.key -eq 'EnvironmentName') {
        $add.SetAttribute('value', $envName)
        $envFound = $true
    }
}
if (-not $envFound) {
    $node = $cfg.CreateElement('add')
    $node.SetAttribute('key', 'EnvironmentName')
    $node.SetAttribute('value', $envName)
    [void]$cfg.configuration.appSettings.AppendChild($node)
}

$cfg.Save($ExeConfigPath)
Write-Host "local-db.json: use=$use -> Database=$database @ $server"
exit 0
