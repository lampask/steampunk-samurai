using UnityEngine;

namespace GameEnvironment
{
    [System.Serializable]
    public class Background
    {
        public string asset { get; private set; }
        public float parallaxPosition { get; private set; }

        public int layer;
        
        public Vector2 offset;
       
        public Background(string asset, float parallaxPosition, int layer, Vector2 offset)
        {
            this.asset = asset;
            this.parallaxPosition = parallaxPosition;
            this.layer = layer;
            this.offset = offset;
        }

        public Background(string asset, float parallaxPosition, Vector2 offset)
        {
            this.asset = asset;
            this.parallaxPosition = parallaxPosition;
            layer = -3;
            this.offset = offset;
        }

    }
}