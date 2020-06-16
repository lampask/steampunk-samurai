using Management;
using Gameplay.Input;

namespace Models
{
    
    public class PlayerModel : Model
    {
        public int id { get; private set; }
        
        private int _health;
        public int health
        {
            get => _health;
            set
            {
                if (value != _health)
                    LocalGameManager.instance.onPlayerHealthChangeEvent.Invoke(id);
            }
        }

        private int _energy;
        public int energy
        {
            get => _energy;
            set
            {
                if (value != _energy)
                    LocalGameManager.instance.onPlayerEnergyChangeEvent.Invoke(id);
            }
        }
        
        public Control control { get; private set; } 
        
        public PlayerModel(int id, int health, int energy, Control control)
        {
            this.id = id;
            _health = health;
            _energy = energy;
            this.control = control;
        }
        
    }
}