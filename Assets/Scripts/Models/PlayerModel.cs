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
                {
                    _health = value;
                    LocalGameManager.instance.onPlayerHealthChangeEvent.Invoke(id);
                    if (_health <= 0)
                        LocalGameManager.instance.onPlayerDeath.Invoke(id);
                }
            }
        }

        private int _energy;
        public int energy
        {
            get => _energy;
            set
            {
                if (value != _energy)
                {
                    _energy = value;
                    LocalGameManager.instance.onPlayerEnergyChangeEvent.Invoke(id);
                }
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