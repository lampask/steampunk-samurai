using System;
using Console;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Management
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager instance;
        public GameObject container;
        [SerializeField] private float scaleAmount = 1.4f;
        private Button previousButton;
        public GameObject defaultButton;
        private bool characterMode;
        
        [Serializable]
        public class MenuPart
        {
            [Utils.ReadOnly] public string name;
            public GameObject obj;

            public MenuPart(string name)
            {
                this.name = name;
            }
        }

        public MenuPart[] menuComponents =
        {
            new MenuPart("Logo"),
            new MenuPart("Mask1"),
            new MenuPart("Mask2"),
            new MenuPart("Mask3"),
            new MenuPart("Mask4"),
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
        }

        private void Start() {
            foreach(var bd in bindings) {
                if (bd.binding.GetPersistentEventCount() <= 1) bd.binding.AddListener(Default);
                bd.button.onClick = bd.binding;
            }
            if (defaultButton != null)
            {
                EventSystem.current.SetSelectedGameObject(defaultButton);
            }
        }

        
        // This is the worst thing I coded in my entire life >.<  please don't judge me I have brain damage
        private void Update()
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

        private void HighlightButton(Button butt)
        {
            butt.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
        }

        private void UnHighlightButton(Button butt)
        {
            butt.transform.localScale = new Vector3(1, 1, 1);
        }

        // Menu Events
        public void Play()
        {
            LeanTween.moveY(container, container.transform.position.y < 0 ? 5 : -5 , 1f).setEaseInOutQuart();
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
