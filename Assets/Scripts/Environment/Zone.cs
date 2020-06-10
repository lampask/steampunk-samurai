using System;
using UnityEngine;

namespace Environment
{
    [Serializable]
    public class Zone
    {
        public Tuple<Vector2, Vector2> zoneLocation { get; private set; }
        public float zoneRadius { get; private set; }
        public int zoneType { get; private set; }

        public Zone(Tuple<Vector2, Vector2> zoneLocation, float zoneRadius, int zoneType)
        {
            this.zoneLocation = zoneLocation;
            this.zoneRadius = zoneRadius;
            this.zoneType = zoneType;
        }
    }
}