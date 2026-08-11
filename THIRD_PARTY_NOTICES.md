# Third-Party Notices

This launcher repository uses or refers to the following third-party software.
ROM files are not third-party dependencies of this repository and are never
redistributed by the build scripts.

## RetroArch 1.22.2

- Binary source: https://buildbot.libretro.com/stable/1.22.2/windows/x86_64/RetroArch.7z
- Source code: https://github.com/libretro/RetroArch/tree/v1.22.2
- License: GNU General Public License v3.0
- Bundled license in private runtime packages: `runtime/retroarch/licenses/RetroArch-COPYING.txt`

## FinalBurn Neo libretro core

- Binary source: https://buildbot.libretro.com/stable/1.22.2/windows/x86_64/RetroArch_cores.7z
- Source snapshot: https://github.com/finalburnneo/FBNeo/tree/b9833b57790093fd2e9c8dccc321375a6537fe3a
- License: FBNeo project license, including its non-commercial-use conditions
- Bundled license in private runtime packages: `runtime/retroarch/licenses/FBNeo-LICENSE.md`

The buildbot core archive was last modified 2025-11-20 02:52 UTC. The pinned
FBNeo source commit above predates that archive and supplies the complete
license text. The buildbot binary does not embed a verifiable FBNeo commit ID,
so this document does not claim an exact binary-to-commit correspondence.

## Radmin VPN

- Website: https://www.radmin-vpn.com/
- Used as separately installed private virtual LAN software.
- Not redistributed by this repository.
