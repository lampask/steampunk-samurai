using UnityEngine;

namespace Environment
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Vector2 position { get; private set; }
        public bool facing { get; private set; }

        public SpawnPoint(Vector2 position, bool facing)
        {
            this.position = position;
            this.facing = facing;
        }
    }
}