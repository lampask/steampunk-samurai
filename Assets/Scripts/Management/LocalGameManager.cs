using UnityEngine;
using UnityEngine.Events;
using Definitions;
using Models;

namespace Management
{
    public class PlayerEvent : UnityEvent<int> {  }
    public class LocalGameManager : MonoBehaviour
    {
        public static LocalGameManager instance;
        public static LoadDefinitions definitions { get; protected set; }
        public static LoadModels models;

        public PlayerEvent onPlayerHealthChangeEvent;
        public PlayerEvent onPlayerEnergyChangeEvent;
        
        private void Awake() {
            if (!instance)
                instance = this;
            else
                Destroy(this);
            
            if (onPlayerHealthChangeEvent != null) onPlayerHealthChangeEvent = new PlayerEvent();
            if (onPlayerEnergyChangeEvent != null) onPlayerEnergyChangeEvent = new PlayerEvent();
        }

        void Start()
        {
            LoadModels();
            // Load Arena
            
            
            // Load Players
            foreach (var def in definitions.players)
            {
                
            }
        }

        void LoadModels()
        {
            definitions = new LoadDefinitions();
            models = new LoadModels();
        }

        
        
    }
}