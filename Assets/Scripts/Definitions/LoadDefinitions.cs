using System.Collections.Generic;
using XInputDotNetPure;

namespace Definitions
{
    public class LoadDefinitions
    {
        public Dictionary<int, PlayerDefinition> players;
        public Dictionary<int, ArenaDefinition> arenas;

        public LoadDefinitions()
        {
            // TODO: load definitions from external files
            LoadFakeData();
        }

        void LoadFakeData()
        {
            players = new Dictionary<int, PlayerDefinition>
            {
                {0, new PlayerDefinition(0, 0, PlayerIndex.One)},
            };
            arenas = new Dictionary<int, ArenaDefinition>();
        }
    }
}