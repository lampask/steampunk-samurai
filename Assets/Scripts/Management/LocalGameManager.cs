using System;
using System.Collections.Generic;
using System.Linq;
using Behaviours;
using UnityEngine;
using UnityEngine.Events;
using Definitions;
using GameEnvironment;
using Models;
using Utilities;
using Random = System.Random;

namespace Management
{
    public class PlayerEvent : UnityEvent<int> {  }
    public class LocalGameManager : MonoBehaviour
    {
        public static LocalGameManager instance;
        public static LoadDefinitions definitions { get; protected set; }
        public static LoadModels models { get; protected set; }

        public Transform barObject;

        private List<Tuple<PlayerModel, PlayerDefinition>> playerComponents;
        private Tuple<ArenaModel, ArenaDefinition> arenaComponents;
        
        // IN GAME VARS

        public GameObject arena;
        public List<PlayerBehaviour> players;

        public Dictionary<int, Tuple<PlayerModel, PlayerDefinition>> playerData;
        
        // GAME EVENTS
        
        public PlayerEvent onPlayerHealthChangeEvent;
        public PlayerEvent onPlayerEnergyChangeEvent;
        public UnityEvent gameStart;
        public UnityEvent gameOver;
        public PlayerEvent roundFinished;
        public UnityEvent powerUpSpawned;

        private void Awake() {
            if (!instance)
                instance = this;
            else
                Destroy(this);
            
            // Initialize events
            if (onPlayerHealthChangeEvent == null) onPlayerHealthChangeEvent = new PlayerEvent();
            if (onPlayerEnergyChangeEvent == null) onPlayerEnergyChangeEvent = new PlayerEvent();

            playerComponents = MenuManager.instance.selections.Where(s => s.confirmed).ToList().Select(s => s.ConvertToDefiningComponents()).ToList();
            //arenaComponents = MenuManager.instance.arena.ConvertToDefiningComponents();
            
            // Initialize easily accessible bridge
            playerData = playerComponents.ToDictionary(p => p.Item1.id);


            // We're freezing bois ...
            Time.timeScale = 0;
        }
        
        private void Start()
        {
            // Load external components
            LoadComponents();
            
            // Load Arena
            arena = new GameObject("ArenaHolder");
            
            /*
             * 0 - Backgrounds
             * 1 - Buildings
             * 2 - Effects
             */
            // TODO: move asset loading to imports 
            
            // 0 
            var bgs = new GameObject("Backgrounds");
            foreach (var b in arenaComponents.Item2.backgrounds)
            {
                var a = (GameObject) Instantiate(Resources.Load($"Prefabs/Backgrounds/{b.asset}"), Vector3.zero,
                    Quaternion.identity);
                a.transform.SetParent(bgs.transform);
                a.GetComponent<SpriteRenderer>().sortingOrder = (int) b.parallaxPosition;
            }
            
            // 1
            var buildings = new GameObject("Buildings");
            foreach (var b in arenaComponents.Item2.buildings)
            {
                var a = (GameObject) Instantiate(Resources.Load($"Prefabs/Buildings/{b.asset}"), b.position,
                    Quaternion.identity);
                a.transform.SetParent(bgs.transform);
                a.GetComponent<SpriteRenderer>().sortingOrder = b.layer;
            }
            
            // 2 (idk will be used in future)
            var effects = new GameObject("Effects");
            arenaComponents.Item2.effects.ForEach(e => Instantiate(new GameObject(), e.position, Quaternion.identity).transform.SetParent(effects.transform));
            
            // Create hierarchy

            bgs.transform.SetParent(arena.transform);
            buildings.transform.SetParent(arena.transform);
            effects.transform.SetParent(arena.transform);
            
            
            // Load Players into generated arena
            
            // Shuffle spawn points
            var rnd = new Random();
            var spawnPoints = arenaComponents.Item2.spawnLocations[MenuManager.instance.confirmed]
                .Select(x => new {value = x, order = rnd.Next()})
                .OrderBy(x => x.order).Select(x => x.value).ToList();
            
            playerComponents.ForEach(c =>
            {
                players.Add(Instantiate(Imports.PlayerObject,
                    spawnPoints[c.Item1.id].position,
                    Quaternion.identity).GetComponent<PlayerBehaviour>());
                
                // Generate bars ==> PlayerInfo Objects
                ((GameObject) Instantiate(Resources.Load("Prefabs/Bar"))).transform.SetParent(barObject);
            });
            
        }

        void LoadComponents()
        {
            // Load from external sources
            definitions = new LoadDefinitions();
            models = new LoadModels();
        }
    }
}