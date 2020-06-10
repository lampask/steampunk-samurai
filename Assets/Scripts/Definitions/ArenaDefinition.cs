using System.Collections.Generic;
using Environment;

namespace Definitions
{
    public class ArenaDefinition
    {
        public Dictionary<int, List<SpawnPoint>> spawnLocations { get; private set; }
        public List<Building> buildings { get; private set; }
        public List<Zone> zones { get; private set; }
        public List<Background> backgrounds { get; private set; }
        public List<Effect> effects { get; private set; }

        public ArenaDefinition(Dictionary<int, List<SpawnPoint>> spawnLocations, List<Building> buildings, List<Zone> zones, List<Background> backgrounds, List<Effect> effects)
        {
            this.spawnLocations = spawnLocations;
            this.buildings = buildings;
            this.zones = zones;
            this.backgrounds = backgrounds;
            this.effects = effects;
        }
    }
}