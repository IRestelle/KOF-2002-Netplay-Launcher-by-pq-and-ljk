$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$configPath = Join-Path $root "config\launcher.json"
if (-not (Test-Path -LiteralPath $configPath)) {
    throw "Missing config\launcher.json. Copy config\launcher.example.json first."
}

$config = Get-Content -LiteralPath $configPath -Raw -Encoding UTF8 | ConvertFrom-Json
$stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$log = "logs\retroarch-netplay-host-$stamp.log"
$sessionConfig = "config\kof2002-session.generated.cfg"
$port = [string]$config.netplayPort

New-Item -ItemType Directory -Force -Path (Join-Path $root "logs") | Out-Null

$sessionParts = @()
foreach ($relativePath in @("config\kof2002-netplay-low-latency.cfg", "config\kof2002-joystick.cfg")) {
    $fullPath = Join-Path $root $relativePath
    if (Test-Path -LiteralPath $fullPath) {
        $sessionParts += "# Begin $relativePath"
        $sessionParts += Get-Content -LiteralPath $fullPath -Raw -Encoding UTF8
        $sessionParts += "# End $relativePath"
    }
}
if ($sessionParts.Count -gt 0) {
    Set-Content -LiteralPath (Join-Path $root $sessionConfig) -Value ($sessionParts -join [Environment]::NewLine) -Encoding UTF8
}

$retroArchPath = Join-Path $root $config.retroArchPath

try {
    netsh advfirewall firewall add rule name="KOF2002 RetroArch Netplay TCP $port" dir=in action=allow protocol=TCP localport=$port program="$retroArchPath" | Out-Null
    netsh advfirewall firewall add rule name="KOF2002 RetroArch Netplay UDP $port" dir=in action=allow protocol=UDP localport=$port program="$retroArchPath" | Out-Null
} catch {
    Write-Warning "Could not add firewall rules. Run this script as administrator or allow RetroArch when Windows prompts."
}

$arguments = @(
    "-v",
    "--log-file=$log",
    "-L",
    $config.corePath,
    "--config",
    $config.retroArchConfigPath,
    "--appendconfig=$sessionConfig",
    "--check-frames=0",
    "--host",
    "--port=$port",
    "--nick=host",
    $config.contentPath
)

Start-Process -FilePath $retroArchPath -ArgumentList $arguments -WorkingDirectory $root
