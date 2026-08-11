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

---

# KOF 2002 联机启动器

这是一套小型 Windows 启动器与辅助脚本，用于通过 Radmin VPN，配合 RetroArch/FinalBurn Neo 进行《KOF 2002》双人联机对战。

本仓库仅包含启动器源代码、辅助脚本、配置示例和文档，不包含 ROM 文件、BIOS 文件、RetroArch 二进制文件、FinalBurn Neo 二进制文件或任何受版权保护的游戏内容。

## 功能特点

- 一键创建或加入 RetroArch 联机对战。
- 面向 Radmin VPN 私有局域网的联机流程。
- 为便携式 Windows 游戏目录设置固定的游戏内容、核心和配置路径。
- 启动前验证 ROM 哈希值。
- 提供摇杆映射辅助工具，支持街机摇杆和游戏手柄。
- 提供低延迟、稳定和救急三种联机配置。

## 贡献者

- [IRestelle](https://github.com/IRestelle)
- [JKLee4049](https://github.com/JKLee4049)

## 仓库结构

```text
.
|-- src/
|   |-- Kof2002Netplay/          # WinForms 启动器
|   `-- JoystickMapper/          # WinForms 摇杆按键绑定工具
|-- config/                      # RetroArch 附加配置示例
|-- scripts/                     # 构建及联机辅助脚本
|-- docs/                        # 设置与测试说明
|-- THIRD_PARTY_NOTICES.md
|-- LICENSE
`-- README.md
```

## 不包含的内容

请勿向本仓库提交或通过本仓库重新分发以下文件：

- `rom/*.zip`
- `runtime/retroarch/**`
- 编译生成的 `*.exe` 文件
- 日志、存档、即时存档、截图、录像或本地生成的配置文件

每位玩家都必须自行提供通过合法途径获得的 ROM/BIOS 文件，并按照相应许可证安装或打包第三方运行时文件。

## 快速开始

1. 两位玩家都安装 Radmin VPN，并加入同一个 Radmin 网络。

2. 确认双方都能 ping 通对方：

```powershell
ping <对方的-radmin-ip>
```

3. 构建工具：

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
```

4. 将 `dist\bin` 中生成的文件复制到你的私人游戏目录中。

5. 根据 `config\launcher.example.json` 创建 `config\launcher.json`。

6. 在客户端电脑上，将 `hostAddress` 设置为主机玩家的 Radmin VPN IP，通常是一个 `26.x.x.x` 地址。

7. 将通过合法途径获得的 ROM/BIOS ZIP 文件放入你的私人 `rom` 文件夹。

8. 在其中一台电脑上启动主机：

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\host-netplay.ps1
```

9. 在另一台电脑上加入联机：

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\join-netplay.ps1
```

## Radmin VPN 注意事项

- 使用 Radmin 客户端中显示的 Radmin VPN IP，而不是公网 IP。
- RetroArch 的默认联机端口为 `55435`。
- 如果 Windows 防火墙提示是否允许 RetroArch，请允许其访问专用网络。
- 为获得更流畅的游戏体验，建议使用有线以太网或信号良好的 5 GHz/6 GHz Wi-Fi 连接。
- 对于休闲对战，稳定在 30–60 毫秒左右的延迟通常可以正常游玩。

## 延迟配置

开始联机前，请选择并运行以下脚本之一：

```text
scripts\set-profile-sensitive.cmd  # 适用于稳定的低延迟连接
scripts\set-profile-stable.cmd     # 适用于存在中等网络抖动的连接
scripts\set-profile-rescue.cmd     # 适用于非常不稳定的连接
```

## 构建要求

- Windows 10/11
- 位于 `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe` 的 .NET Framework 编译器

构建脚本使用 Windows 内置的 .NET Framework 编译器，无需安装 Visual Studio。

## 法律声明

本项目仅提供启动器和配置辅助工具，不提供任何游戏内容，也不授予任何游戏内容的使用许可。有关第三方软件的说明，请参阅 `THIRD_PARTY_NOTICES.md`。
