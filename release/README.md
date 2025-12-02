# CrateGuardCS - Release Package

This directory contains everything needed to install the CrateGuardCS plugin on your Rust server.

## Installation

### Step 1: Extract Files
Extract this archive to your Rust server root directory so that:
- `CrateGuardCS.dll` goes to `oxide/plugins/`
- `CrateGuards.json` goes to `oxide/config/`

### Step 2: Load the Plugin
If your server is already running, use the console command:
```
oxide.reload crateGuards
```

Or restart your server to auto-load the plugin.

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
