using UnityEngine;

namespace Management
{
    public class Settings
    {
        public static Settings instance;

        public Settings()
        {
            if (instance == null || instance.Equals(null))
                instance = this;
            else
                return;
        }
    }
}