# Manual Test Checklist

Use this checklist before publishing a release package.

## Local Startup

- Launcher opens on Windows 10/11.
- Host button starts RetroArch with `--host --port=55435`.
- Join button starts RetroArch with `--connect=<hostAddress> --port=55435`.
- Logs are written under `logs/`.
- A second launch stops the old RetroArch session first.

## Network

- Radmin VPN peer ping is stable.
- Tailscale peer route is direct when using Tailscale.
- Client joins as player 2.
- Connection remains established for at least 10 minutes.

## Controller

- Joystick is detected by Windows.
- RetroArch log reports the joypad driver and autoconfig entry.
- Directional input works.
- A/B/C/D map to light punch, light kick, heavy punch, heavy kick.
- Start and coin/select work.

## Legal/Packaging

- No ROM files are included.
- No third-party runtime binaries are included unless their license allows redistribution.
- No personal IP addresses are committed.
- No logs, saves, or generated session configs are committed.
