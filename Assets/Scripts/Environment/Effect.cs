using UnityEngine;

namespace Environment
{
    public class Effect
    {
        public Vector2 position { get; private set; }
        
        public Effect(Vector2 position)
        {
            this.position = position;
        }
    }
}