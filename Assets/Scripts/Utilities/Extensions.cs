using UnityEngine;

namespace Utilities
{
    public static class Extensions {
        public static void Half(this Vector2 vector)
        {
            vector = new Vector2(0.5f, 0.5f);
        }
    }
}