# KOF 2002 Netplay Launcher

A small Windows launcher and helper scripts for two-player KOF 2002 netplay with RetroArch/FinalBurn Neo over a private virtual LAN such as Radmin VPN or Tailscale.

This repository contains only the launcher source code, helper scripts, example configuration, and documentation. It does not include ROM files, BIOS files, RetroArch binaries, FinalBurn Neo binaries, or any copyrighted game content.

## Features

- One-click host/join launcher for RetroArch netplay.
- Fixed content/core/config paths for a portable Windows game folder.
- ROM hash validation before launch.
- Optional Tailscale status check for `100.x.x.x` peer addresses.
- Radmin VPN-friendly join scripts.
- Joystick mapping helper for arcade sticks/gamepads.
- Low-latency and rescue netplay configuration examples.

## Repository Layout

```text
.
|-- src/
|   |-- Kof2002Netplay/          # WinForms launcher
|   |-- JoystickMapper/          # WinForms joystick binding tool
|   `-- TailscaleStatusProxy/    # Compatibility wrapper for Tailscale status output
|-- config/                      # Example RetroArch append configs
|-- scripts/                     # Build and netplay helper scripts
|-- docs/                        # Setup and testing notes
|-- THIRD_PARTY_NOTICES.md
|-- LICENSE
`-- README.md
```

## What Is Not Included

Do not commit or redistribute these files in this repository:

- `rom/*.zip`
- `runtime/retroarch/**`
- compiled `*.exe` outputs
- logs, saves, states, screenshots, recordings, or local generated configs

Each player must provide their own legally obtained ROM/BIOS files and install or bundle third-party runtime files according to the relevant licenses.

## Quick Start

1. Build the tools:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
```

2. Copy the generated files from `dist\bin` into your private game folder.

3. Create `config\launcher.json` from `config\launcher.example.json`.

4. Set `hostAddress` to the host player's virtual LAN IP, for example a Radmin VPN `26.x.x.x` address or a Tailscale `100.x.x.x` address.

5. Put legally obtained ROM/BIOS ZIP files in your private `rom` folder.

6. Start the host on one PC:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\host-netplay.ps1
```

7. Join from the other PC:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\join-netplay.ps1
```

## Radmin VPN Setup

Radmin VPN often works better for classic LAN-style games when Tailscale falls back to high-latency DERP relay.

1. Both players join the same Radmin VPN network.
2. Confirm both sides can ping each other:

```powershell
ping <other-player-radmin-ip>
```

3. The host opens port `55435` by starting the launcher as host.
4. The client sets `hostAddress` to the host Radmin IP and joins.

## Latency Profiles

Use one of these scripts before starting netplay:

```text
scripts\set-profile-sensitive.cmd  # stable low-latency links
scripts\set-profile-stable.cmd     # moderate jitter
scripts\set-profile-rescue.cmd     # very unstable links
```

## Tailscale Setup

Tailscale is supported for private netplay addresses in the `100.x.x.x` range. If `tailscale ping <peer>` reports `direct`, latency should usually be playable. If it reports `via DERP` with high jitter, use Radmin VPN or improve the network path before tuning RetroArch.

## Build Requirements

- Windows 10/11
- .NET Framework compiler at `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`

The build script uses the built-in .NET Framework compiler and does not require Visual Studio.

## Legal Notice

This project is for a launcher and configuration helper only. It does not provide or license any game content. See `THIRD_PARTY_NOTICES.md` for third-party software notes.
