using UnityEngine;

namespace GameEnvironment
{
    [System.Serializable]
    public class Building
    {
        public Vector2 position { get; private set; }
        public int layer { get; private set; }
        public string asset { get; private set; }
        
        public Building(Vector2 position, int layer, string asset)
        {
            this.position = position;
            this.layer = layer;
            this.asset = asset;
        }
    }
}