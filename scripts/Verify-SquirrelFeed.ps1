# Validates a Squirrel feed directory (RELEASES + nupkg). Used after publish-updates
# and as a local smoke check. Does not start Docker or talk to S3.
param(
    [Parameter(Mandatory = $true)]
    [string]$FeedDir,
    [string]$ExpectedPackageId
)

$ErrorActionPreference = 'Stop'
$failures = New-Object System.Collections.Generic.List[string]

if (-not (Test-Path -LiteralPath $FeedDir)) {
    throw "FeedDir no existe: $FeedDir"
}

$FeedDir = (Resolve-Path -LiteralPath $FeedDir).Path
$releasesPath = Join-Path $FeedDir 'RELEASES'
if (-not (Test-Path -LiteralPath $releasesPath)) {
    Write-Host "FAIL: falta RELEASES (esto no es un feed Squirrel; bin/Release no alcanza)." -ForegroundColor Red
    exit 1
}

$lines = Get-Content -LiteralPath $releasesPath | Where-Object { $_.Trim().Length -gt 0 }
if ($lines.Count -eq 0) {
    $failures.Add('RELEASES esta vacio')
}

$named = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::OrdinalIgnoreCase)
$i = 0
foreach ($line in $lines) {
    $i++
    if ($line -notmatch '^(?i)([0-9a-f]{40})\s+(\S+)\s+(\d+)\s*$') {
        $failures.Add("RELEASES linea ${i}: se esperaba 'sha1 archivo tamaño', llego: $line")
        continue
    }
    $sha = $Matches[1]
    $fileName = $Matches[2]
    $size = [long]$Matches[3]
    [void]$named.Add($fileName)

    if ($ExpectedPackageId -and ($fileName -notlike "$ExpectedPackageId*" -or $fileName -notlike '*.nupkg')) {
        $failures.Add("RELEASES menciona $fileName; se esperaba nupkg con id $ExpectedPackageId")
    }

    $pkg = Join-Path $FeedDir $fileName
    if (-not (Test-Path -LiteralPath $pkg)) {
        $failures.Add("RELEASES menciona $fileName pero no esta en el feed")
        continue
    }
    $actual = (Get-Item -LiteralPath $pkg).Length
    if ($actual -ne $size) {
        $failures.Add("$fileName tamaño RELEASES=$size disco=$actual")
    }
    $hash = (Get-FileHash -LiteralPath $pkg -Algorithm SHA1).Hash
    if (-not $hash.Equals($sha, [StringComparison]::OrdinalIgnoreCase)) {
        $failures.Add("$fileName SHA1 RELEASES=$sha disco=$hash")
    }
    else {
        Write-Host "OK  $fileName $actual bytes"
    }
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Host "FAIL: $_" -ForegroundColor Red }
    exit 1
}

$suffix = if ($ExpectedPackageId) { ", id=$ExpectedPackageId" } else { '' }
Write-Host "Verify-SquirrelFeed OK ($($named.Count) paquete(s)$suffix)"
exit 0
