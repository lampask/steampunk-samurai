namespace Environment
{
    [System.Serializable]
    public class Background
    {
        public string asset { get; private set; }
        public float parallaxPosition { get; private set; }
       
        public Background(string asset, float parallaxPosition)
        {
            this.asset = asset;
            this.parallaxPosition = parallaxPosition;
        }
    }
}