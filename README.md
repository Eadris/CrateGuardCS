# CrateGuardCS
Crate Guards for Rust

A Rust game plugin that automatically spawns hostile scientist NPCs to guard supply drop crates when they land.

## Quick Start

### For Server Administrators

Download the latest release from the `release/` directory:
1. Copy `CrateGuardCS.dll` to `oxide/plugins/`
2. Copy `CrateGuards.json` to `oxide/config/`
3. Reload with: `oxide.reload crateGuards`

See `release/README.md` for detailed installation instructions and configuration options.

### For Developers

Build from source:
```powershell
# Requires .NET 6.0 SDK or later
.\build.ps1 -RustServerPath "C:\path\to\rust\server"
```

Compiled DLL will be in `oxide/plugins/net48/CrateGuardCS.dll`

See `BUILD.md` for detailed build instructions.
