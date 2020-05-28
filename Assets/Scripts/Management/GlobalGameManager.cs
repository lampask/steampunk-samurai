using System;
using System.Collections;
using System.Collections.Generic;
using Console;
using Console.Commands;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

namespace Management
{
    public class GlobalGameManager : MonoBehaviour
    {
        public static GlobalGameManager instance;
        public GameObject loadingScreen;

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
        }

        private void OnGUI()
        {
            if (inputOverlay)
            {
                var state = GamePad.GetState(0);
                var text = $"IsConnected {state.IsConnected} Packet #{state.PacketNumber}\n";
                text += $"\tTriggers {state.Triggers.Left} {state.Triggers.Right}\n";
                text += $"\tD-Pad {state.DPad.Up} {state.DPad.Right} {state.DPad.Down} {state.DPad.Left}\n";
                text += $"\tButtons Start {state.Buttons.Start} Back {state.Buttons.Back} Guide {state.Buttons.Guide}\n";
                text += $"\tButtons LeftStick {state.Buttons.LeftStick} RightStick {state.Buttons.RightStick} LeftShoulder {state.Buttons.LeftShoulder} RightShoulder {state.Buttons.RightShoulder}\n";
                text += $"\tButtons A {state.Buttons.A} B {state.Buttons.B} X {state.Buttons.X} Y {state.Buttons.Y}\n";
                text += $"\tSticks Left {state.ThumbSticks.Left.X} {state.ThumbSticks.Left.Y} Right {state.ThumbSticks.Right.X} {state.ThumbSticks.Right.Y}\n";
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text);
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
