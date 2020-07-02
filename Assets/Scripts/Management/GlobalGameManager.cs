using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Console;
using SharpDX.XInput;
using SharpDX.DirectInput;
using Console.Commands;
using Gameplay.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using Discord;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace Management
{
    public class InputEvent : UnityEvent<Control> { }
    public class GlobalGameManager : MonoBehaviour
    {
        public static GlobalGameManager instance;
        
        public GameObject loadingScreen;
        public GameObject loadingSinner;
        public GameObject shade;
        
        public static ActivityManager activityManager;
        public Dictionary<string, Activity> activities;
        public Discord.Discord discord;
        public long startTimestamp;
        
        public static DirectInput dInput;
        public Dictionary<Unid, Control> controls;
        public static InputEvent connectedInput;
        public static InputEvent disconnectedInput;
        public bool inputOverlay { get; set; }

        private Settings settingInstance;

        // Preserve game state 
        [SerializeField] private List<ICommand> gameCommands;
    
        public enum SceneIndexes {
            Manager = 0,
            MenuScreen = 1,
            Game = 2
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    GlobalConsole.Error($"{logString}\n {stackTrace}");
                    break;
                case LogType.Exception:
                    GlobalConsole.Error($"{logString}\n {stackTrace}");
                    break;
                default:
                    GlobalConsole.Log(logString);
                    break;
            }
        }

        private void Awake() {
            if (!instance)
                instance = this;
            else
                Destroy(this);
            
            Globals.settingPath = Application.dataPath + "/settings.samurai";
            startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            inputOverlay = true;
            Cursor.visible = false;
            connectedInput = new InputEvent();
            disconnectedInput = new InputEvent();
            
            // Initialize base classes
            gameCommands = new List<ICommand>();
        
        
            // Setup activities
            activities = new Dictionary<string, Activity>();
            activities.Add("Menu", new Activity {
                State = "Currently not in game",
                Details = "Browsing menu",
                Timestamps = new ActivityTimestamps()
                {                                                            
                    Start = startTimestamp
                },
                Assets = new ActivityAssets()
                {
                    LargeImage = "shield"
                }
            });              
            activities.Add("CHSelection", new Activity {
                State = "Selecting characters",
                Details = $"{1} / 4 are ready",
                Timestamps = new ActivityTimestamps()
                {
                    Start = startTimestamp
                },
                Assets = new ActivityAssets()
                {
                    LargeImage = "shield"
                }
            });
            
            
            // Initialize DirectInput
            dInput = new DirectInput();
            controls = new Dictionary<Unid, Control>();
            
            controls.Add(new Unid("Keyboard"), new Control());
            
            // Load Menu 
            SceneManager.LoadSceneAsync((int) SceneIndexes.MenuScreen, LoadSceneMode.Additive);
        }

        private void SetupDiscord(bool silent)
        {
            try
            {
                discord = new Discord.Discord(722501715622101176, 
                    (ulong) CreateFlags.NoRequireDiscord);
                activityManager = discord.GetActivityManager();
                
                activityManager.UpdateActivity(activities["Menu"], (res) =>
                {
                    if (res == Result.Ok)
                    {
                        Debug.Log("Discord activity successfully updated");
                    }
                });
                discord.SetLogHook(LogLevel.Debug, LogProblemsFunction);
                GlobalConsole.Log("Discord SDK successfully loaded");
            }
            catch (ResultException e)
            {
                if (!silent) GlobalConsole.Error($"Discord SDK couldn't be loaded\n {e}");
            }
        }

        private void LogProblemsFunction(LogLevel level, string message)
        { 
            GlobalConsole.Log($"Discord:{level} - {message}");
        }

        private void Start()
        {
            // Initialise console system
            gameCommands.Add(new QuitCommand());
            gameCommands.Add(new InputDebugCommand());
            
            // Late Initialize (needs preload)
            settingInstance = new Settings();
            SetupDiscord(false);
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
                        var cntr = new Control(controller);
                        controls.Add(new Unid((UserIndex) i), cntr);
                        Debug.Log($"{Input.GetJoystickNames()[(int) controller.UserIndex]} found and connected using xInput");
                        connectedInput.Invoke(cntr);
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
                    var cntr = new Control(stick);
                    controls.Add(new Unid(device.InstanceGuid), cntr);
                    connectedInput.Invoke(cntr);
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
                    disconnectedInput.Invoke(control.Value);
                }
            }
            foreach (var control in delList) { controls.Remove(control); }
            
            // Error checks
            foreach (var control in controls)
            {
                if (control.Value.type != Control.ControlType.XInputController)
                {
                    var others = controls.Where(x => x.Value.type == Control.ControlType.XInputController).ToDictionary(p => p.Key, p => p.Value); // XInput controllers
                    if (!others.FirstOrDefault(y => y.Key.ExtractUserIndex() == (UserIndex) Array.IndexOf(Input.GetJoystickNames(), control.Value.name)).Equals(default(KeyValuePair<Unid,Control>)))
                    {
                        Debug.Log($"Conflicting dInput for {control.Value.name} removed");
                        delList.Add(control.Key);
                        disconnectedInput.Invoke(control.Value);
                    }
                }
            }
            foreach (var control in delList) { controls.Remove(control); }
            
            if (discord == null) SetupDiscord(true);
            else
                try
                {
                    discord.RunCallbacks();
                }
                catch (ResultException)
                {
                    discord.Dispose();
                    discord = null;
                    GlobalConsole.Log("Discord SDK disconnected");
                }
        }

        private void OnGUI()
        {
            var i = 0;
            var scaleFactor = Screen.height/1080f;
            
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

        // SCENE LOADING
        
        List<AsyncOperation> loading = new List<AsyncOperation>();

        public IEnumerator ToggleLoadingScreen(bool toggle, Action after)
        {
            shade.LeanValue(value => {
                shade.GetComponent<Image>().color = value;
            }, new Color32(0, 0, 0, 0), new Color(0,0,0,1), 0.5f).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                loadingScreen.SetActive(toggle);
                if (toggle)
                    // Create tween for loading spinner
                    loadingSinner.LeanRotateAroundLocal(Vector3.forward, -360, 4f).setLoopClamp();
                else
                    LeanTween.cancel(loadingSinner);
                shade.LeanValue(value =>
                {
                    shade.GetComponent<Image>().color = value;
                }, new Color32(0, 0, 0, 1), new Color(0, 0, 0, 0), 0.5f).setIgnoreTimeScale(true);
            });
            yield return new WaitForSeconds(4f);
            after();
        }
        
        public void LoadScene(SceneIndexes current, SceneIndexes target)
        {
            StartCoroutine(ToggleLoadingScreen(true, () =>
            {
                loading.Add(SceneManager.UnloadSceneAsync((int) current));
                loading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.Game, LoadSceneMode.Additive));

                StartCoroutine(GetLoadProgress());
            }));
        }
        
        // default overload
        public void LoadGame() {
            LoadScene(SceneIndexes.MenuScreen, SceneIndexes.Game);
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
            StartCoroutine(ToggleLoadingScreen(false, () => { }));
        }
    }
}
