using System.Collections.Generic;
using UnityEngine;
using Utilities.Debug;

namespace Utilities
{
    public static class Globals
    {
        public static string settingPath;
        public enum GamepadType
        {
            Xbox,
            Ps4,
            Ps3,
            Generic
        }
        public static readonly Dictionary<GamepadType, GamePadComponents> GamepadProps = new Dictionary<GamepadType, GamePadComponents>
        {
            {GamepadType.Xbox, new GamePadComponents(
                new Rect(71,12.5f, 10, 13),
                new Rect(144.5f,12.5f, 10.5f, 13),
                new Rect(63,48, 19.5f, 17.5f),
                new Vector2(71, 57), 
                new Rect(121,66, 19.5f, 17.5f),
                new Vector2(129, 75),
                new Rect(80,62, 25.5f, 21),
                false,
                new Rect(91,67,3.5f,3.5f),
                new Rect(98,72.5f,3.5f,3.5f),
                new Rect(91,77.5f,3.5f,3.5f),
                new Rect(84,72.5f,3.5f,3.5f),
                new Rect(60,25.5f, 26, 9.5f),
                new Rect(140,25.5f, 26, 9.5f),
                new Rect(136.5f,49, 10, 8),
                new Rect(148,40, 10, 9),
                new Rect(158.5f,49, 10.5f, 8),
                new Rect(147,56.5f, 10.5f, 9),
                new Rect(123.5f,51, 8, 5.5f),
                new Rect(94.5f,51, 8, 5.5f),
                new Rect(106,48, 13.5f, 11)
            )},
            {GamepadType.Ps3, new GamePadComponents(
                new Rect(64.5f, 5.5f, 16, 2),
                new Rect(153.5f, 5.5f, 16, 2),
                new Rect(85.5f, 53, 19.5f, 19.5f),
                new Vector2(93.5f, 61), 
                new Rect(129, 53, 19.5f, 19.5f),
                new Vector2(137, 61), 
                new Rect(),
                true,
                new Rect(68.5f, 23.5f, 8, 13.5f),
                new Rect(75, 35.5f, 13.5f, 8),
                new Rect(68.5f, 42, 8, 13.5f),
                new Rect(56.5f, 35.5f, 13.5f, 8),
                new Rect(60, 3.25f, 25, 22),
                new Rect(148.5f, 3.25f, 25, 22),//
                new Rect(145,34.5f, 11, 11),
                new Rect(156, 23.5f, 11, 11),
                new Rect(167, 34.5f, 11, 11),
                new Rect(156, 45.5f, 11, 11),
                new Rect(126, 39.5f, 7, 5),
                new Rect(99, 39.5f, 8.5f, 5.5f),
                new Rect(112.5f, 48, 10.5f, 10.5f)
            )},
        };
        
        public static readonly Color[] Colors = new[]
        {
            Color.green,
            Color.blue, 
            Color.yellow, 
            Color.red
        };
        
    }
}