$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$configPath = Join-Path $root "config\launcher.json"
if (-not (Test-Path -LiteralPath $configPath)) {
    throw "Missing config\launcher.json. Copy config\launcher.example.json and set hostAddress."
}

$config = Get-Content -LiteralPath $configPath -Raw -Encoding UTF8 | ConvertFrom-Json
if ([string]::IsNullOrWhiteSpace($config.hostAddress)) {
    throw "hostAddress is empty in config\launcher.json."
}

$stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$log = "logs\retroarch-netplay-join-$stamp.log"
$sessionConfig = "config\kof2002-session.generated.cfg"

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

Test-Connection -ComputerName $config.hostAddress -Count 3 -Quiet | Out-Null

$arguments = @(
    "-v",
    "--log-file=$log",
    "-L",
    $config.corePath,
    "--config",
    $config.retroArchConfigPath,
    "--appendconfig=$sessionConfig",
    "--check-frames=0",
    "--connect=$($config.hostAddress)",
    "--port=$($config.netplayPort)",
    "--nick=friend",
    $config.contentPath
)

Start-Process -FilePath (Join-Path $root $config.retroArchPath) -ArgumentList $arguments -WorkingDirectory $root
