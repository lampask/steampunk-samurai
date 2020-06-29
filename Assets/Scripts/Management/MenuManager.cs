using System;
using System.Linq;
using Console;
using Discord;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Management
{
    public class MenuManager : MonoBehaviour
    {
        public class SelectionEvent : UnityEvent<int> { }
        public enum MenuStages
        {
            Menu,
            CharacterSelection,
            ArenaSelection
        }
        public static MenuManager instance;
        [SerializeField] private float scaleAmount = 1.4f;
        private Button previousButton;
        public GameObject defaultButton;
        private bool characterMode;
        public bool[] charactersAvailable;
        public bool[] colorsAvailable;
        public SelectionEvent playerConfirmed;
        public SelectionEvent playerUnconfirmed;
        public int confirmed { get; private set; } 
        public MenuStages stage { get; private set; } = MenuStages.Menu;
        
        [Serializable]
        public class MenuPart
        {
            public string name;
            public GameObject obj;

            public MenuPart(string name)
            {
                this.name = name;
            }
        }

        public MenuPart[] menuComponents = new MenuPart[]
        {
            new MenuPart("Logo"),
            new MenuPart("Version"), 
            new MenuPart("Mask1"),
            new MenuPart("Mask2"),
            new MenuPart("Mask3"),
            new MenuPart("Mask4"),
            new MenuPart("Menu"), 
            new MenuPart("Moon"),
            new MenuPart("Lights"), 
            new MenuPart("Container"), 
            new MenuPart("Settings"),
            new MenuPart("Lights2"),
            new MenuPart("AllContainer"), 
        };
        
        [Serializable]
        public class Binding {
            public Button button;
            public Button.ButtonClickedEvent binding;
            public Binding()
            {
                if (button != null)
                {
                    var onClick = button.onClick;
                    binding = onClick != null ? onClick : new Button.ButtonClickedEvent();
                }
            }
        }
        public Binding[] bindings;
        public Selection[] selections = new Selection[4];
        
        private void Awake() {
            if (!instance)
                instance = this;
            else
                Destroy(this);
            
            charactersAvailable = Enumerable.Repeat(true, 4).ToArray();
            colorsAvailable = Enumerable.Repeat(true, 4).ToArray();
            playerConfirmed = new SelectionEvent();
            playerUnconfirmed = new SelectionEvent();
            playerConfirmed.AddListener((id) =>
            {
                confirmed++;
                if (confirmed >= 2)
                    menuComponents[9].obj.transform.GetChild(2).gameObject.SetActive(true);
            });
            playerUnconfirmed.AddListener((id) =>
            {
                confirmed--;
                if (confirmed < 2)
                    menuComponents[9].obj.transform.GetChild(2).gameObject.SetActive(false);
            });
            selections = menuComponents[9].obj.GetComponentsInChildren<Selection>();
            LeanTween.value(menuComponents[9].obj.transform.GetChild(2).gameObject,value =>
                {
                    menuComponents[9].obj.transform.GetChild(2).gameObject.GetComponentsInChildren<TMP_Text>().ToList().ForEach(t => t.fontSize = value);
                }, 36, 37, 0.4f).setLoopPingPong();
        }

        private void Start() {
            foreach(var bd in bindings) {
                if (bd.binding.GetPersistentEventCount() <= 1) bd.binding.AddListener(Default);
                bd.button.onClick = bd.binding;
            }

            if (EventSystem.current != null)
            {
                if (defaultButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(defaultButton);
                }
            }
            else
            {
                Debug.LogWarning("This scene doesn't contain event system! Functionality might be reduced. try running persistent scene as root.");
            }
            
            // Set Input selections
            var start = GlobalGameManager.instance.controls.Count >= 4 ? 1 : 0;
            var id = start;
            foreach (var control in GlobalGameManager.instance.controls)
            {
                if (id > start+4) break;
                try
                {
                    var pair = selections[id];
                    pair.ObtainControlUnit(control.Value);
                    pair.InitializeFrameCache();
                }
                catch (Exception)
                {
                    GlobalConsole.Error("Could not assign control to character selector");
                }
                id++;
            }
            
            // Register event listeners
            
            GlobalGameManager.connectedInput.AddListener(c =>
            {
                foreach (var selection in  menuComponents[9].obj.transform.GetChild(1).GetComponentsInChildren<Selection>())
                {
                    if (selection.controlledBy == null)
                    {
                        selection.ObtainControlUnit(c);
                        break;
                    }
                }
            });

            // Animations
            LeanTween.rotateZ(menuComponents[7].obj, 22, 20f).setLoopPingPong();
            LeanTween.scale(menuComponents[8].obj, Vector3.one * 1.5f, 10f).setLoopPingPong();
            LeanTween.moveY(menuComponents[2].obj,0.5f,2.5f).setEaseInOutCubic().setLoopPingPong();
            LeanTween.moveY(menuComponents[3].obj,0.5f,2.5f).setEaseInOutCubic().setLoopPingPong();
            LeanTween.moveY(menuComponents[4].obj,0.5f,2.5f).setEaseInOutCubic().setLoopPingPong();
            LeanTween.moveY(menuComponents[5].obj,0.5f,2.5f).setEaseInOutCubic().setLoopPingPong();
        }

        
        // This is the worst thing I coded in my entire life >.<  please don't judge me I have brain damage
        private void Update()
        {
            if (EventSystem.current == null) return;
            
            if (!EventSystem.current.alreadySelecting && EventSystem.current.currentSelectedGameObject == null)
            {
                if (previousButton)
                {
                    EventSystem.current.SetSelectedGameObject(previousButton.gameObject);
                    HighlightButton(previousButton);
                }
                else if (defaultButton)
                {
                    EventSystem.current.SetSelectedGameObject(defaultButton);
                }
            }

            var selectedObj = EventSystem.current.currentSelectedGameObject;
            if (selectedObj.TryGetComponent(typeof(Button), out var selectedAsButton))
            {
                if (selectedAsButton != previousButton)
                {
                    HighlightButton((Button) selectedAsButton);
                }

                if (previousButton && previousButton != selectedAsButton)
                {
                    UnHighlightButton(previousButton);
                }

                previousButton = (Button) selectedAsButton;
            }
            else
            {
                UnHighlightButton(previousButton);
            }
        }

        private void HighlightButton(Button butt)
        {
            butt.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
        }

        private void UnHighlightButton(Button butt)
        {
            butt.transform.localScale = new Vector3(1, 1, 1);
        }

        
        
        public void GlobalConfirm()
        {
            stage = MenuStages.ArenaSelection;
        }

        public void Back()
        {
            if (stage == MenuStages.CharacterSelection && !LeanTween.isTweening(menuComponents[9].obj))
            {
                menuComponents[6].obj.SetActive(true);
                menuComponents[9].obj.transform.GetChild(1).GetComponentsInChildren<Selection>().ToList()
                    .ForEach(s => s.confirmed = false);
                LeanTween.moveY(menuComponents[0].obj, 0, 1f).setDelay(0.5f).setEaseInOutQuad();
                LeanTween.moveY(menuComponents[1].obj, 0.5f, 1f).setDelay(0.5f).setEaseInOutQuad();
                LeanTween.moveX(menuComponents[2].obj, 0, 1f).setDelay(0.5f).setEaseInOutQuad();
                LeanTween.moveX(menuComponents[3].obj, 0, 1f).setDelay(0.5f).setEaseInOutQuad();
                LeanTween.moveX(menuComponents[4].obj, 0, 1f).setDelay(0.5f).setEaseInOutQuad();
                LeanTween.moveX(menuComponents[5].obj, 0, 1f).setDelay(0.5f).setEaseInOutQuad();
                LeanTween.scale(menuComponents[6].obj, Vector3.one, 1f).setDelay(0.5f).setEaseInOutQuad();
                LeanTween.moveY(menuComponents[6].obj, -2.5f, 1f).setDelay(0.5f).setEaseInOutQuad();
                LeanTween.moveY(menuComponents[12].obj, 0, 1f).setEaseInOutCubic();
                LeanTween.moveY(menuComponents[9].obj, -10, 1f).setEaseInOutQuad().setOnComplete(() =>
                {
                    stage = MenuStages.Menu;
                    menuComponents[6].obj.GetComponentsInChildren<Button>().ToList().ForEach(b => b.enabled = true);
                    if (GlobalGameManager.instance.discord != null)
                    {
                        GlobalGameManager.activityManager.UpdateActivity(GlobalGameManager.instance.activities["Menu"], (res) =>
                        {
                            if (res == Result.Ok)
                            {
                                Debug.Log("Discord activity successfully updated");
                            }
                        }); 
                    }
                });
                foreach (var btn in bindings) { btn.button.enabled = true; }
            }
        }
        
        
        
        
        // Menu Events
        public void Play()
        {
            if (LeanTween.isTweening(menuComponents[9].obj)) return;
            foreach (var btn in bindings) { btn.button.enabled = false; }
            menuComponents[6].obj.GetComponentsInChildren<Button>().ToList().ForEach(b => b.enabled = false);
            
            LeanTween.moveY(menuComponents[0].obj, 5, 1f).setEaseInOutQuad();
            LeanTween.moveY(menuComponents[1].obj, 6, 1f).setEaseInOutQuad();
            LeanTween.moveX(menuComponents[2].obj, -5, 1f).setEaseInOutQuad();
            LeanTween.moveX(menuComponents[3].obj, -5, 1f).setEaseInOutQuad();
            LeanTween.moveX(menuComponents[4].obj, 5, 1f).setEaseInOutQuad();
            LeanTween.moveX(menuComponents[5].obj, 5, 1f).setEaseInOutQuad();
            LeanTween.scale(menuComponents[6].obj, Vector3.one * 0.2f , 1f).setEaseInOutQuad();
            LeanTween.moveY(menuComponents[6].obj, -5, 1f).setEaseInOutQuad();
            LeanTween.moveY(menuComponents[12].obj, 2, 1f).setEaseInOutCubic();
            LeanTween.moveY(menuComponents[9].obj, 0, 1f).setEaseInOutQuad().setOnComplete(() =>
            {
                stage = MenuStages.CharacterSelection;
                if (GlobalGameManager.instance.discord != null)
                {
                    GlobalGameManager.activityManager.UpdateActivity(GlobalGameManager.instance.activities["CHSelection"], (res) =>
                    {
                        if (res == Result.Ok)
                        {
                            Debug.Log("Discord activity successfully updated");
                        }
                    });
                }
            });
           
            //GlobalGameManager.instance.LoadGame();
        }

        public void Settings()
        {
            
        }

        public void Quit()
        {
            GlobalConsole.commands["quit"].OnCommand(new []{"quit"});
        }

        public void Volume()
        {
            Management.Settings.instance.volume = !Management.Settings.instance.volume;
        }

        private void Default() {
            Debug.Log("Not implemented");
        }
    }
}
