using System.Collections.Generic;
namespace Models
{
    public class Models
    {
        public Dictionary<int, PlayerModel> players;
        public Dictionary<int, ArenaModel> arenas;

        public Models()
        {
            // TODO: load data from external file/service etc.
            LoadFakeData();
        }

        void LoadFakeData()
        {
            players = new Dictionary<int, PlayerModel>()
            {
                {0, new PlayerModel(0,100,100)}
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