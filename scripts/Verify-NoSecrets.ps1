# Fails if versioned configs still contain RDS host+password, old EBS, or Production JWT placeholder.
param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'
$failures = New-Object System.Collections.Generic.List[string]

$tracked = @(
    'src/desktop/Aries.Desktop/app.config',
    'src/desktop/Aries.Desktop/App.Production.config',
    'src/hosts/Aries.WebAPI/appsettings.json',
    'src/hosts/Aries.WebAPI/appsettings.Development.json',
    'src/hosts/Aries.WebAPI/appsettings.Production.json'
)

$banned = @(
    'ariescontrol.cn28u0mqcci2',
    '116390867',
    'eba-32ctm9kr',
    'AriesContador-prod-jwt-key-change-me'
)

foreach ($rel in $tracked) {
    $path = Join-Path $RepoRoot $rel
    if (-not (Test-Path -LiteralPath $path)) {
        $failures.Add("No existe $rel")
        continue
    }
    $text = Get-Content -LiteralPath $path -Raw
    foreach ($needle in $banned) {
        if ($text.IndexOf($needle, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
            $failures.Add("$rel contiene secreto o host prohibido: $needle")
        }
    }
}

$prodApi = Join-Path $RepoRoot 'src/hosts/Aries.WebAPI/appsettings.Production.json'
if (Test-Path -LiteralPath $prodApi) {
    $prod = Get-Content -LiteralPath $prodApi -Raw
    if ($prod -match 'Password=') {
        $failures.Add('appsettings.Production.json no debe llevar Password=')
    }
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Host "FAIL: $_" -ForegroundColor Red }
    exit 1
}

Write-Host 'Verify-NoSecrets OK'
exit 0
