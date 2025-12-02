using System.Collections.Generic;
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

        // The prefab path for the scientist NPC
        private const string ScientistPrefab = "assets/prefabs/npc/scientist/scientist.prefab";

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
                // 1. Determine a randomized spawn position near the crate
                float xOffset = Random.Range(-5f, 5f);
                float zOffset = Random.Range(-5f, 5f);
                
                // Spawn 1.5 units above the ground
                Vector3 spawnPos = cratePos + new Vector3(xOffset, 1.5f, zOffset);
                
                // 2. Create the scientist entity
                BaseEntity scientistEntity = GameManager.server.CreateEntity(ScientistPrefab, spawnPos);
                
                if (scientistEntity is Scientist scientist)
                {
                    // 3. Configure the Scientist's behavior
                    scientist.Hostile = true; // Make it aggressive towards players
                    
                    // Set the scientist's home position to the crate's location. 
                    // This tells the AI to stay and patrol around this point.
                    scientist.SetHome(cratePos); 
                    
                    // The min/max roam range is often handled by the AI controller, 
                    // but we can enforce the home range if needed.
                    scientist.minRoamRange = 0f;
                    scientist.maxRoamRange = config.RoamRadius;

                    // 4. Spawn the entity into the world
                    scientist.Spawn();
                    
                    Puts($"Spawned Scientist #{i+1} near crate.");
                }
                else
                {
                    Puts("Error: Failed to create or cast scientist entity.");
                }
            }
        }
    }
}