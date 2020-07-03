using Gameplay.Input;

namespace Definitions
{
    public class PlayerDefinition : Definition
    {
        public int id { get; private set; }
        public int color { get; private set; }
        public int type { get; private set; }
        public int maxHealth { get; private set; }
        public int maxEnergy { get; private set; }
        public PlayerDefinition(int id, int color, int type, int maxHealth, int maxEnergy)
        {
            this.id = id;
            this.color = color;
            this.type = type;
            this.maxHealth = maxHealth;
            this.maxEnergy = maxEnergy;
        }
    }
}