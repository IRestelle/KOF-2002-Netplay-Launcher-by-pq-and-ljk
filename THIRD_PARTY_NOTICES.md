# Third-Party Notices

This private, non-commercial launcher uses or redistributes the following
third-party software. ROM files are not third-party dependencies of this
repository and are never redistributed by the build scripts.

## RetroArch 1.22.2

- Binary source: https://buildbot.libretro.com/stable/1.22.2/windows/x86_64/RetroArch.7z
- Source code: https://github.com/libretro/RetroArch/tree/v1.22.2
- License: GNU General Public License v3.0
- Bundled license: runtime/retroarch/licenses/RetroArch-COPYING.txt

## FinalBurn Neo libretro core

- Binary source: https://buildbot.libretro.com/stable/1.22.2/windows/x86_64/RetroArch_cores.7z
- Source snapshot: https://github.com/finalburnneo/FBNeo/tree/b9833b57790093fd2e9c8dccc321375a6537fe3a
- License: FBNeo project license, including its non-commercial-use conditions
- Bundled license: runtime/retroarch/licenses/FBNeo-LICENSE.md

The buildbot core archive was last modified 2025-11-20 02:52 UTC. The pinned
FBNeo source commit above predates that archive and supplies the complete
license text. The buildbot binary does not embed a verifiable FBNeo commit ID,
so this document does not claim an exact binary-to-commit correspondence.

## .NET SDK 8.0.423

- Binary source: https://builds.dotnet.microsoft.com/dotnet/Sdk/8.0.423/dotnet-sdk-8.0.423-win-x64.zip
- Source code: https://github.com/dotnet
- License: MIT and component-specific notices in the SDK
- Used only as a project-local build tool. The release bundles the SDK notices
  actually used by the build at `notices/dotnet-sdk-LICENSE.txt` and
  `notices/dotnet-sdk-ThirdPartyNotices.txt`, plus WindowsDesktop SDK notices
  at `notices/windowsdesktop-sdk-LICENSE.TXT` and
  `notices/windowsdesktop-sdk-THIRD-PARTY-NOTICES.TXT`.

## MinGit 2.55.0.3

- Binary source: https://github.com/git-for-windows/git/releases/tag/v2.55.0.windows.3
- Source code: https://github.com/git-for-windows/git
- License: GNU General Public License v2.0 and bundled component licenses
- Used only as a project-local version-control tool.

## Tailscale

- Website: https://tailscale.com/
- Source code: https://github.com/tailscale/tailscale
- License: BSD 3-Clause for the open-source client
- Installed separately by each player and not redistributed in this package.
