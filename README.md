# CrateGuardCS
Crate Guards for Rust

A Rust game plugin that automatically spawns hostile scientist NPCs to guard supply drop crates when they land.

## Quick Start

### For Server Administrators

Download the latest release from the `release/` directory:

1. Copy `crateGuards.cs` to your server's `oxide/plugins/` folder (use the source file, not the DLL).
2. Copy `CrateGuards.json` to `oxide/config/`
3. Restart your Rust server. Oxide will compile and load the plugin automatically.

See `release/README.md` for detailed installation instructions and configuration options.

## Prerequisite

This plugin requires the Oxide/uMod runtime (Oxide.Rust) to be installed on your Rust server. Oxide provides the plugin system that loads and runs uMod plugins.

Quick install summary:

- Download Oxide for Rust from: https://umod.org/
- Extract the Oxide release ZIP into your Rust server root (where `RustDedicated.exe` is). This will create an `oxide/` folder.
- Restart the server and verify Oxide is initialized by running `oxide.version` in the server console.

After Oxide is installed, deploy this plugin as described above.

### For Developers

Build from source:
```powershell
# Requires .NET 6.0 SDK or later
.\build.ps1 -RustServerPath "C:\path\to\rust\server"
```

Compiled DLL will be in `oxide/plugins/net48/CrateGuardCS.dll`

See `BUILD.md` for detailed build instructions.
