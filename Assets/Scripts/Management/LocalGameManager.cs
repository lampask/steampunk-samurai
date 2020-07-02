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
        public Transform cameraObject;
        public Transform anchor;
        
        private List<Tuple<PlayerModel, PlayerDefinition>> _playerComponents;
        private Tuple<ArenaModel, ArenaDefinition> _arenaComponents;
        
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

        public float loadProgress;
        public bool doneLoading;
        
        private void Awake() {
            if (!instance)
                instance = this;
            else
                Destroy(this);
            
            // Initialize events
            if (onPlayerHealthChangeEvent == null) onPlayerHealthChangeEvent = new PlayerEvent();
            if (onPlayerEnergyChangeEvent == null) onPlayerEnergyChangeEvent = new PlayerEvent();

            _playerComponents = MenuManager.instance.selections.Where(s => s.confirmed).ToList().Select(s => s.ConvertToDefiningComponents()).ToList();
            _arenaComponents = /*MenuManager.instance.arena.ConvertToDefiningComponents();*/ DefaultArena();
            
            // Initialize easily accessible bridge
            playerData = _playerComponents.ToDictionary(p => p.Item1.id);


            // We're freezing bois ...
            //Time.timeScale = 0;
        }
        
        // TEMP CODE
        // TODO: Remove and use in arena selector
        private Tuple<ArenaModel, ArenaDefinition> DefaultArena()
        {
            var model = new ArenaModel();
            var definition = new ArenaDefinition(
                new Dictionary<int, List<SpawnPoint>>()
                {
                    {2, new List<SpawnPoint>()
                    {
                        new SpawnPoint(new Vector2(-30,-3.5f), true),
                        new SpawnPoint(new Vector2(30,-3.5f), false)
                    }},
                    {3, new List<SpawnPoint>()
                    {
                        new SpawnPoint(new Vector2(0,-10), true),
                        new SpawnPoint(new Vector2(30,15), true),
                        new SpawnPoint(new Vector2(-30,15), false)
                    }},
                    {4, new List<SpawnPoint>()
                    {
                        new SpawnPoint(new Vector2(30,-13), true),
                        new SpawnPoint(new Vector2(-30,-13), false),
                        new SpawnPoint(new Vector2(30,15), true),
                        new SpawnPoint(new Vector2(-30,15), false)
                    }}
                },
                new List<Building>()
                {
                    new Building(new Vector2(0, 0), 0, "budova1"),
                    new Building(new Vector2(30, 0), 0, "budova2"),
                    new Building(new Vector2(-30, 0), 0, "budova2"),
                }, 
                new List<Zone>()
                {
                    
                }, 
                new List<Background>()
                {
                    new Background("pozadie", 0f, new Vector2(0,0)),
                    new Background("mesiac", 1f, -2, new Vector2(0,20)),
                    new Background("hmla", 1f, 100, new Vector2(0,9)),
                },
                new List<Effect>()
                {
                    
                }
            );
            return new Tuple<ArenaModel, ArenaDefinition>(model, definition);
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
            foreach (var b in _arenaComponents.Item2.backgrounds)
            {
                var a = (GameObject) Instantiate(Resources.Load("Backgrounds/Background"), Vector2.zero + b.offset,
                    Quaternion.identity);
                a.transform.SetParent(bgs.transform);
                var sr = a.GetComponent<SpriteRenderer>();
                sr.sortingOrder = b.layer;
                sr.sprite = Resources.Load<Sprite>($"Backgrounds/{b.asset}");
            }
            
            // 1
            var buildings = new GameObject("Buildings");
            foreach (var b in _arenaComponents.Item2.buildings)
            {
                var a = (GameObject) Instantiate(Resources.Load($"Buildings/{b.asset}"), b.position,
                    Quaternion.identity);
                a.transform.SetParent(buildings.transform);
                //a.GetComponent<SpriteRenderer>().sortingOrder = b.layer;
            }
            
            // 2 (idk will be used in future)
            var effects = new GameObject("Effects");
            _arenaComponents.Item2.effects.ForEach(e => Instantiate(new GameObject(), e.position, Quaternion.identity).transform.SetParent(effects.transform));
            
            // Create hierarchy

            bgs.transform.SetParent(arena.transform);
            buildings.transform.SetParent(arena.transform);
            effects.transform.SetParent(arena.transform);
            
            arena.transform.SetParent(anchor);
            
            
            // Load Players into generated arena
            
            // Shuffle spawn points
            var rnd = new Random();
            var spawnPoints =  new Stack<SpawnPoint>(_arenaComponents.Item2.spawnLocations[MenuManager.instance.confirmed]
                .Select(x => new {value = x, order = rnd.Next()})
                .OrderBy(x => x.order).Select(x => x.value).ToList());
            
            _playerComponents.ForEach(c =>
            {
                var p = Instantiate(Imports.PlayerObject,
                    spawnPoints.Pop().position,
                    Quaternion.identity);
                
                var pb = p.GetComponent<PlayerBehaviour>();
                players.Add(pb);

                pb.id = c.Item1.id;
                pb.controlledBy = c.Item1.control;
                
                // Generate bars ==> PlayerInfo Objects
                ((GameObject) Instantiate(Resources.Load("PlayerInfo"), Vector3.zero, Quaternion.identity)).transform.SetParent(barObject);
                
                p.transform.SetParent(anchor);
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