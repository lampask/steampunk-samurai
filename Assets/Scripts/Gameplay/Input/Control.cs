using System;
using Management;
using SharpDX.DirectInput;
using SharpDX.XInput;
using UnityEngine;
using Utilities;

namespace Gameplay.Input
{
    public class Control
    {
        public enum ControlType
        {
            XInputController,
            DInputController,
            Keyboard
        }
        public ControlType type { get; private set; }
        public Controller xControl { get; private set; }
        public Joystick dControl { get; private set; }

        public Globals.GamepadType debugType;

        public Control(Controller xControl)
        {
            type = ControlType.XInputController;
            this.xControl = xControl;
        }

        public Control(Joystick dControl)
        {
            type = ControlType.DInputController;
            this.dControl = dControl;
        }
        
        public string name
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    return UnityEngine.Input.GetJoystickNames()[(int) xControl.UserIndex];
                }
                else
                {
                    return dControl.Information.InstanceName;
                }
            }
        }

        public bool attached
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    return xControl.IsConnected;
                }
                else
                {
                    return GlobalGameManager.dInput.IsDeviceAttached(dControl.Information.InstanceGuid);
                }
            }
        }

        /////////////////// CONTROLS ///////////////////

        public class DPad
        {
            public bool up { get; private set; }
            public bool down { get; private set; }
            public bool left { get; private set; }
            public bool right { get; private set; }

            public DPad(bool up, bool down, bool left, bool right)
            {
                this.up = up;
                this.down = down;
                this.left = left;
                this.right = right;
            }

            public bool any => up || down || left || right;
        }
        public Vector2 rightStick
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return new Vector2(state.Gamepad.RightThumbX / 1000f, y: state.Gamepad.RightThumbY / 1000f);
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return new Vector2(x: (state.Z-32767)/1000f, y: -(state.RotationZ-32767)/1000f);
                }
            }
        }

        public bool rightStickButton
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int)state.Gamepad.Buttons >> 7) & 1) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[11];
                }
            }
        }
        public Vector2 leftStick
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return new Vector2(state.Gamepad.LeftThumbX / 1000f, y: state.Gamepad.LeftThumbY / 1000f);
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return new Vector2(x: (state.X-32767)/1000f, y: -(state.Y-32767)/1000f);
                }
            }
        }

        public bool leftStickButton
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int)state.Gamepad.Buttons >> 6) & 1) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[10];
                }
            }
        }
        public float rightTrigger
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return state.Gamepad.RightTrigger;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    var trigger = (float) state.RotationY;
                    if (Math.Abs(trigger) < 0.02f)
                        trigger = (state.Buttons[7]) ? 100f : 0f;
                    return trigger;
                }
            }
        }
        public float leftTrigger
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return state.Gamepad.LeftTrigger;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    var trigger = (float) state.RotationX;
                    if (Math.Abs(trigger) < 0.02f)
                        trigger = (state.Buttons[6]) ? 100f : 0f;
                    return trigger;
                }
            }
        }

        public DPad dPad
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return new DPad((((int) state.Gamepad.Buttons >> 0) & 1) == 1,
                                    (((int) state.Gamepad.Buttons >> 1) & 1) == 1,
                                    (((int) state.Gamepad.Buttons >> 2) & 1) == 1,
                                    (((int) state.Gamepad.Buttons >> 3) & 1) == 1);
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    var p = state.PointOfViewControllers[0];
                    return new DPad(p == 0 || p == 4500 || p == 31500,
                                    p == 18000 || p == 22500 || p == 13500,
                                    p == 27000 || p == 31500 || p == 22500,
                                    p == 9000 || p == 13500 || p == 4500);
                }
            }
        }

        public bool leftShoulder
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int) state.Gamepad.Buttons >> 8) & 1) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[4];
                }
            }
        }

        public bool rightShoulder
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int) state.Gamepad.Buttons >> 9) & 1) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[5];
                }
            }
        }

        public bool back
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int) state.Gamepad.Buttons >> 5) & 1) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[8];
                }
            }
        }

        public bool start
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int) state.Gamepad.Buttons >> 4) & 1) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[9];
                }
            }
        }
        
        public bool x
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int)state.Gamepad.Buttons >> 8) & 14) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[0];
                }
            }
        }
        
        public bool y
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int)state.Gamepad.Buttons >> 15) & 1) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[3];
                }
            }
        }
        
        public bool a
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int)state.Gamepad.Buttons >> 13) & 1) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[2];
                }
            }
        }
        
        public bool b
        {
            get
            {
                if (type == ControlType.XInputController)
                {
                    var state = xControl.GetState();
                    return (((int)state.Gamepad.Buttons >> 12) & 1) == 1;
                }
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[1];
                }
            }
        }
        
        public bool guide
        {
            get
            {
                if (type == ControlType.XInputController)
                    return false;
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[12];
                }
            }
        }
        
        public bool display
        {
            get
            {
                if (type == ControlType.XInputController)
                    return false;
                else
                {
                    var state = dControl.GetCurrentState();
                    return state.Buttons[13];
                }
            }
        }
        
        /////// DRAW ////////

        public void DrawControl(float x, float y, float scaleFactor)
        {
            GUI.DrawTexture(new Rect(x, y,225*scaleFactor, 104*scaleFactor), Imports.Gamepad[debugType]["gamepad"], ScaleMode.ScaleToFit);
                 
            // Determine font size and styles
            var styles = new GUIStyle();
            styles.alignment = TextAnchor.UpperLeft;
            styles.normal.textColor = Color.red;
            styles.fontSize = (int)(10f*scaleFactor);
            
            var stickRadius = 7.5f;
                
            // Define some useful functions
            Rect SetupBounds(Rect relativeBounds) { return new Rect(new Vector2(x, y) + relativeBounds.position * scaleFactor, relativeBounds.size * scaleFactor); }

            // Handle trigger change
            if (Math.Abs(leftTrigger) > 1f) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].lTrigger), Imports.Gamepad[debugType]["LTrigger"], ScaleMode.ScaleToFit); 
            GUI.Label(new Rect(x+11.5f*scaleFactor, y+22.5f*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round(leftTrigger, 4)}", styles);
            if (Math.Abs(rightTrigger) > 1f) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].rTrigger), Imports.Gamepad[debugType]["RTrigger"], ScaleMode.ScaleToFit); 
            GUI.Label(new Rect(x+193*scaleFactor, y+22.5f*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round(rightTrigger, 4)}", styles); 
            
            // Handle buttons
            if (leftShoulder) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].lShoulder), Imports.Gamepad[debugType]["LShoulder"], ScaleMode.ScaleToFit); 
            if (rightShoulder) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].rShoulder), Imports.Gamepad[debugType]["RShoulder"], ScaleMode.ScaleToFit); 
            if (back) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].back), Imports.Gamepad[debugType]["Back"], ScaleMode.ScaleToFit); 
            if (start) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].start), Imports.Gamepad[debugType]["Start"], ScaleMode.ScaleToFit); 
            if (this.x) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].x), Imports.Gamepad[debugType]["X"], ScaleMode.ScaleToFit); 
            if (this.y) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].y), Imports.Gamepad[debugType]["Y"], ScaleMode.ScaleToFit); 
            if (b) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].b), Imports.Gamepad[debugType]["B"], ScaleMode.ScaleToFit); 
            if (a) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].a), Imports.Gamepad[debugType]["A"], ScaleMode.ScaleToFit); 
            if (guide) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].guide), Imports.Gamepad[debugType]["Guide"], ScaleMode.ScaleToFit);
            if (display) { /* Not implemented */ }
            
            // Handle stick change
            if (Math.Abs(leftStick.x) > 2f || Math.Abs(leftStick.y) > 2f || leftStickButton)
            {
                GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].lStick), Imports.Gamepad[debugType]["LStick"], ScaleMode.ScaleToFit);
                var product = Globals.GamepadProps[debugType].lStickCenter + stickRadius * new Vector2(leftStick.x, -leftStick.y).normalized;
                GUI.DrawTexture(new Rect(x+product.x*scaleFactor,y+product.y*scaleFactor, 3.5f*scaleFactor, 3.5f*scaleFactor), Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
            }
            GUI.Label(new Rect(x+11.5f*scaleFactor, y+48*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round(leftStick.x, 4)}\n{Math.Round(leftStick.y, 4)}", styles);
            
            if (Math.Abs(rightStick.x) > 2f || Math.Abs(rightStick.y) > 2f || rightStickButton)
            {
                GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].rStick), Imports.Gamepad[debugType]["RStick"], ScaleMode.ScaleToFit);
                var product = Globals.GamepadProps[debugType].rStickCenter + stickRadius * new Vector2(rightStick.x, -rightStick.y).normalized;
                GUI.DrawTexture(new Rect(x+product.x*scaleFactor,y+product.y*scaleFactor, 3.5f*scaleFactor, 3.5f*scaleFactor), Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
            }
            GUI.Label(new Rect(x+193*scaleFactor, y+66*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{Math.Round(rightStick.x, 4)}\n{Math.Round(rightStick.y, 4)}", styles);

            // Handle dPad change
            if (!Globals.GamepadProps[debugType].compositeDPad) {
                if (dPad.any) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].dPad), Imports.Gamepad[debugType]["DPad"], ScaleMode.ScaleToFit);
            }
            if (dPad.up) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].up), Globals.GamepadProps[debugType].compositeDPad ? Imports.Gamepad[debugType]["DPadUp"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
            if (dPad.down) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].down), Globals.GamepadProps[debugType].compositeDPad ? Imports.Gamepad[debugType]["DPadDown"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
            if (dPad.left) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].left), Globals.GamepadProps[debugType].compositeDPad ? Imports.Gamepad[debugType]["DPadLeft"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
            if (dPad.right) GUI.DrawTexture(SetupBounds(Globals.GamepadProps[debugType].right), Globals.GamepadProps[debugType].compositeDPad ? Imports.Gamepad[debugType]["DPadRight"] : Imports.Gamepad[Globals.GamepadType.Generic]["Pointer"], ScaleMode.ScaleToFit);
            GUI.Label(new Rect(x+11.5f*scaleFactor, y+78*scaleFactor, 42*scaleFactor, 20*scaleFactor), $"{(dPad.up ? 1 : 0)} {(dPad.down ? 1 : 0)} {(dPad.left ? 1 : 0)} {(dPad.right ? 1 : 0)}", styles);
            
            GUI.Label(new Rect(x+5*scaleFactor, y+90*scaleFactor, 500*scaleFactor, 50*scaleFactor), $"{name}: {(attached ? "Connected" : "Disconnected")} \n Packet: {(type == ControlType.XInputController ? xControl.GetState().PacketNumber.ToString() : "Not Supported")}");
        }
    }
}