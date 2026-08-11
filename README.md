# KOF 2002 Netplay Launcher

A small Windows launcher and helper script set for two-player KOF 2002 netplay with RetroArch/FinalBurn Neo over Radmin VPN.

This repository contains only the launcher source code, helper scripts, example configuration, and documentation. It does not include ROM files, BIOS files, RetroArch binaries, FinalBurn Neo binaries, or any copyrighted game content.

## Features

- One-click host/join launcher for RetroArch netplay.
- Radmin VPN-oriented private LAN workflow.
- Fixed content/core/config paths for a portable Windows game folder.
- ROM hash validation before launch.
- Joystick mapping helper for arcade sticks/gamepads.
- Low-latency, stable, and rescue netplay profiles.

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

1. Both players install Radmin VPN and join the same Radmin network.

2. Confirm both sides can ping each other:

```powershell
ping <other-player-radmin-ip>
```

3. Build the tools:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
```

4. Copy the generated files from `dist\bin` into your private game folder.

5. Create `config\launcher.json` from `config\launcher.example.json`.

6. On the client PC, set `hostAddress` to the host player's Radmin VPN IP, usually a `26.x.x.x` address.

7. Put legally obtained ROM/BIOS ZIP files in your private `rom` folder.

8. Start the host on one PC:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\host-netplay.ps1
```

9. Join from the other PC:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\join-netplay.ps1
```

## Radmin VPN Notes

- Use the Radmin VPN IP shown in the Radmin client, not a public IP.
- The default RetroArch netplay port is `55435`.
- If Windows Firewall prompts for RetroArch, allow private network access.
- For smoother play, prefer wired Ethernet or a strong 5 GHz/6 GHz Wi-Fi connection.
- A stable ping around 30-60 ms is usually playable for casual matches.

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
