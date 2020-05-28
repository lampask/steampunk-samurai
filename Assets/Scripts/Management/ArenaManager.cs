using UnityEngine;

namespace Management
{
    public class ArenaManager : MonoBehaviour
    {
        public static ArenaManager instance;

        private void Awake() {
            if (!instance)
                instance = this;
            else
                Destroy(this);
        }
    }
}
