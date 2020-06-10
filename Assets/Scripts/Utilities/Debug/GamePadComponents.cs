using UnityEngine;

namespace Utilities.Debug
{
    public class GamePadComponents
    {
        public Rect lTrigger;
        public Rect rTrigger;

        public Rect lStick;
        public Vector2 lStickCenter;
        public Rect rStick;
        public Vector2 rStickCenter;

        public Rect dPad;
        public bool compositeDPad;
        public Rect up;
        public Rect right;
        public Rect down;
        public Rect left;
        public Rect lShoulder;
        public Rect rShoulder;
        public Rect x;
        public Rect y;
        public Rect b;
        public Rect a;
        public Rect start;
        public Rect back;
        public Rect guide;

        public GamePadComponents(Rect lTrigger, Rect rTrigger, Rect lStick, Vector2 lStickCenter, 
            Rect rStick, Vector2 rStickCenter, Rect dPad, bool compositeDPad, Rect up, Rect right, 
            Rect down, Rect left, Rect lShoulder, Rect rShoulder, Rect x, Rect y, Rect b, Rect a, 
            Rect start, Rect back, Rect guide)
        {
            this.lTrigger = lTrigger;
            this.rTrigger = rTrigger;
            this.lStick = lStick;
            this.lStickCenter = lStickCenter;
            this.rStick = rStick;
            this.rStickCenter = rStickCenter;
            this.dPad = dPad;
            this.compositeDPad = compositeDPad;
            this.up = up;
            this.right = right;
            this.down = down;
            this.left = left;
            this.lShoulder = lShoulder;
            this.rShoulder = rShoulder;
            this.x = x;
            this.y = y;
            this.b = b;
            this.a = a;
            this.start = start;
            this.back = back;
            this.guide = guide;
        }
    }
}