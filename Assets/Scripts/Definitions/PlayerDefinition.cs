using Gameplay.Input;

namespace Definitions
{
    public class PlayerDefinition : Definition
    {
        public int id { get; private set; }
        public int color { get; private set; }
        public int type { get; private set; }
        public PlayerDefinition(int id, int color, int type)
        {
            this.id = id;
            this.color = color;
            this.type = type;
        }
    }
}