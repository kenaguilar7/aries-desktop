# Empaqueta bin/Release del escritorio como feed Squirrel (nupkg full + RELEASES + Setup.exe).
# No necesita un RELEASES anterior: el primer --releasify basta para artefactos/QA.
param(
    [string]$BinDir,
    [string]$OutputDir,
    [string]$AssemblyInfo,
    [string]$ExpectedVersion,
    [string]$SquirrelExe,
    [string]$NuGetExe
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

$nuspecPath = Join-Path $work 'CapaPresentacion.nuspec'
$nuspec = @"
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <id>CapaPresentacion</id>
    <version>$version</version>
    <authors>Sistemas Aries</authors>
    <description>Aries Contador</description>
  </metadata>
  <files>
    <file src="**" target="lib\net45" exclude="**\*.xml;**\*.pdb;**\*.log" />
  </files>
</package>
"@
[System.IO.File]::WriteAllText($nuspecPath, $nuspec, (New-Object System.Text.UTF8Encoding $false))

Write-Host "nuget pack CapaPresentacion $version"
& $NuGetExe pack $nuspecPath -BasePath $BinDir -OutputDirectory $work -NoPackageAnalysis -NonInteractive
if ($LASTEXITCODE -ne 0) {
    throw "nuget pack fallo (exit $LASTEXITCODE)"
}

$nupkg = Get-ChildItem -LiteralPath $work -Filter 'CapaPresentacion.*.nupkg' | Select-Object -First 1
if (-not $nupkg) {
    throw "nuget pack no produjo CapaPresentacion.*.nupkg"
}

Write-Host "Squirrel --releasify $($nupkg.Name) -> $OutputDir"
& $SquirrelExe --releasify $nupkg.FullName --releaseDir=$OutputDir --no-msi
if ($LASTEXITCODE -ne 0) {
    throw "Squirrel --releasify fallo (exit $LASTEXITCODE)"
}

$releasesFile = Join-Path $OutputDir 'RELEASES'
if (-not (Test-Path -LiteralPath $releasesFile)) {
    $fallback = Join-Path (Get-Location) 'Releases'
    if (Test-Path -LiteralPath (Join-Path $fallback 'RELEASES')) {
        Copy-Item -Path (Join-Path $fallback '*') -Destination $OutputDir -Force
    }
}

$setup = Join-Path $OutputDir 'Setup.exe'
if (-not (Test-Path -LiteralPath $setup)) {
    throw "Squirrel no dejo Setup.exe en $OutputDir"
}

$verify = Join-Path $root 'scripts\Verify-SquirrelFeed.ps1'
& $verify -FeedDir $OutputDir
if ($LASTEXITCODE -ne 0) {
    throw "Verify-SquirrelFeed fallo sobre $OutputDir"
}

if ($env:GITHUB_OUTPUT) {
    Add-Content -Path $env:GITHUB_OUTPUT -Value "version=$version"
}

Write-Host "Pack-Squirrel OK version=$version feed=$OutputDir"
exit 0
