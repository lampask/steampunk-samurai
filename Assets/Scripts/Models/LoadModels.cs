using System.Collections.Generic;
using Gameplay.Input;
using Management;
using SharpDX.XInput;

namespace Models
{
    public class LoadModels
    {
        public Dictionary<int, PlayerModel> players;
        public Dictionary<int, ArenaModel> arenas;

        public LoadModels()
        {
            // TODO: load data from external file/service etc.
            LoadFakeData();
        }

        void LoadFakeData()
        {
            
        }
    }
}