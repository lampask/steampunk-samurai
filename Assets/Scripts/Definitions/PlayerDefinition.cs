using Gameplay.Input;

namespace Definitions
{
    public class PlayerDefinition : Definition
    {
        public int id { get; private set; }
        public int color { get; private set; }
        public Control controls { get; private set; }
        
        public PlayerDefinition(int id, int color, Control controls)
        {
            this.id = id;
            this.color = color;
            this.controls = controls;
        }
    }
}