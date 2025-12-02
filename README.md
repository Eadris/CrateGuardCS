# CrateGuardCS
Crate Guards for Rust

A Rust plugin that automatically spawns hostile scientist NPCs to guard supply drop crates when they land.

## Features

- **Automatic Guard Spawning**: Scientists spawn around supply drops after they land
- **Configurable Settings**: Adjust min/max guard count, roam radius, and spawn delay
- **Admin Commands**: Manage settings in-game via chat commands
- **Permission System**: Control who can modify settings and spawn guards
- **Ground Detection**: Scientists spawn on terrain, not inside crates
- **Rust 2025 Compatible**: Uses modern Rust AI prefab system

## Quick Start

### For Server Administrators

1. **Install Oxide/uMod** (if not already installed):
   - Download Oxide for Rust from: https://umod.org/
   - Extract the Oxide release ZIP into your Rust server root (where `RustDedicated.exe` is)
   - Restart the server and verify Oxide is initialized by running `oxide.version` in the server console

2. **Install Plugin**:
   - Copy `CrateGuardCS.cs` to your server's `oxide/plugins/` folder
   - (Optional) Copy `CrateGuardCS.json` to `oxide/config/` to customize defaults
   - Restart your server or run `oxide.reload CrateGuardCS` in the server console

3. **Grant Admin Permissions**:
   Run these in the Rust server console:
   
   ```
   oxide.grant user <username> crateguard.spawn
   oxide.grant group admin crateguard.spawn
   
   # Example using SteamID64 directly
   oxide.grant user 76561198000000000 crateguard.spawn

   # Verify
   oxide.usergroup <username>
   oxide.show perms <username>
   
   # Optional: revoke
   oxide.revoke user <username> crateguard.spawn
   oxide.revoke group admin crateguard.spawn
   ```

## Configuration

Edit `oxide/config/CrateGuardCS.json`:

```json
{
  "MinGuardsPerCrate": 0,
  "MaxGuardsPerCrate": 3,
  "RoamRadius": 5.0,
  "SpawnDelaySeconds": 2,
   "MinSpawnDistanceFromCrate": 2.0,
   "EnableCrateGuards": true
}
```

- **MinGuardsPerCrate**: Minimum scientists to spawn (can be 0)
- **MaxGuardsPerCrate**: Maximum scientists to spawn (>= 1)
- **RoamRadius**: Randomization radius for spawn positions (in meters)
- **SpawnDelaySeconds**: Delay after crate lands before spawning guards
- **MinSpawnDistanceFromCrate**: Minimum distance (in meters) from the crate center where scientists can spawn
 - **EnableCrateGuards**: Toggle spawning guards for supply drop crates

## Admin Commands

Requires `crateguard.spawn` permission:

- `/crateguards` - Show available commands
- `/crateguards min <number>` - Set minimum guards per crate
- `/crateguards max <number>` - Set maximum guards per crate
- `/crateguards radius <distance>` - Set roam radius
- `/crateguards delay <seconds>` - Set spawn delay
 - `/crateguards crates <on|off>` - Toggle crate guards on or off
- `/crateguards status` - Show current settings
- `/crateguards spawn` - Spawn a scientist at your position (testing)

## Permissions Troubleshooting

- Ensure Oxide/uMod is loaded: run `oxide.version` in server console.
- Verify the player identifier: the plugin checks `player.UserIDString` (SteamID64). Use `status` in the server console to see connected players and their SteamIDs.
- Confirm permission assignment:
   - `oxide.show perms <username>` should list `crateguard.spawn`.
   - If using SteamID64, you can grant directly: `oxide.grant user <steamid64> crateguard.spawn`.
- Reload after changes: `oxide.reload CrateGuardCS`.
- Check logs for errors:
   - Compiler: `oxide/logs/oxide.compiler_*.log`
   - Runtime: `oxide/logs/oxide_*.txt`

## Technical Notes

**Rust 2025 API Limitations**: The Rust 2025 API does not expose methods to customize NPC behavior (aggression, patrol radius, etc.) via plugins. Scientists use default prefab AI behavior defined by the server.

**Prefab Path**: Uses `assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_roam.prefab` (modern Rust AI system).

**Ground Spawning**: Guards are raycast to terrain and spawned at ground level, with a minimum 2m offset from the crate center to avoid spawning inside the crate.

**Config Generation & Saving**:
- On first load, Oxide auto-generates `oxide/config/CrateGuardCS.json` with defaults.
- Empty or invalid configs are replaced with defaults and saved.
- Changes made via chat commands are saved immediately.

**Repo Hygiene**:
- The `oxide/` folder and generated config are not committed.
- Use `oxide.reload CrateGuardCS` to apply updates.

## For Developers

This is a source-only plugin. Simply edit `CrateGuardCS.cs` and Oxide will auto-compile on server restart or reload.
The plugin validates and persists configuration in `LoadConfig()` and `LoadDefaultConfig()` to ensure sane defaults.
