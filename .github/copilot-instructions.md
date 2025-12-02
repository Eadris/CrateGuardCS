# CrateGuardCS - AI Coding Agent Instructions

## Project Overview
CrateGuardCS is a Rust game plugin (using Oxide/uMod framework) written in C# that automatically spawns hostile scientist NPCs to guard supply drop crates when they land in-game.

## Architecture & Key Components

### Plugin Structure
- **Main Plugin Class**: `CrateGuardCS` (in `crateGuards.cs`) - extends `RustPlugin`
- **Oxide Framework**: Rust plugin framework providing hooks, configuration, and entity spawning
- **Configuration System**: Uses Oxide's built-in `Config.ReadObject<T>()` pattern

### Data Flow
1. Server initializes → plugin loads configuration from `oxide/config/CrateGuards.json`
2. Supply drop lands → `OnSupplyDropLanded` hook fires (Oxide event system)
3. Plugin spawns N scientist entities at randomized positions around crate
4. Scientists patrol crate using AI (home position set via `SetHome()`)

### Core Dependencies
- **UnityEngine**: Game engine (Vector3, physics, transforms)
- **Oxide.Core**: Plugin framework for hooks and configuration
- **Rust Game API**: `SupplyDrop`, `Scientist`, `BaseEntity`, `GameManager.server`

## Configuration Patterns

Configuration is declared in a nested class and populated from JSON:
```csharp
class Configuration {
    public int GuardsPerCrate = 3;      // Default: 3 scientists per crate
    public float RoamRadius = 15f;       // Default: 15 unit patrol radius
}
```

The JSON file (`oxide/config/CrateGuards.json`) mirrors this structure exactly. When modifying config, update both the class and JSON file.

## Plugin Hooks & Game Integration

- **`OnServerInitialized()`**: Fired when server fully starts - use for initialization logging
- **`OnSupplyDropLanded(SupplyDrop)`**: Primary hook - fires when supply crates land; receives the crate entity
- **`ScientistPrefab` const**: Points to `"assets/prefabs/npc/scientist/scientist.prefab"` - don't hardcode paths elsewhere

## Entity Spawning Patterns

Standard pattern for spawning Rust NPCs:
```csharp
BaseEntity entity = GameManager.server.CreateEntity(prefabPath, position);
if (entity is DesiredType castedEntity) {
    // Configure: SetHome(), Set properties, Hostile = true, etc.
    castedEntity.Spawn();  // CRITICAL: Must call Spawn() to activate
}
```

Key properties for Scientist configuration:
- `Hostile = true`: Makes NPC attack players
- `SetHome(Vector3)`: Sets patrol center point
- `minRoamRange / maxRoamRange`: Defines roam boundaries

## Naming & Style Conventions
- Plugin name in `[Info]` attribute must match class name and filename: `CrateGuardCS`
- Configuration class is nested (not a separate file)
- Prefab paths use assets/prefabs/ structure - validate in Rust documentation
- Use `Puts()` for server console logging (Oxide standard)

## Common Patterns to Follow
1. **Null checks**: Always validate `supplyDrop != null && supplyDrop.IsValid()` before use
2. **Random positioning**: Use `Random.Range()` for varied spawns (avoids stacking)
3. **Entity casting**: Use `is` keyword for safe type casting before use
4. **Configuration**: Load in `LoadConfig()` and provide defaults in `LoadDefaultConfig()`

## Testing & Debugging
- Plugin reloads via `oxide.reload crateGuards` command (Oxide CLI)
- Console output via `Puts()` appears in server logs
- Invalid prefab paths will cause silent failures - check entity != null after creation

## File Organization
```
crateGuards/
├── crateGuards.cs           # Main plugin (single-file for simplicity)
├── oxide/config/CrateGuards.json  # Configuration (matches Configuration class)
└── oxide/plugins/            # (empty - plugins deployed here by admin)
```
