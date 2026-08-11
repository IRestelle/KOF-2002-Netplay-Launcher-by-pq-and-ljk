# KOF 2002 Netplay Launcher

A small Windows launcher and helper scripts for two-player KOF 2002 netplay with RetroArch/FinalBurn Neo over Radmin VPN.

This repository contains only the launcher source code, helper scripts, example configuration, and documentation. It does not include ROM files, BIOS files, RetroArch binaries, FinalBurn Neo binaries, or any copyrighted game content.

## Features

- One-click host/join launcher for RetroArch netplay.
- Fixed content/core/config paths for a portable Windows game folder.
- ROM hash validation before launch.
- Radmin VPN-friendly host/join scripts.
- Joystick mapping helper for arcade sticks/gamepads.
- Low-latency and rescue netplay configuration examples.

## Contributors

- [IRestelle](https://github.com/IRestelle)
- [JKLee4049](https://github.com/JKLee4049)

## Repository Layout

```text
.
|-- src/
|   |-- Kof2002Netplay/          # WinForms launcher
|   `-- JoystickMapper/          # WinForms joystick binding tool
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

4. Set `hostAddress` to the host player's Radmin VPN IP, for example a `26.x.x.x` address.

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

Both players should join the same Radmin VPN network before starting RetroArch netplay.

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

## Build Requirements

- Windows 10/11
- .NET Framework compiler at `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`

The build script uses the built-in .NET Framework compiler and does not require Visual Studio.

## Legal Notice

This project is for a launcher and configuration helper only. It does not provide or license any game content. See `THIRD_PARTY_NOTICES.md` for third-party software notes.
