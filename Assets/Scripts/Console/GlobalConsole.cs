using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Console {
    public class GlobalConsole : MonoBehaviour {
        public enum ConsoleMode
        {
            OneTime = 0,
            Execution = 1
        }
        public new static bool enabled { get; private set; }
        public static ConsoleMode mode { get; private set; }
        public static GlobalConsole instance {get; private set;}
        public static List<string> history {get; protected set;}
        public static Dictionary<string, AbstractCommand> commands {get; private set;}
        
        [Header("UI")]
        public Canvas consoleCanvas;
        public TMP_InputField consoleInput;
        public GameObject scrollObject;
        private static ScrollRect scrollRect;
        public GameObject consoleTextObject;
        private static TMP_Text consoleLog;

        [Header("Controls")] 
        public KeyCode mainTrigger = KeyCode.BackQuote;
        public KeyCode stayTrigger = KeyCode.LeftShift;

        private RectTransform trans;
        private static RectTransform logTrans;
        
        // Start is called before the first frame update
        private void Awake()
        {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(this);
            }
            
            history = new List<string>();
            commands = new Dictionary<string, AbstractCommand>();
            scrollRect = scrollObject.GetComponent<ScrollRect>();
            trans = scrollObject.GetComponent<RectTransform>();
            consoleLog = consoleTextObject.GetComponent<TMP_Text>();
            logTrans = consoleTextObject.GetComponent<RectTransform>();
            
            ///////////////
            // LISTENERS //
            ///////////////
            
            // Select console
            consoleInput.onSelect.AddListener((arg0) =>
            {
                consoleInput.placeholder.gameObject.SetActive(false);
                Compact(false);
                
            });
            
            // Deselect console
            consoleInput.onDeselect.AddListener((arg0) =>
            {
                consoleInput.placeholder.gameObject.SetActive(true);
                Compact(true);
            });
            
            // Command execute
            consoleInput.onSubmit.AddListener((arg0) =>
            {
                consoleInput.text = "";
                if (arg0 != "")
                {
                    var args = arg0.Split(null);
                    if (args.Length != 0)
                    {
                        if (commands.ContainsKey(args[0]))
                        {
                            commands[args[0]].process.Invoke(args);
                            Debug.Log($"Executing command {arg0}");
                        }
                        else
                        {
                            Error($"The command '{args[0]}' doesn't exist");
                        }
                    }
                }
                var eventSystem = EventSystem.current;
                if (!eventSystem.alreadySelecting) eventSystem.SetSelectedGameObject(null);
                if (mode == ConsoleMode.OneTime) HideConsole();
                consoleInput.onDeselect.Invoke("");
            });
        }

        private void Start()
        {
            // call before font object is disabled
            AdjustLogToFont(); 
            HideConsole();
        }
        
        // Update is called once per frame
        private async void Update()
        {
            if (Input.GetKeyDown(mainTrigger))
            {
                if (enabled)
                {
                    if (mode == ConsoleMode.Execution)
                    {
                        if (!Input.GetKey(stayTrigger))
                        {
                            await EnableEdit();
                            return;
                        }
                    }
                    HideConsole();
                }
                else
                {
                    OpenConsole(!Input.GetKey(stayTrigger));
                    await EnableEdit();
                }
            }
        }

        private async Task EnableEdit()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            consoleInput.Select();
            consoleInput.ActivateInputField();
        }
        
        private static void Send(string msg)
        {
            consoleLog.text += msg;
            Debug.Log($"[InGame Console] ==> {msg}");
            history.Add(msg);
            AdjustLogToFont();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        
        public static void Log(string msg)
        {
            Send($"{msg}\n");
        }
        public static void Error(string msg) {
            Send($"[Error] {msg}\n");
        }
        private static void AdjustLogToFont()
        {
            logTrans.sizeDelta = new Vector2(logTrans.sizeDelta.x, history.Count * consoleLog.font.faceInfo.lineHeight);
        }
        private void OpenConsole(bool value)
        {
            consoleCanvas.gameObject.SetActive(true);
            mode = value ? ConsoleMode.OneTime : ConsoleMode.Execution;
            enabled = true;
        }
        private void HideConsole()
        {
            consoleCanvas.gameObject.SetActive(false);
            enabled = false;
            consoleInput.text = "";
        }
        private void Compact(bool value)
        {
            var sizeDelta = trans.sizeDelta;
            sizeDelta = value ? new Vector2(sizeDelta.x, 50) : new Vector2(sizeDelta.x, 200);
            trans.sizeDelta = sizeDelta;
        }
        public void AddCommand(string commandKeyphrase, AbstractCommand command)
        {
            if (!commands.ContainsKey(commandKeyphrase))
            {
                commands.Add(commandKeyphrase, command);
                Debug.Log($"Initialised {command.name}");
            }
        }
    }
}
