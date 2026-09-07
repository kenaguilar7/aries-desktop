# Verifies the WinForms output that Squirrel would ship:
# binding redirects match DLL assembly versions, Oracle MySql.Data is absent,
# and HintPaths stay inside the repo.
param(
    [Parameter(Mandatory = $true)]
    [string]$OutputDir,
    [Parameter(Mandatory = $true)]
    [string]$AppConfig,
    [string]$ProjectFile,
    [switch]$CheckDns
)

$OutputDir = $OutputDir.Trim().TrimEnd('\', '/')
$ErrorActionPreference = 'Stop'
$failures = New-Object System.Collections.Generic.List[string]

function Add-Failure([string]$message) {
    $script:failures.Add($message)
    Write-Host "FAIL: $message" -ForegroundColor Red
}

if (-not (Test-Path -LiteralPath $OutputDir)) {
    throw "OutputDir no existe: $OutputDir"
}
if (-not (Test-Path -LiteralPath $AppConfig)) {
    throw "AppConfig no existe: $AppConfig"
}

$oracle = Join-Path $OutputDir 'MySql.Data.dll'
if (Test-Path -LiteralPath $oracle) {
    Add-Failure "El output contiene MySql.Data.dll (Oracle). El escritorio debe usar solo MySqlConnector."
}

$dlls = @{}
Get-ChildItem -LiteralPath $OutputDir -Filter '*.dll' | ForEach-Object {
    try {
        $name = [System.Reflection.AssemblyName]::GetAssemblyName($_.FullName)
        $dlls[$name.Name] = $name
    }
    catch {
        # native / mixed
    }
}

[xml]$cfg = Get-Content -LiteralPath $AppConfig -Raw
$ns = New-Object System.Xml.XmlNamespaceManager($cfg.NameTable)
$ns.AddNamespace('asm', 'urn:schemas-microsoft-com:asm.v1')
$ns.AddNamespace('b', 'http://schemas.microsoft.com/.NetConfiguration/v2.0')

$redirects = $cfg.SelectNodes('//asm:dependentAssembly', $ns)
if (-not $redirects -or $redirects.Count -eq 0) {
    $redirects = $cfg.configuration.runtime.assemblyBinding.dependentAssembly
}

foreach ($dep in @($redirects)) {
    $id = $dep.assemblyIdentity
    $redir = $dep.bindingRedirect
    if (-not $id -or -not $redir) { continue }
    $asmName = [string]$id.name
    $newVersion = [string]$redir.newVersion
    if ([string]::IsNullOrWhiteSpace($asmName) -or [string]::IsNullOrWhiteSpace($newVersion)) { continue }

    if ($asmName -eq 'MySql.Data') {
        Add-Failure "app.config todavía redirige MySql.Data a $newVersion. Quitar el redirect; el driver es MySqlConnector."
        continue
    }

    if (-not $dlls.ContainsKey($asmName)) {
        Add-Failure "Redirect $asmName -> $newVersion pero $asmName.dll no está en $OutputDir"
        continue
    }

    $actual = $dlls[$asmName].Version.ToString()
    if ($actual -ne $newVersion) {
        Add-Failure "Redirect $asmName newVersion=$newVersion pero el DLL es $actual"
    }
    else {
        Write-Host "OK  $asmName $actual"
    }
}

$csEntries = $cfg.configuration.connectionStrings.add
foreach ($add in @($csEntries)) {
    $n = [string]$add.name
    if ($n -notmatch '^(DBconnectionString|DBconnectionstring)$') { continue }
    $cs = [string]$add.connectionString
    $server = $null
    foreach ($part in $cs.Split(';')) {
        $eq = $part.IndexOf('=')
        if ($eq -lt 1) { continue }
        $key = $part.Substring(0, $eq).Trim()
        if ($key -eq 'Server') {
            $server = $part.Substring($eq + 1).Trim()
            break
        }
    }
    if ($cs -match 'ariescontrol\.cn28u0mqcci2' -or $cs -match '116390867') {
        Add-Failure "DBconnectionString versionado no puede llevar host RDS ni password real"
    }
    if ([string]::IsNullOrWhiteSpace($server)) {
        Add-Failure "DBconnectionString no tiene Server="
    }
    elseif ($server -eq 'SET_ON_MACHINE') {
        Write-Host "OK  Server placeholder SET_ON_MACHINE (secreto fuera de git)"
    }
    elseif ($CheckDns) {
        try {
            [void][System.Net.Dns]::GetHostAddresses($server)
            Write-Host "OK  DNS $server"
        }
        catch {
            Add-Failure "Server='$server' no resuelve DNS"
        }
    }
}

if ($ProjectFile) {
    if (-not (Test-Path -LiteralPath $ProjectFile)) {
        Add-Failure "ProjectFile no existe: $ProjectFile"
    }
    else {
        $projectFileFull = (Resolve-Path -LiteralPath $ProjectFile).Path
        $projectDir = Split-Path -Parent $projectFileFull
        # scripts/ lives at repo root; don't walk relative parents (pwsh 7 Join-Path rejects '').
        $repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
        $nugetConfig = Join-Path $repoRoot 'nuget.config'
        if (-not (Test-Path -LiteralPath $nugetConfig)) {
            Add-Failure "No se encontró nuget.config en $repoRoot (desde $projectDir)"
        }
        $hintMatches = Select-String -LiteralPath $ProjectFile -Pattern '<HintPath>([^<]+)</HintPath>' -AllMatches
        foreach ($m in $hintMatches) {
            $hint = $m.Matches[0].Groups[1].Value
            if ($hint -match 'Desktop|ClosedXML.Report-develop') {
                Add-Failure "HintPath fuera del repo: $hint"
                continue
            }
            $resolved = [System.IO.Path]::GetFullPath((Join-Path $projectDir $hint))
            if (-not (Test-Path -LiteralPath $resolved)) {
                Add-Failure "HintPath no existe: $hint"
                continue
            }
            $allowed = @(
                [System.IO.Path]::GetFullPath((Join-Path $repoRoot 'packages')),
                [System.IO.Path]::GetFullPath((Join-Path $repoRoot 'Recursos'))
            )
            $okRoot = $false
            foreach ($root in $allowed) {
                if ($resolved.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
                    $okRoot = $true
                    break
                }
            }
            if (-not $okRoot) {
                Add-Failure "HintPath no está en packages\ ni Recursos\: $hint"
            }
        }
    }
}

if ($failures.Count -gt 0) {
    Write-Host ""
    Write-Host "$($failures.Count) problema(s) de publicación del escritorio." -ForegroundColor Red
    exit 1
}

Write-Host "Verify-DesktopPublish OK"
exit 0
