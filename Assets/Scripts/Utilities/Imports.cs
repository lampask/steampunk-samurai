using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class Imports
    {
        public static readonly GameObject PlayerObject = Resources.Load("player") as GameObject;
        public static readonly GameObject BackgroundObject = Resources.Load("background") as GameObject;
        public static readonly List<Sprite> Characters = new List<Sprite>()
        {
            Resources.Load<Sprite>("Characters/one"), 
            Resources.Load<Sprite>("Characters/two"),
            Resources.Load<Sprite>("Characters/three"),
            Resources.Load<Sprite>("Characters/four")
        };
        public static readonly Dictionary<string, GameObject> BuildingObjects = new Dictionary<string, GameObject>
        {
            {"building1", Resources.Load("Objects/building1") as GameObject}
        };
        public static readonly Dictionary<Globals.GamepadType, Dictionary<string, Texture>> Gamepad = new Dictionary<Globals.GamepadType, Dictionary<string, Texture>>()
        {
            {Globals.GamepadType.Xbox, new Dictionary<string, Texture>() {
                {"gamepad", Resources.Load("Debug/Gamepad/Xbox/gamepad") as Texture},
                {"LTrigger", Resources.Load("Debug/Gamepad/Xbox/LTrigger") as Texture},
                {"RTrigger", Resources.Load("Debug/Gamepad/Xbox/RTrigger") as Texture},
                {"LShoulder", Resources.Load("Debug/Gamepad/Xbox/LShoulder") as Texture},
                {"RShoulder", Resources.Load("Debug/Gamepad/Xbox/RShoulder") as Texture},
                {"Back", Resources.Load("Debug/Gamepad/Xbox/Back") as Texture},
                {"Start", Resources.Load("Debug/Gamepad/Xbox/Start") as Texture},
                {"X", Resources.Load("Debug/Gamepad/Xbox/X") as Texture},
                {"Y", Resources.Load("Debug/Gamepad/Xbox/Y") as Texture},
                {"B", Resources.Load("Debug/Gamepad/Xbox/B") as Texture},
                {"A", Resources.Load("Debug/Gamepad/Xbox/A") as Texture},
                {"LStick", Resources.Load("Debug/Gamepad/Xbox/LStick") as Texture},
                {"RStick", Resources.Load("Debug/Gamepad/Xbox/RStick") as Texture},
                {"DPad", Resources.Load("Debug/Gamepad/Xbox/DPad") as Texture},
                {"Guide", Resources.Load("Debug/Gamepad/Xbox/Guide") as Texture},
            }},
            {Globals.GamepadType.Ps3, new Dictionary<string, Texture>() {
                {"gamepad", Resources.Load("Debug/Gamepad/PS3/gamepad") as Texture},
                {"LTrigger", Resources.Load("Debug/Gamepad/PS3/LTrigger") as Texture},
                {"RTrigger", Resources.Load("Debug/Gamepad/PS3/RTrigger") as Texture},
                {"LShoulder", Resources.Load("Debug/Gamepad/PS3/LShoulder") as Texture},
                {"RShoulder", Resources.Load("Debug/Gamepad/PS3/RShoulder") as Texture},
                {"Back", Resources.Load("Debug/Gamepad/PS3/Back") as Texture},
                {"Start", Resources.Load("Debug/Gamepad/PS3/Start") as Texture},
                {"X", Resources.Load("Debug/Gamepad/PS3/X") as Texture},
                {"Y", Resources.Load("Debug/Gamepad/PS3/Y") as Texture},
                {"B", Resources.Load("Debug/Gamepad/PS3/B") as Texture},
                {"A", Resources.Load("Debug/Gamepad/PS3/A") as Texture},
                {"LStick", Resources.Load("Debug/Gamepad/PS3/LStick") as Texture},
                {"RStick", Resources.Load("Debug/Gamepad/PS3/RStick") as Texture},
                {"DPadUp", Resources.Load("Debug/Gamepad/PS3/DPadUp") as Texture},
                {"DPadDown", Resources.Load("Debug/Gamepad/PS3/DPadDown") as Texture},
                {"DPadLeft", Resources.Load("Debug/Gamepad/PS3/DPadLeft") as Texture},
                {"DPadRight", Resources.Load("Debug/Gamepad/PS3/DPadRight") as Texture},
                {"Guide", Resources.Load("Debug/Gamepad/PS3/Guide") as Texture},
            }},
            {Globals.GamepadType.Ps4, new Dictionary<string, Texture>() {
                {"gamepad", Resources.Load("Debug/Gamepad/PS4/gamepad") as Texture},
                {"LTrigger", Resources.Load("Debug/Gamepad/PS4/LTrigger") as Texture},
                {"RTrigger", Resources.Load("Debug/Gamepad/PS4/RTrigger") as Texture},
                {"LShoulder", Resources.Load("Debug/Gamepad/PS4/LShoulder") as Texture},
                {"RShoulder", Resources.Load("Debug/Gamepad/PS4/RShoulder") as Texture},
                {"Back", Resources.Load("Debug/Gamepad/PS4/Back") as Texture},
                {"Start", Resources.Load("Debug/Gamepad/PS4/Start") as Texture},
                {"X", Resources.Load("Debug/Gamepad/PS4/X") as Texture},
                {"Y", Resources.Load("Debug/Gamepad/PS4/Y") as Texture},
                {"B", Resources.Load("Debug/Gamepad/PS4/B") as Texture},
                {"A", Resources.Load("Debug/Gamepad/PS4/A") as Texture},
                {"LStick", Resources.Load("Debug/Gamepad/PS4/LStick") as Texture},
                {"RStick", Resources.Load("Debug/Gamepad/PS4/RStick") as Texture},
                {"DPadUp", Resources.Load("Debug/Gamepad/PS4/DPadUp") as Texture},
                {"DPadDown", Resources.Load("Debug/Gamepad/PS4/DPadDown") as Texture},
                {"DPadLeft", Resources.Load("Debug/Gamepad/PS4/DPadLeft") as Texture},
                {"DPadRight", Resources.Load("Debug/Gamepad/PS4/DPadRight") as Texture},
                {"Guide", Resources.Load("Debug/Gamepad/PS4/Guide") as Texture},
            }},
            {Globals.GamepadType.Generic, new Dictionary<string, Texture>() {
                {"gamepad", Resources.Load("Debug/Gamepad/Generic/gamepad") as Texture},
                {"LTrigger", Resources.Load("Debug/Gamepad/Generic/LTrigger") as Texture},
                {"RTrigger", Resources.Load("Debug/Gamepad/Generic/RTrigger") as Texture},
                {"LShoulder", Resources.Load("Debug/Gamepad/Generic/LShoulder") as Texture},
                {"RShoulder", Resources.Load("Debug/Gamepad/Generic/RShoulder") as Texture},
                {"Back", Resources.Load("Debug/Gamepad/Generic/Back") as Texture},
                {"Start", Resources.Load("Debug/Gamepad/Generic/Start") as Texture},
                {"X", Resources.Load("Debug/Gamepad/Generic/X") as Texture},
                {"Y", Resources.Load("Debug/Gamepad/Generic/Y") as Texture},
                {"B", Resources.Load("Debug/Gamepad/Generic/B") as Texture},
                {"A", Resources.Load("Debug/Gamepad/Generic/A") as Texture},
                {"LStick", Resources.Load("Debug/Gamepad/Generic/LStick") as Texture},
                {"RStick", Resources.Load("Debug/Gamepad/Generic/RStick") as Texture},
                {"DPadUp", Resources.Load("Debug/Gamepad/Generic/DPadUp") as Texture},
                {"DPadDown", Resources.Load("Debug/Gamepad/Generic/DPadDown") as Texture},
                {"DPadLeft", Resources.Load("Debug/Gamepad/Generic/DPadLeft") as Texture},
                {"DPadRight", Resources.Load("Debug/Gamepad/Generic/DPadRight") as Texture},
                {"Guide", Resources.Load("Debug/Gamepad/Generic/Guide") as Texture},
                // Global gamepad imports
                {"Pointer", Resources.Load("Debug/Gamepad/Pointer") as Texture},
            }},
        };

        public static readonly Sprite disabled;
    }
}