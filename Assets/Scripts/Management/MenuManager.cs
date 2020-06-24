using System;
using System.Linq;
using Console;
using Discord;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Management
{
    public class MenuManager : MonoBehaviour
    {
        enum MenuStages
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
        private MenuStages stage = MenuStages.Menu;
        
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

        private void Awake() {
            if (!instance)
                instance = this;
            else
                Destroy(this);
            
            charactersAvailable = Enumerable.Repeat(true, 4).ToArray();
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
            if (EventSystem.current != null)
            {
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
        }

        private void HighlightButton(Button butt)
        {
            butt.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
        }

        private void UnHighlightButton(Button butt)
        {
            butt.transform.localScale = new Vector3(1, 1, 1);
        }

        
        
        public void Confirm(int id)
        {
            
        }

        public void Back()
        {
            if (stage == MenuStages.CharacterSelection)
            {
                menuComponents[6].obj.SetActive(true);
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
                    foreach (var selection in menuComponents[9].obj.transform.GetChild(1).GetComponentsInChildren<Selection>()) { selection.controlledBy = null; }
                    menuComponents[9].obj.SetActive(false);
                    stage = MenuStages.Menu;
                
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
            foreach (var btn in bindings) { btn.button.enabled = false; }
            stage = MenuStages.CharacterSelection;
            menuComponents[9].obj.SetActive(true);
            var id = 0;

            foreach (var control in GlobalGameManager.instance.controls)
            {
                if (id > 4) break;
                try
                {
                    var pair = menuComponents[9].obj.transform.GetChild(1).GetComponentsInChildren<Selection>()[id];
                    pair.id = id;
                    pair.controlledBy = control.Value;
                    pair.InitializeFrameCache();
                }
                catch (Exception)
                {
                    GlobalConsole.Error("Could not assign control to character selector");
                }
                id++;
            }
            
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
                menuComponents[6].obj.SetActive(false);
            
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

        public void Default() {
            Debug.Log("Not implemented");
        }
        
        
    }
}
