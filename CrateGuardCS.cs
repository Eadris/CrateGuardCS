using System.Collections.Generic;
using Rust;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("CrateGuardCS", "Eadris", "1.0.0")]
    [Description("Spawns scientists to guard supply drop crates.")]
    public class CrateGuardCS : RustPlugin
    {
        // Register permission on plugin load
        void Init()
        {
            permission.RegisterPermission("crateguard.spawn", this);
        }

        // Configuration structure
        private class Configuration
        {
            public int MinGuardsPerCrate = 0;
            public int MaxGuardsPerCrate = 3;
            public float RoamRadius = 5f;
            public int SpawnDelaySeconds = 2; // Delay after crate lands before spawning
        }

        private Configuration config;

        protected override void LoadConfig()
        {
            base.LoadConfig();
            config = Config.ReadObject<Configuration>();
            if (config == null)
            {
                // If the config file is empty or invalid, regenerate defaults and save
                LoadDefaultConfig();
                return;
            }
            // Ensure reasonable bounds and persist any missing/defaulted values
            if (config.MinGuardsPerCrate < 0)
            {
                config.MinGuardsPerCrate = 0;
            }
            if (config.MaxGuardsPerCrate < 0)
            {
                config.MaxGuardsPerCrate = 3;
            }
            if (config.MinGuardsPerCrate > config.MaxGuardsPerCrate)
            {
                config.MinGuardsPerCrate = 0;
                config.MaxGuardsPerCrate = Mathf.Max(1, config.MaxGuardsPerCrate);
            }
            if (config.RoamRadius <= 0f)
            {
                config.RoamRadius = 5f;
            }
            if (config.SpawnDelaySeconds < 0)
            {
                config.SpawnDelaySeconds = 0;
            }
            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            config = new Configuration();
            SaveConfig();
        }

        protected override void SaveConfig() => Config.WriteObject(config, true);

        void OnServerInitialized()
        {
            Puts("[CrateGuardCS] Plugin loaded successfully.");
            Puts($"[CrateGuardCS] Min guards per crate: {config.MinGuardsPerCrate}");
            Puts($"[CrateGuardCS] Max guards per crate: {config.MaxGuardsPerCrate}");
            Puts($"[CrateGuardCS] Roam radius: {config.RoamRadius}");
        }

        [ChatCommand("crateguards")]
        void CmdCrateGuards(BasePlayer player, string command, string[] args)
        {
            if (!permission.UserHasPermission(player.UserIDString, "crateguard.spawn"))
            {
                player.ChatMessage("You do not have permission to use this command.");
                return;
            }

            if (args.Length == 0)
            {
                player.ChatMessage("=== CrateGuardCS Commands ===");
                player.ChatMessage("/crateguards min <number> - Set minimum guards per crate (0+)");
                player.ChatMessage("/crateguards max <number> - Set maximum guards per crate (1+)");
                player.ChatMessage("/crateguards radius <distance> - Set roam radius");
                player.ChatMessage("/crateguards delay <seconds> - Set spawn delay in seconds");
                player.ChatMessage("/crateguards status - Show current settings");
                player.ChatMessage("/crateguards spawn - Spawn a scientist at your position (requires permission)");
                return;
            }

            string subcommand = args[0].ToLower();

            if (subcommand == "spawn")
            {
                Vector3 pos = player.transform.position + new Vector3(0, 2f, 0);
                SpawnScientist(pos);
                player.ChatMessage($"Spawned scientist at {pos.x}, {pos.y}, {pos.z}");
                Puts($"[CrateGuardCS] {player.displayName} spawned scientist at {pos.x}, {pos.y}, {pos.z}");
                return;
            }
            if (subcommand == "min" && args.Length > 1)
            {
                if (int.TryParse(args[1], out int minVal) && minVal >= 0)
                {
                    config.MinGuardsPerCrate = minVal;
                    if (config.MinGuardsPerCrate > config.MaxGuardsPerCrate)
                    {
                        config.MaxGuardsPerCrate = config.MinGuardsPerCrate;
                    }
                    SaveConfig();
                    player.ChatMessage($"Min guards per crate set to: {config.MinGuardsPerCrate}");
                    Puts($"[CrateGuardCS] {player.displayName} set min guards per crate to: {config.MinGuardsPerCrate}");
                }
                else
                {
                    player.ChatMessage("Invalid value. Please use a number >= 0.");
                }
            }
            else if (subcommand == "max" && args.Length > 1)
            {
                if (int.TryParse(args[1], out int maxVal) && maxVal >= 1)
                {
                    config.MaxGuardsPerCrate = maxVal;
                    if (config.MinGuardsPerCrate > config.MaxGuardsPerCrate)
                    {
                        config.MinGuardsPerCrate = config.MaxGuardsPerCrate;
                    }
                    SaveConfig();
                    player.ChatMessage($"Max guards per crate set to: {config.MaxGuardsPerCrate}");
                    Puts($"[CrateGuardCS] {player.displayName} set max guards per crate to: {config.MaxGuardsPerCrate}");
                }
                else
                {
                    player.ChatMessage("Invalid value. Please use a number >= 1.");
                }
            }
            else if (subcommand == "radius" && args.Length > 1)
            {
                if (float.TryParse(args[1], out float radius) && radius > 0f)
                {
                    config.RoamRadius = radius;
                    SaveConfig();
                    player.ChatMessage($"Roam radius set to: {radius}");
                    Puts($"[CrateGuardCS] {player.displayName} set roam radius to: {radius}");
                }
                else
                {
                    player.ChatMessage("Invalid value. Please use a positive number.");
                }
            }
            else if (subcommand == "delay" && args.Length > 1)
            {
                if (int.TryParse(args[1], out int delay) && delay >= 0)
                {
                    config.SpawnDelaySeconds = delay;
                    SaveConfig();
                    player.ChatMessage($"Spawn delay set to: {delay} seconds");
                    Puts($"[CrateGuardCS] {player.displayName} set spawn delay to: {delay} seconds");
                }
                else
                {
                    player.ChatMessage("Invalid value. Please use a non-negative number.");
                }
            }
            else if (subcommand == "status")
            {
                player.ChatMessage("=== CrateGuardCS Status ===");
                player.ChatMessage($"Min guards per crate: {config.MinGuardsPerCrate}");
                player.ChatMessage($"Max guards per crate: {config.MaxGuardsPerCrate}");
                player.ChatMessage($"Roam radius: {config.RoamRadius}");
                player.ChatMessage($"Spawn delay: {config.SpawnDelaySeconds} seconds");
            }
            else
            {
                player.ChatMessage("Unknown subcommand. Use /crateguards for help.");
            }
        }

        // Spawn scientist NPC at a given position
        // Common scientist prefab variations in Rust:
        // - assets/prefabs/npc/scientist/htn/scientist_full_any.prefab (common)
        // - assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_roam.prefab
        // - assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_roam_tethered.prefab
        private const string ScientistPrefab = "assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_roam.prefab";

        // Raycast constants for ground detection when spawning scientists
        private const float RaycastStartHeight = 50f;
        private const float DefaultGroundOffset = 1.5f;
        private const float RaycastMaxDistance = 100f;
        // NOTE: Rust 2025 does not support customizing patrol, aggression, or roam radius via plugin code.
        // The RoamRadius config only affects spawn position randomization, not AI behavior.
        // Scientists will use default prefab AI (patrol, attack, roam) as defined by the server.
        void SpawnScientist(Vector3 position)
        {
            BaseEntity entity = GameManager.server.CreateEntity(ScientistPrefab, position, Quaternion.identity);
            if (entity != null)
            {
                entity.Spawn();
                Puts($"[CrateGuardCS] Entity spawned: Type={entity.GetType().Name}, ShortPrefabName={entity.ShortPrefabName}");
                timer.Once(1f, () => {
                    if (entity != null && !entity.IsDestroyed)
                    {
                        Puts($"[CrateGuardCS] Entity confirmed alive at {position}");
                    }
                    else
                    {
                        Puts($"[CrateGuardCS] Entity failed to spawn at {position}");
                    }
                });
            }
            else
            {
                Puts($"[CrateGuardCS] Failed to create entity with prefab: {ScientistPrefab}");
            }
        }

        // --- Oxide Hook (The main logic) ---
        void OnSupplyDropLanded(SupplyDrop supplyDrop)
        {
            if (supplyDrop == null || !supplyDrop.IsValid()) 
            {
                return;
            }

            Vector3 cratePos = supplyDrop.transform.position;
            int minG = Mathf.Max(0, config.MinGuardsPerCrate);
            int maxG = Mathf.Max(minG, config.MaxGuardsPerCrate);
            int guards = Random.Range(minG, maxG + 1);
            Puts($"[CrateGuardCS] Supply Crate landed at {cratePos}. Will spawn {guards} guards in {config.SpawnDelaySeconds} seconds...");

            timer.Once(config.SpawnDelaySeconds, () => {
                for (int i = 0; i < guards; i++)
                {
                    // Ensure spawn offset is at least 2 units away from crate center
                    float xOffset = Random.Range(-config.RoamRadius, config.RoamRadius);
                    float zOffset = Random.Range(-config.RoamRadius, config.RoamRadius);
                    float distance = Mathf.Sqrt(xOffset * xOffset + zOffset * zOffset);
                    if (distance < 2f)
                    {
                        float scale = 2f / distance;
                        xOffset *= scale;
                        zOffset *= scale;
                    }
                    
                    Vector3 abovePos = cratePos + new Vector3(xOffset, RaycastStartHeight, zOffset);
                    RaycastHit hit;
                    Vector3 groundPos = cratePos + new Vector3(xOffset, DefaultGroundOffset, zOffset);
                    if (Physics.Raycast(abovePos, Vector3.down, out hit, RaycastMaxDistance, LayerMask.GetMask("Terrain", "World", "Default")))
                    {
                        groundPos = hit.point;
                    }
                    Puts($"[CrateGuardCS] Spawning scientist at {groundPos.x}, {groundPos.y}, {groundPos.z}");
                    SpawnScientist(groundPos);
                }
            });
        }
    }
}