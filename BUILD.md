# Building CrateGuardCS Plugin

## Prerequisites

- .NET 6.0 SDK or later ([download](https://dotnet.microsoft.com/download))
- Required Oxide/Rust assemblies in the `lib/` directory

## Quick Start

### Option 1: Automatic (with Rust server path)
```powershell
.\build.ps1 -RustServerPath "C:\path\to\rust\server"
```

This will:
1. Copy `UnityEngine.dll` and `Assembly-CSharp.dll` from your Rust server
2. Compile the plugin to `oxide/plugins/CrateGuardCS.dll`

### Option 2: Manual (if you have assemblies already)
```powershell
.\build.ps1
```

Or using dotnet CLI directly:
```powershell
dotnet build CrateGuardCS.csproj -c Release
```

## Required Assemblies

The `lib/` directory needs:
- `Oxide.Core.dll` - Core plugin framework
- `Oxide.Rust.dll` - Rust-specific framework
- `UnityEngine.dll` - Game engine (from Rust server)
- `Assembly-CSharp.dll` - Rust game code (from Rust server)

### Getting Assemblies

**From Rust Server Installation:**
- Copy from: `RustDedicated_Data/Managed/`
- Required: `UnityEngine.dll`, `Assembly-CSharp.dll`

**Oxide Assemblies:**
- Available via NuGet package `Oxide.Rust`
- Or download from [Oxide GitHub](https://github.com/OxideMod/Oxide.Rust)

## Output

Compiled DLL is placed in: `oxide/plugins/CrateGuardCS.dll`

Deploy to your Rust server by copying the DLL to the server's `oxide/plugins/` directory.

## Troubleshooting

**"Missing required assemblies" error:**
- Ensure all required DLLs are in the `lib/` directory
- Use `-RustServerPath` parameter to auto-copy from your server installation

**Build fails with type resolution errors:**
- Verify the Rust server version matches your assembly versions
- Check that Assembly-CSharp.dll is from the same build as UnityEngine.dll

**Plugin doesn't load on server:**
- Verify the DLL is in the correct `oxide/plugins/` directory
- Check server console for errors: `oxide.show errors`
