using System;
using System.Collections;
using System.Collections.Generic;
using Console;
using SharpDX.XInput;
using SharpDX.DirectInput;
using Console.Commands;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Management
{
    public class GlobalGameManager : MonoBehaviour
    {
        public static GlobalGameManager instance;
        public GameObject loadingScreen;
        public static DirectInput dInput;
        private Dictionary<Guid, Joystick> dInputControls;
        public Dictionary<UserIndex, Controller> xInputControls;
        
        public bool inputOverlay { get; set; }
        
        // Preserve game state 
        [SerializeField] private List<ICommand> gameCommands;
    
        public enum SceneIndexes {
            Manager = 0,
            MenuScreen = 1,
            Game = 2
        }

        private void Awake() {
            if (!instance)
                instance = this;
            else
                Destroy(this);
        
            gameCommands = new List<ICommand>();
            inputOverlay = true;
            
            // Load Menu 
            SceneManager.LoadSceneAsync((int) SceneIndexes.MenuScreen, LoadSceneMode.Additive);
        }

        private void Start()
        {
            // Initialise console system
            gameCommands.Add(new QuitCommand());
            gameCommands.Add(new InputDebugCommand());
            
            // Initialize DirectInput
            dInput = new DirectInput();
            dInputControls = new Dictionary<Guid, Joystick>();
            xInputControls = new Dictionary<UserIndex, Controller>();
        }

        private void Update()
        {
            // Connections checks
            
            for (var i = 0; i < 5; i++)
            {
                if (!xInputControls.ContainsKey((UserIndex) i))
                {
                    var controller = new Controller((UserIndex) i);
                    if (controller.IsConnected)
                    {
                        xInputControls.Add((UserIndex) i, controller);
                        Debug.Log($"{Input.GetJoystickNames()[(int) controller.UserIndex]} found and connected using xInput");
                    }
                }
            }

            foreach (var device in dInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
            {
                if (!dInputControls.ContainsKey(device.InstanceGuid) && !xInputControls.ContainsKey((UserIndex) Array.IndexOf(Input.GetJoystickNames(), device.InstanceName))) 
                {
                    var stick = new Joystick(dInput, device.InstanceGuid);
                    stick.Acquire();

                    /*foreach (var deviceObject in stick.GetObjects(DeviceObjectTypeFlags.Axis))
                    {
                        stick.GetObjectPropertiesById(deviceObject.ObjectId).Range = new InputRange(-100, 100);
                    }*/
                    
                    Debug.Log($"{device.InstanceName} found and connected using dInput");
                    dInputControls.Add(device.InstanceGuid, stick);
                }
            }

            // Disconnection checks
            
            var dDelList = new List<Guid>(); 
            foreach (var control in dInputControls)
            {
                if (!dInput.IsDeviceAttached(control.Key))
                {
                    Debug.Log($"{control.Value.Information.InstanceName} disconnected");
                    dDelList.Add(control.Value.Information.InstanceGuid);
                }
            }
            foreach (var control in dDelList) { dInputControls.Remove(control); }

            var xDelList = new List<UserIndex>();
            foreach (var control in xInputControls)
            {
                if (!control.Value.IsConnected)
                {
                    Debug.Log($"{Input.GetJoystickNames()[(int) control.Value.UserIndex]} disconnected");
                    xDelList.Add(control.Key);
                }
            }
            foreach (var control in xDelList) { xInputControls.Remove(control); }
            
            // Error checks
            foreach (var control in dInputControls)
            {
                if (xInputControls.ContainsKey((UserIndex) Array.IndexOf(Input.GetJoystickNames(), control.Value.Information.InstanceName)))
                {
                    Debug.Log($"Conflicting dInput for {control.Value.Information.InstanceName} removed");
                    dDelList.Add(control.Key);
                }
            }
            foreach (var control in dDelList) { dInputControls.Remove(control); }
            
        }

        private void OnGUI()
        {
            var i = 0;
            var scaleFactor = 1;
            
            if (inputOverlay)
            {
                foreach (var control in xInputControls)
                {
                    DrawController(control.Value, 0, i*150*scaleFactor, scaleFactor, Globals.GamepadType.Xbox);
                    i++;
                }

                foreach (var control in dInputControls)
                {
                    DrawController(control.Value, 0, i*150*scaleFactor, scaleFactor, Globals.GamepadType.Ps3);
                    i++;
                }
            }
        }

        private void DrawController(Controller c, float x, float y, float scaleFactor, Globals.GamepadType type) { DrawXInputController(c, x, y, scaleFactor, type); }
        private void DrawController(Joystick j, float x, float y, float scaleFactor, Globals.GamepadType type) { DrawDInputController(j, x, y, scaleFactor, type); }
        
        private void DrawXInputController(Controller c, float x, float y, float scaleFactor, Globals.GamepadType type)
        {
            if (c.IsConnected) {
                var state = c.GetState();
                GUI.DrawTexture(new Rect(x, y,225*scaleFactor, 104*scaleFactor), Imports.Gamepad[type]["gamepad"], ScaleMode.ScaleToFit);
                
                // Determine font size and styles
                var styles = new GUIStyle();
                styles.alignment = TextAnchor.UpperLeft;
                styles.normal.textColor = Color.red;
                styles.fontSize = (int)(10f*scaleFactor);
                var stickRadius = 7.5f;
                
                // Define some useful functions
                Rect SetupBounds(Rect relativeBounds) { return new Rect(new Vector2(x,y) + relativeBounds.position * scaleFactor, relativeBounds.size * scaleFactor); }
                
                // Handle trigger change
                if (Math.Abs(state.Gamepad.LeftTrigger) > 0.01f) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].lTrigger), Imports.Gamepad[type]["LTrigger"], ScaleMode.ScaleToFit); 
                GUI.Label(new Rect(x+11.5f*scaleFactor, y+22.5f*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round((double) state.Gamepad.LeftTrigger, 4)}", styles);
                if (Math.Abs(state.Gamepad.RightTrigger) > 0.01f) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].rTrigger), Imports.Gamepad[type]["RTrigger"], ScaleMode.ScaleToFit); 
                GUI.Label(new Rect(x+193*scaleFactor, y+22.5f*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round((double) state.Gamepad.RightTrigger, 4)}", styles);
                
                // Handle buttons
                if ((((int)state.Gamepad.Buttons >> 8) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].lShoulder), Imports.Gamepad[type]["LShoulder"], ScaleMode.ScaleToFit); 
                if ((((int)state.Gamepad.Buttons >> 9) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].rShoulder), Imports.Gamepad[type]["RShoulder"], ScaleMode.ScaleToFit); 
                if ((((int)state.Gamepad.Buttons >> 5) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].back), Imports.Gamepad[type]["Back"], ScaleMode.ScaleToFit); 
                if ((((int)state.Gamepad.Buttons >> 4) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].start), Imports.Gamepad[type]["Start"], ScaleMode.ScaleToFit); 
                if ((((int)state.Gamepad.Buttons >> 14) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].x), Imports.Gamepad[type]["X"], ScaleMode.ScaleToFit); 
                if ((((int)state.Gamepad.Buttons >> 15) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].y), Imports.Gamepad[type]["Y"], ScaleMode.ScaleToFit); 
                if ((((int)state.Gamepad.Buttons >> 13) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].b), Imports.Gamepad[type]["B"], ScaleMode.ScaleToFit); 
                if ((((int)state.Gamepad.Buttons >> 12) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].a), Imports.Gamepad[type]["A"], ScaleMode.ScaleToFit); 
                //if (state.Gamepad.Buttons == GamepadButtonFlags.) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].guide), Imports.Gamepad[type]["Guide"], ScaleMode.ScaleToFit);

                // Handle stick change
                var leftStick = new Vector2(x: state.Gamepad.LeftThumbX/1000f, y: state.Gamepad.LeftThumbY/1000f);
                if (Math.Abs(leftStick.x) > 2f || Math.Abs(leftStick.y) > 2f || (((int)state.Gamepad.Buttons >> 6) & 1) == 1)
                {
                    GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].lStick), Imports.Gamepad[type]["LStick"], ScaleMode.ScaleToFit);
                    var product = Globals.GamepadProps[type].lStickCenter + stickRadius * new Vector2(leftStick.x, -leftStick.y).normalized;
                    GUI.DrawTexture(new Rect(x+product.x*scaleFactor,y+product.y*scaleFactor, 3.5f*scaleFactor, 3.5f*scaleFactor), Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                }
                GUI.Label(new Rect(x+11.5f*scaleFactor, y+48*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round(leftStick.x, 4)}\n{Math.Round(leftStick.y, 4)}", styles);
                
                var rightStick = new Vector2(x: state.Gamepad.RightThumbX/1000f, y: state.Gamepad.RightThumbY/1000f);
                if (Math.Abs(rightStick.x) > 2f || Math.Abs(rightStick.y) > 2f || (((int)state.Gamepad.Buttons >> 7) & 1) == 1)
                {
                    GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].rStick), Imports.Gamepad[type]["RStick"], ScaleMode.ScaleToFit);
                    var product = Globals.GamepadProps[type].rStickCenter + stickRadius * new Vector2(rightStick.x, -rightStick.y).normalized;
                    GUI.DrawTexture(new Rect(x+product.x*scaleFactor,y+product.y*scaleFactor, 3.5f*scaleFactor, 3.5f*scaleFactor), Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                }
                GUI.Label(new Rect(x+193*scaleFactor, y+66*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round(rightStick.x, 4)}\n{Math.Round(rightStick.y, 4)}", styles);

                // Handle dPad change
                if (!Globals.GamepadProps[type].compositeDPad) {
                    if ((((int)state.Gamepad.Buttons >> 0) & 1) == 1 || 
                        (((int)state.Gamepad.Buttons >> 1) & 1) == 1 ||
                        (((int)state.Gamepad.Buttons >> 2) & 1) == 1 ||
                        (((int)state.Gamepad.Buttons >> 3) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].dPad), Imports.Gamepad[type]["DPad"], ScaleMode.ScaleToFit);
                }
                if ((((int)state.Gamepad.Buttons >> 0) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].up), Globals.GamepadProps[type].compositeDPad ? Imports.Gamepad[type]["DPadUp"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                if ((((int)state.Gamepad.Buttons >> 1) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].down), Globals.GamepadProps[type].compositeDPad ? Imports.Gamepad[type]["DPadDown"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                if ((((int)state.Gamepad.Buttons >> 2) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].left), Globals.GamepadProps[type].compositeDPad ? Imports.Gamepad[type]["DPadLeft"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                if ((((int)state.Gamepad.Buttons >> 3) & 1) == 1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].right), Globals.GamepadProps[type].compositeDPad ? Imports.Gamepad[type]["DPadRight"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                GUI.Label(new Rect(x+11.5f*scaleFactor, y+78*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{((((int)state.Gamepad.Buttons >> 0) & 1) == 1 ? 1 : 0)} {((((int)state.Gamepad.Buttons >> 1) & 1) == 1 ? 1 : 0)} {((((int)state.Gamepad.Buttons >> 2) & 1) == 1 ? 1 : 0)} {((((int)state.Gamepad.Buttons >> 3) & 1) == 1 ? 1 : 0)}", styles);
            }
            GUI.Label(new Rect(x+5*scaleFactor, y+90*scaleFactor, 200*scaleFactor, 35*scaleFactor), $"{Input.GetJoystickNames()[(int)c.UserIndex]}: {(c.IsConnected ? "Connected" : "Disconnected")} Packet #{(c.IsConnected ? c.GetState().PacketNumber.ToString() : "???")}\n");
        }

        private void DrawDInputController(Joystick j, float x, float y, float scaleFactor, Globals.GamepadType type)
        {
            if (dInput.IsDeviceAttached(j.Information.InstanceGuid))
            {
                var state = j.GetCurrentState();
                GUI.DrawTexture(new Rect(x, y,225*scaleFactor, 104*scaleFactor), Imports.Gamepad[type]["gamepad"], ScaleMode.ScaleToFit);
                 
                // Determine font size and styles
                var styles = new GUIStyle();
                styles.alignment = TextAnchor.UpperLeft;
                styles.normal.textColor = Color.red;
                styles.fontSize = (int)(10f*scaleFactor);
                var stickRadius = 7.5f;
                
                // Define some useful functions
                Rect SetupBounds(Rect relativeBounds) { return new Rect(new Vector2(x,y) + relativeBounds.position * scaleFactor, relativeBounds.size * scaleFactor); }
                
                // In case triggers are as buttons
                if (state.Buttons[6]) state.RotationX = 100;
                if (state.Buttons[7]) state.RotationY = 100;
                
                // Handle trigger change
                if (Math.Abs(state.RotationX) > 1f) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].lTrigger), Imports.Gamepad[type]["LTrigger"], ScaleMode.ScaleToFit); 
                GUI.Label(new Rect(x+11.5f*scaleFactor, y+22.5f*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round((double) state.RotationX, 4)}", styles);
                if (Math.Abs(state.RotationY) > 1f) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].rTrigger), Imports.Gamepad[type]["RTrigger"], ScaleMode.ScaleToFit); 
                GUI.Label(new Rect(x+193*scaleFactor, y+22.5f*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round((double) state.RotationY, 4)}", styles);

                // Handle buttons
                if (state.Buttons[4]) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].lShoulder), Imports.Gamepad[type]["LShoulder"], ScaleMode.ScaleToFit); 
                if (state.Buttons[5]) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].rShoulder), Imports.Gamepad[type]["RShoulder"], ScaleMode.ScaleToFit); 
                if (state.Buttons[8]) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].back), Imports.Gamepad[type]["Back"], ScaleMode.ScaleToFit); 
                if (state.Buttons[9]) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].start), Imports.Gamepad[type]["Start"], ScaleMode.ScaleToFit); 
                if (state.Buttons[0]) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].x), Imports.Gamepad[type]["X"], ScaleMode.ScaleToFit); 
                if (state.Buttons[3]) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].y), Imports.Gamepad[type]["Y"], ScaleMode.ScaleToFit); 
                if (state.Buttons[2]) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].b), Imports.Gamepad[type]["B"], ScaleMode.ScaleToFit); 
                if (state.Buttons[1]) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].a), Imports.Gamepad[type]["A"], ScaleMode.ScaleToFit); 
                if (state.Buttons[12]) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].guide), Imports.Gamepad[type]["Guide"], ScaleMode.ScaleToFit);
                // PS4 screen thingy -> 13
                
                // Handle stick change
                var leftStick = new Vector2(x: (state.X-32767)/1000f, y: -(state.Y-32767)/1000f);
                if (Math.Abs(leftStick.x) > 2f || Math.Abs(leftStick.y) > 2f || state.Buttons[10])
                {
                    GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].lStick), Imports.Gamepad[type]["LStick"], ScaleMode.ScaleToFit);
                    var product = Globals.GamepadProps[type].lStickCenter + stickRadius * new Vector2(leftStick.x, -leftStick.y).normalized;
                    GUI.DrawTexture(new Rect(x+product.x*scaleFactor,y+product.y*scaleFactor, 3.5f*scaleFactor, 3.5f*scaleFactor), Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                }
                GUI.Label(new Rect(x+11.5f*scaleFactor, y+48*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round(leftStick.x, 4)}\n{Math.Round(leftStick.y, 4)}", styles);
                
                var rightStick = new Vector2(x: (state.Z-32767)/1000f, y: -(state.RotationZ-32767)/1000f);
                if (Math.Abs(rightStick.x) > 2f || Math.Abs(rightStick.y) > 2f || state.Buttons[11])
                {
                    GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].rStick), Imports.Gamepad[type]["RStick"], ScaleMode.ScaleToFit);
                    var product = Globals.GamepadProps[type].rStickCenter + stickRadius * new Vector2(rightStick.x, -rightStick.y).normalized;
                    GUI.DrawTexture(new Rect(x+product.x*scaleFactor,y+product.y*scaleFactor, 3.5f*scaleFactor, 3.5f*scaleFactor), Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                }
                GUI.Label(new Rect(x+193*scaleFactor, y+66*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round(rightStick.x, 4)}\n{Math.Round(rightStick.y, 4)}", styles);

                var p = state.PointOfViewControllers[0];
                // Handle dPad change
                if (!Globals.GamepadProps[type].compositeDPad) {
                    if (p != -1) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].dPad), Imports.Gamepad[type]["DPad"], ScaleMode.ScaleToFit);
                }
                
                // 0        =>    0
                // 4500     =>    1000110010100    
                // 9000     =>    10001100101000
                // 13500    =>    11010010111100
                // 18000    =>    100011001010000
                // 22500    =>    101011111100100
                // 27000    =>    110100101111000
                // 31500    =>    111101100001100
                
                if (p == 0 || p == 4500 || p == 31500) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].up), Globals.GamepadProps[type].compositeDPad ? Imports.Gamepad[type]["DPadUp"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                if (p == 18000 || p == 22500 || p == 13500) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].down), Globals.GamepadProps[type].compositeDPad ? Imports.Gamepad[type]["DPadDown"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                if (p == 27000 || p == 31500 || p == 22500) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].left), Globals.GamepadProps[type].compositeDPad ? Imports.Gamepad[type]["DPadLeft"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                if (p == 9000 || p == 13500 || p == 4500) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[type].right), Globals.GamepadProps[type].compositeDPad ? Imports.Gamepad[type]["DPadRight"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
                GUI.Label(new Rect(x + 11.5f * scaleFactor, y + 78 * scaleFactor, 42 * scaleFactor, 20 * scaleFactor), $"{(p == 0 || p == 4500 || p == 31500 ? 1 : 0)} {(p == 18000 || p == 22500 || p == 13500 ? 1 : 0)} {(p == 27000 || p == 31500 || p == 22500 ? 1 : 0)} {(p == 9000 || p == 13500 || p == 4500 ? 1 : 0)}", styles);
            }
            GUI.Label(new Rect(x+5*scaleFactor, y+90*scaleFactor, 500*scaleFactor, 50*scaleFactor), $"{j.Information.InstanceName}: {(dInput.IsDeviceAttached(j.Information.InstanceGuid) ? "Connected" : "Disconnected")} Packet #{(dInput.IsDeviceAttached(j.Information.InstanceGuid) ? "NaN" : "???")}\n");
        }
        
        public void ToggleInput()
        {
            inputOverlay = !inputOverlay;
        }

        List<AsyncOperation> loading = new List<AsyncOperation>(); 

        public void LoadScene(SceneIndexes current, SceneIndexes target) {
            loadingScreen.SetActive(true);

            loading.Add(SceneManager.UnloadSceneAsync((int) current));
            loading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.Game, LoadSceneMode.Additive));

            StartCoroutine(GetLoadProgress());
        }
        // default overload
        public void LoadGame() {
            LoadScene(SceneIndexes.MenuScreen, SceneIndexes.Game);
        }
        public void LoadCharacterSelection() {
        
        }
        public float totalLoadingProgress;
        public IEnumerator GetLoadProgress() {
            for(var i = 0; i<loading.Count; i++) {
                while (!loading[i].isDone) {
                    totalLoadingProgress = 0;

                    foreach(var operation in loading) {
                        totalLoadingProgress += operation.progress;
                    }

                    totalLoadingProgress /= loading.Count;

                    // Update progress

                    yield return null;
                }
            }
            loading.Clear();
            loadingScreen.SetActive(false);
        }
    }
}
