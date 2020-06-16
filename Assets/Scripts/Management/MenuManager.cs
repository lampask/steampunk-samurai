using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Management
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager instance;
        private Button previousButton;
        [SerializeField] private float scaleAmount = 1.4f;
        public GameObject defaultButton;
        
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
            if (defaultButton)
            {
                var selectedObj = EventSystem.current.currentSelectedGameObject;
                var selectedAsButton = selectedObj.GetComponent<Button>();
                if (selectedAsButton.Equals(null))
                {
                    if (selectedObj.Equals(null))
                    {
                        EventSystem.current.SetSelectedGameObject(previousButton.gameObject);
                        previousButton = null;
                    }
                    return;
                }
                if (selectedAsButton != previousButton)
                {
                    HighlightButton(selectedAsButton);
                }
                if (previousButton && previousButton != selectedAsButton)
                {
                    UnHighlightButton(previousButton);
                }
                previousButton = selectedAsButton;
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
        public void Play() {
            
            GlobalGameManager.instance.LoadGame();
        }

        public void Default() {
            Debug.Log("Not implemented");
        }
    }
}
