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
            players = new Dictionary<int, PlayerModel>()
            {
                {0, new PlayerModel(0, 100, 50, GlobalGameManager.instance.controls[new Unid((UserIndex) 1)])}
            };
            arenas = new Dictionary<int, ArenaModel>
            {
                {0, new ArenaModel()},
                {1, new ArenaModel()},
                {2, new ArenaModel()}
            };
        }
    }
}