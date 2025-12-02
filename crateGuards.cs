using System.Collections.Generic;
using System.Reflection;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    // Define the plugin name and author
    [Info("CrateGuardCS", "Eadris", "1.0.0")]
    [Description("Spawns scientists to guard supply drop crates.")]
    public class CrateGuardCS : RustPlugin
    {
        // --- Configuration Fields ---
        
        // This class defines the structure for your configuration file (oxide/config/CrateGuardCS.json)
        class Configuration
        {
            public int GuardsPerCrate = 3;
            public float RoamRadius = 15f;
        }

        private Configuration config;

        // The prefab path for the scientist NPC (Rust 2025)
        private const string ScientistPrefab = "assets/prefabs/npc/scientist/scientist_full_any.prefab";

        // --- Plugin Initialization ---

        protected override void LoadConfig()
        {
            base.LoadConfig();
            config = Config.ReadObject<Configuration>();
        }

        protected override void LoadDefaultConfig()
        {
            config = new Configuration();
        }

        void OnServerInitialized()
        {
            // Code that runs when the server is fully started
            Puts("CrateGuardCS plugin loaded successfully.");
            Puts($"  Guards per crate: {config.GuardsPerCrate}");
            Puts($"  Roam radius: {config.RoamRadius}");
        }

        // --- Chat Commands ---

        [ChatCommand("crateguards")]
        void CmdCrateGuards(BasePlayer player, string command, string[] args)
        {
            if (!player.IsAdmin)
            {
                player.ChatMessage("You need admin permissions to use this command.");
                return;
            }

            if (args.Length == 0)
            {
                player.ChatMessage("=== CrateGuardCS Commands ===");
                player.ChatMessage("/crateguards guards <number> - Set guards per crate");
                player.ChatMessage("/crateguards radius <distance> - Set roam radius");
                player.ChatMessage("/crateguards status - Show current settings");
                return;
            }

            string subcommand = args[0].ToLower();

            if (subcommand == "guards" && args.Length > 1)
            {
                if (int.TryParse(args[1], out int guardCount) && guardCount > 0)
                {
                    config.GuardsPerCrate = guardCount;
                    SaveConfig();
                    player.ChatMessage($"Guards per crate set to: {guardCount}");
                    Puts($"[CrateGuards] Admin {player.displayName} set guards per crate to: {guardCount}");
                }
                else
                {
                    player.ChatMessage("Invalid value. Please use a positive number.");
                }
            }
            else if (subcommand == "radius" && args.Length > 1)
            {
                if (float.TryParse(args[1], out float radius) && radius > 0f)
                {
                    config.RoamRadius = radius;
                    SaveConfig();
                    player.ChatMessage($"Roam radius set to: {radius}");
                    Puts($"[CrateGuards] Admin {player.displayName} set roam radius to: {radius}");
                }
                else
                {
                    player.ChatMessage("Invalid value. Please use a positive number.");
                }
            }
            else if (subcommand == "status")
            {
                player.ChatMessage("=== CrateGuardCS Status ===");
                player.ChatMessage($"Guards per crate: {config.GuardsPerCrate}");
                player.ChatMessage($"Roam radius: {config.RoamRadius}");
            }
            else
            {
                player.ChatMessage("Unknown subcommand. Use /crateguards for help.");
            }
        }

        // --- Oxide Hook (The main logic) ---

        // HOOK: This is the uMod/Oxide hook that triggers when a supply drop lands.
        // The object passed is the SupplyDrop entity (a BaseEntity).
        void OnSupplyDropLanded(SupplyDrop supplyDrop)
        {
            if (supplyDrop == null || !supplyDrop.IsValid()) 
            {
                return;
            }

            Vector3 cratePos = supplyDrop.transform.position;
            int guards = config.GuardsPerCrate;
            
            Puts($"Supply Crate landed at {cratePos}. Spawning {guards} guards...");

            for (int i = 0; i < guards; i++)
            {
                float xOffset = Random.Range(-5f, 5f);
                float zOffset = Random.Range(-5f, 5f);
                Vector3 spawnPos = cratePos + new Vector3(xOffset, 1.5f, zOffset);

                BaseEntity entity = GameManager.server.CreateEntity(ScientistPrefab, spawnPos);
                if (entity == null || !entity.IsValid())
                {
                    Puts($"Error: Failed to create scientist entity #{i+1}.");
                    continue;
                }

                entity.enableSaving = false;
                entity.Spawn();

                // Try to cast to NPCPlayerApex (actual scientist AI)
                var apexType = entity.GetType().Assembly.GetType("NPCPlayerApex");
                if (apexType != null && apexType.IsInstanceOfType(entity))
                {
                    dynamic npc = entity;
                    npc.Hostile = true;
                    npc.SetHome(cratePos);
                    npc.minRoamRange = 0f;
                    npc.maxRoamRange = config.RoamRadius;
                    npc.StartAI();
                    Puts($"Spawned Scientist #{i+1} (Apex AI) near crate.");
                }
                else
                {
                    // Fallback: send SetHome and try to set Hostile property
                    entity.SendMessage("SetHome", cratePos, SendMessageOptions.DontRequireReceiver);
                    var type = entity.GetType();
                    var hostileProp = type.GetProperty("Hostile");
                    if (hostileProp != null)
                        hostileProp.SetValue(entity, true);
                    var minRangeProp = type.GetProperty("minRoamRange");
                    var maxRangeProp = type.GetProperty("maxRoamRange");
                    if (minRangeProp != null)
                        minRangeProp.SetValue(entity, 0f);
                    if (maxRangeProp != null)
                        maxRangeProp.SetValue(entity, config.RoamRadius);
                    Puts($"Spawned Scientist #{i+1} (fallback) near crate.");
                }
            }
        }
    }
}