using Management;

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
        
        
        
        public PlayerModel(int id, int health, int energy)
        {
            this.id = id;
            _health = health;
            _energy = energy;
        }
        
    }
}