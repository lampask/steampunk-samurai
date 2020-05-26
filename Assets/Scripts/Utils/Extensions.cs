using UnityEngine;


public static class Extensions {
    public static void half(this Vector2 vector)
    {
        vector = new Vector2(0.5f, 0.5f);
    }
}