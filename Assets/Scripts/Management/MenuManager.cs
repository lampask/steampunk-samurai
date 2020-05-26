using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

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

    private void OnEnable() {

    }

    private void Awake() {
        if (!instance)
            instance = this;
        else
            Destroy(this);
    }

    private void Start() {
        Debug.Log("Start");
        foreach(Binding bd in bindings) {
            if (bd.binding.GetPersistentEventCount() <= 1) bd.binding.AddListener(Default);
            bd.button.onClick = bd.binding;
        }
    }

    // Menu Events

    public void Play() {
        GlobalGameManager.instance.LoadGame();
    }

    public void Default() {
        Debug.Log("Not implemented");
    }
}
