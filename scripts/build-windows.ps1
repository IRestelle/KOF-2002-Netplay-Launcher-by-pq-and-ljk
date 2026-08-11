$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$outDir = Join-Path $root "dist\bin"

if (-not (Test-Path -LiteralPath $csc)) {
    throw "Cannot find csc.exe at $csc. Install/enable .NET Framework build tools on Windows."
}

New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$launcherOut = Join-Path $outDir "Kof2002Netplay.exe"
$mapperOut = Join-Path $outDir "JoystickMapper.exe"
$proxyOut = Join-Path $outDir "tailscale.exe"

& $csc /nologo /target:winexe /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Web.Extensions.dll /out:$launcherOut (Join-Path $root "src\Kof2002Netplay\Program.cs")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& $csc /nologo /target:winexe /r:System.Windows.Forms.dll /r:System.Drawing.dll /out:$mapperOut (Join-Path $root "src\JoystickMapper\Program.cs")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& $csc /nologo /target:exe /out:$proxyOut (Join-Path $root "src\TailscaleStatusProxy\Program.cs")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Build complete: $outDir"
