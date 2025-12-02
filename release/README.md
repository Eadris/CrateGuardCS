# CrateGuardCS - Release Package

This directory contains everything needed to install the CrateGuardCS plugin on your Rust server.

## Prerequisite

This plugin requires the Oxide/uMod runtime (Oxide.Rust) to be installed on your Rust server. Oxide provides the plugin system that loads and runs uMod plugins.

Quick install summary:

- Download Oxide for Rust from: https://umod.org/
- Extract the Oxide release ZIP into your Rust server root (where `RustDedicated.exe` is). This will create an `oxide/` folder.
- Restart the server and verify Oxide is initialized by running `oxide.version` in the server console.

See the **Installation** section below for deploying this plugin after Oxide is installed.

## Installation

### Step 1: Place Plugin Source
Copy `crateGuards.cs` into your server's `oxide/plugins` folder. (Do not use the DLL; Oxide/uMod will compile the source file automatically.)

### Step 2: Place Configuration
Copy `CrateGuards.json` into your server's `oxide/config` folder.

### Step 3: Restart Server
Restart your Rust server. Oxide will compile and load the plugin automatically.

### Step 4: Verify Plugin
In the server console, run:
```
oxide.plugins
```
You should see `CrateGuardCS` listed. If not, check server logs and ensure the `.cs` file is present in `oxide/plugins`.

## Configuration

Edit `oxide/config/CrateGuards.json` to customize behavior:

```json
{
  "GuardsPerCrate": 3,      // Number of scientist NPCs per crate
  "RoamRadius": 15.0        // How far scientists roam from the crate
}
```

After editing the config, reload with:
```
oxide.reload crateGuards
```

## Admin Commands

Admins can also adjust settings in-game without reloading:

```
/crateguards status        - Show current settings
/crateguards guards <num>  - Set guards per crate (applies immediately)
/crateguards radius <dist> - Set roam radius (applies immediately)
```

Example:
```
/crateguards guards 5
/crateguards radius 20
/crateguards status
```

Changes made via commands are automatically saved to the config file.

## Features

- Automatically spawns hostile scientist NPCs when supply crates land
- Configurable number of guards per crate
- Configurable patrol radius
- Scientists spawn at random positions around the crate
- Configurable via JSON configuration file

## Version

- Plugin Version: 1.0.0
- Built for: Rust Dedicated Server (latest)
- Framework: Oxide/uMod 2.0
