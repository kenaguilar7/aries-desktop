# Empaqueta bin/Release del escritorio como feed Squirrel (nupkg full + RELEASES + Setup.exe).
# Exe FIJO: CapaPresentacion.exe. El id por defecto es CapaPresentacion (CD actual).
# Para el canal de clientes usa -PackageId AriesUpdater.
#
#   .\scripts\Pack-Squirrel.ps1
#   .\scripts\Pack-Squirrel.ps1 -PackageId AriesUpdater -ExpectedUpdateUrlContains updates-test
#   .\scripts\Pack-Squirrel.ps1 -PreviousFeedDir .\publish\squirrel-previous
param(
    [string]$BinDir,
    [string]$OutputDir,
    [string]$AssemblyInfo,
    [string]$ExpectedVersion,
    [string]$SquirrelExe,
    [string]$NuGetExe,
    [string]$PreviousFeedDir,
    [string]$ConnectionStringsConfig,
    [string]$ExpectedUpdateUrlContains,
    [string]$PackageId = 'CapaPresentacion'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
if (-not (Test-Path (Join-Path $root 'Aries.sln'))) {
    $root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
}

if (-not $BinDir) {
    $BinDir = Join-Path $root 'src\desktop\Aries.Desktop\bin\Release'
}
if (-not $OutputDir) {
    $OutputDir = Join-Path $root 'publish\squirrel'
}
if (-not $AssemblyInfo) {
    $AssemblyInfo = Join-Path $root 'src\desktop\Aries.Desktop\Properties\AssemblyInfo.cs'
}

if (-not (Test-Path -LiteralPath $BinDir)) {
    throw "BinDir no existe: $BinDir. Compila Release antes (msbuild Aries.sln /p:Configuration=Release)."
}
if (-not (Test-Path -LiteralPath $AssemblyInfo)) {
    throw "No se encontro AssemblyInfo: $AssemblyInfo"
}

$exe = Join-Path $BinDir 'CapaPresentacion.exe'
if (-not (Test-Path -LiteralPath $exe)) {
    throw "Falta CapaPresentacion.exe en $BinDir"
}

$info = Get-Content -LiteralPath $AssemblyInfo -Raw
if ($info -notmatch 'AssemblyFileVersion\("([^"]+)"\)') {
    throw "No hay AssemblyFileVersion en $AssemblyInfo"
}
$version = $Matches[1]
if ($ExpectedVersion -and $ExpectedVersion -ne $version) {
    throw "AssemblyFileVersion $version no coincide con la version esperada $ExpectedVersion"
}

if (-not $NuGetExe) {
    $nugetCmd = Get-Command nuget -ErrorAction SilentlyContinue
    if (-not $nugetCmd) {
        throw "nuget.exe no esta en PATH. Instala nuget o pasa -NuGetExe."
    }
    $NuGetExe = $nugetCmd.Source
}
if (-not $SquirrelExe) {
    $SquirrelExe = Join-Path $root 'packages\squirrel.windows.1.9.1\tools\Squirrel.exe'
}
if (-not (Test-Path -LiteralPath $SquirrelExe)) {
    throw "Falta Squirrel.exe ($SquirrelExe). Corre nuget restore Aries.sln."
}

$BinDir = (Resolve-Path -LiteralPath $BinDir).Path
$work = Join-Path $root 'publish\squirrel-pack'
if (Test-Path -LiteralPath $work) {
    Remove-Item -LiteralPath $work -Recurse -Force
}
New-Item -ItemType Directory -Path $work | Out-Null
if (Test-Path -LiteralPath $OutputDir) {
    Remove-Item -LiteralPath $OutputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputDir | Out-Null

if ($PreviousFeedDir) {
    if (-not (Test-Path -LiteralPath $PreviousFeedDir)) {
        throw "PreviousFeedDir no existe: $PreviousFeedDir"
    }
    Write-Host "Copiando feed anterior desde $PreviousFeedDir (cadena de update -> $version)"
    Copy-Item -Path (Join-Path $PreviousFeedDir '*') -Destination $OutputDir -Force
}

if ($ConnectionStringsConfig) {
    if (-not (Test-Path -LiteralPath $ConnectionStringsConfig)) {
        throw "ConnectionStringsConfig no existe: $ConnectionStringsConfig"
    }
    $exeConfig = Join-Path $BinDir 'CapaPresentacion.exe.config'
    if (-not (Test-Path -LiteralPath $exeConfig)) {
        throw "Falta $exeConfig; no se pueden fusionar connectionStrings."
    }
    Write-Host "Fusionando connectionStrings desde $ConnectionStringsConfig (se conservan bindingRedirects del build)"
    [xml]$built = Get-Content -LiteralPath $exeConfig -Raw
    [xml]$overlay = Get-Content -LiteralPath $ConnectionStringsConfig -Raw
    $overlayAdds = @($overlay.configuration.connectionStrings.add)
    if ($overlayAdds.Count -eq 0) {
        throw "$ConnectionStringsConfig no tiene connectionStrings/add"
    }
    $builtCs = $built.configuration.connectionStrings
    if (-not $builtCs) {
        throw "$exeConfig no tiene connectionStrings"
    }
    foreach ($src in $overlayAdds) {
        $name = [string]$src.name
        $value = [string]$src.connectionString
        $existing = @($builtCs.add) | Where-Object { $_.name -eq $name } | Select-Object -First 1
        if ($existing) {
            $existing.connectionString = $value
        }
        else {
            $node = $built.ImportNode($src, $true)
            [void]$builtCs.AppendChild($node)
        }
    }
    $built.Save($exeConfig)
}

if ($ExpectedUpdateUrlContains) {
    $exeConfig = Join-Path $BinDir 'CapaPresentacion.exe.config'
    if (-not (Test-Path -LiteralPath $exeConfig)) {
        throw "Falta $exeConfig; no se puede validar UpdateServerString."
    }
    [xml]$packedCfg = Get-Content -LiteralPath $exeConfig -Raw
    $updateNode = @($packedCfg.configuration.connectionStrings.add) |
        Where-Object { [string]$_.name -eq 'UpdateServerString' } |
        Select-Object -First 1
    $updateUrl = if ($updateNode) { [string]$updateNode.connectionString } else { '' }
    if ([string]::IsNullOrWhiteSpace($updateUrl) -or ($updateUrl -notlike "*$ExpectedUpdateUrlContains*")) {
        throw "UpdateServerString='$updateUrl' no contiene '$ExpectedUpdateUrlContains'. Abortando pack para no enviar clientes al canal equivocado."
    }
    Write-Host "UpdateServerString OK: $updateUrl"
}

$nuspecPath = Join-Path $work "$PackageId.nuspec"
$nuspec = @"
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <id>$PackageId</id>
    <version>$version</version>
    <authors>Sistemas Aries</authors>
    <description>Aries Contador</description>
  </metadata>
  <files>
    <file src="**" target="lib\net45" exclude="**\*.xml;**\*.pdb;**\*.log;**\roslyn\**" />
  </files>
</package>
"@
[System.IO.File]::WriteAllText($nuspecPath, $nuspec, (New-Object System.Text.UTF8Encoding $false))

Write-Host "nuget pack $PackageId $version"
& $NuGetExe pack $nuspecPath -BasePath $BinDir -OutputDirectory $work -NoPackageAnalysis -NonInteractive
if ($LASTEXITCODE -ne 0) {
    throw "nuget pack fallo (exit $LASTEXITCODE)"
}

$nupkg = Get-ChildItem -LiteralPath $work -Filter "$PackageId.*.nupkg" | Select-Object -First 1
if (-not $nupkg) {
    throw "nuget pack no produjo $PackageId.*.nupkg"
}

Write-Host "Squirrel --releasify $($nupkg.Name) -> $OutputDir"
# Squirrel.exe es una app Win32 (no consola): `& Squirrel.exe` vuelve antes de terminar.
# Hay que esperar el proceso; si no, OutputDir queda sin Setup.exe/RELEASES.
$squirrelArgs = @(
    "--releasify=$($nupkg.FullName)",
    "--releaseDir=$OutputDir",
    "--no-msi"
)
$proc = Start-Process -FilePath $SquirrelExe -ArgumentList $squirrelArgs -Wait -PassThru -NoNewWindow
if ($null -eq $proc) {
    throw "No se pudo iniciar Squirrel.exe"
}
if ($proc.ExitCode -ne 0 -and $null -ne $proc.ExitCode) {
    throw "Squirrel --releasify fallo (exit $($proc.ExitCode))"
}

$candidateDirs = @(
    $OutputDir,
    (Join-Path (Get-Location) 'Releases'),
    (Join-Path (Split-Path -Parent $SquirrelExe) 'Releases')
)
foreach ($dir in $candidateDirs) {
    if (-not (Test-Path -LiteralPath $dir)) { continue }
    if ([string]::Equals((Resolve-Path -LiteralPath $dir).Path, (Resolve-Path -LiteralPath $OutputDir).Path, [StringComparison]::OrdinalIgnoreCase)) {
        continue
    }
    $hasFeed = (Test-Path -LiteralPath (Join-Path $dir 'RELEASES')) -or (Test-Path -LiteralPath (Join-Path $dir 'Setup.exe'))
    if (-not $hasFeed) { continue }
    Write-Host "Copiando feed Squirrel desde $dir"
    Copy-Item -Path (Join-Path $dir '*') -Destination $OutputDir -Force
}

$deadline = (Get-Date).AddSeconds(45)
while (-not (Test-Path -LiteralPath (Join-Path $OutputDir 'Setup.exe')) -and (Get-Date) -lt $deadline) {
    Start-Sleep -Milliseconds 500
    foreach ($dir in $candidateDirs) {
        if (-not (Test-Path -LiteralPath (Join-Path $dir 'Setup.exe'))) { continue }
        Copy-Item -Path (Join-Path $dir '*') -Destination $OutputDir -Force
    }
}

$setup = Join-Path $OutputDir 'Setup.exe'
if (-not (Test-Path -LiteralPath $setup)) {
    Write-Host "Contenido de carpetas Squirrel:"
    foreach ($dir in $candidateDirs) {
        Write-Host "--- $dir ---"
        if (Test-Path -LiteralPath $dir) {
            Get-ChildItem -LiteralPath $dir | ForEach-Object { Write-Host ("  {0} {1} bytes" -f $_.Name, $_.Length) }
        }
        else {
            Write-Host "  (no existe)"
        }
    }
    $squirrelLog = Join-Path (Split-Path -Parent $SquirrelExe) 'Squirrel-Releasify.log'
    if (Test-Path -LiteralPath $squirrelLog) {
        Write-Host "--- $squirrelLog ---"
        Get-Content -LiteralPath $squirrelLog -Tail 40
    }
    throw "Squirrel no dejo Setup.exe en $OutputDir"
}

$verify = Join-Path $root 'scripts\Verify-SquirrelFeed.ps1'
& $verify -FeedDir $OutputDir -ExpectedPackageId $PackageId
if ($LASTEXITCODE -ne 0) {
    throw "Verify-SquirrelFeed fallo sobre $OutputDir"
}

if ($env:GITHUB_OUTPUT) {
    Add-Content -Path $env:GITHUB_OUTPUT -Value "version=$version"
}

Write-Host "Pack-Squirrel OK version=$version id=$PackageId feed=$OutputDir"
exit 0
