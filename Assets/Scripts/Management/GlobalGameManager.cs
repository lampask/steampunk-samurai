using System;
using System.Collections;
using System.Collections.Generic;
using Console;
using SharpDX.XInput;
using SharpDX.DirectInput;
using Console.Commands;
using Gameplay.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    public class GlobalGameManager : MonoBehaviour
    {
        public static GlobalGameManager instance;
        public GameObject loadingScreen;
        public static DirectInput dInput;
        public Dictionary<Unid, Control> controls;
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
            controls = new Dictionary<Unid, Control>();
        }

        private void Update()
        {
            // Connections checks
            
            for (var i = 0; i < 5; i++)
            {
                if (!controls.ContainsKey(new Unid((UserIndex) i)))
                {
                    var controller = new Controller((UserIndex) i);
                    if (controller.IsConnected)
                    {
                        controls.Add(new Unid((UserIndex) i), new Control(controller));
                        Debug.Log($"{Input.GetJoystickNames()[(int) controller.UserIndex]} found and connected using xInput");
                    }
                }
            }

            foreach (var device in dInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
            {
                if (!controls.ContainsKey(new Unid(device.InstanceGuid)) && !controls.ContainsKey(new Unid((UserIndex) Array.IndexOf(Input.GetJoystickNames(), device.InstanceName))))
                {
                    var stick = new Joystick(dInput, device.InstanceGuid);
                    stick.Acquire();

                    /*foreach (var deviceObject in stick.GetObjects(DeviceObjectTypeFlags.Axis))
                    {
                        stick.GetObjectPropertiesById(deviceObject.ObjectId).Range = new InputRange(-100, 100);
                    }*/
                    
                    Debug.Log($"{device.InstanceName} found and connected using dInput");
                    controls.Add(new Unid(device.InstanceGuid), new Control(stick));
                }
            }

            // Disconnection checks
            
            var delList = new List<Unid>(); 
            foreach (var control in controls)
            {
                if (!control.Value.attached)
                {
                    Debug.Log($"{control.Value.name} disconnected");
                    delList.Add(control.Key);
                }
            }
            foreach (var control in delList) { controls.Remove(control); }
            
            // Error checks
            foreach (var control in controls)
            {
                if (!control.Value.isXInput)
                {
                    if (controls.ContainsKey(new Unid((UserIndex) Array.IndexOf(Input.GetJoystickNames(),
                        control.Value.name))))
                    {
                        Debug.Log($"Conflicting dInput for {control.Value.name} removed");
                        delList.Add(control.Key);
                    }
                }
            }
            foreach (var control in delList) { controls.Remove(control); }
            
        }

        private void OnGUI()
        {
            var i = 0;
            var scaleFactor = 1;
            
            if (inputOverlay)
            {
                foreach (var control in controls)
                {
                    control.Value.DrawControl(0, i*150*scaleFactor, scaleFactor);
                    i++;
                }
            }
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
