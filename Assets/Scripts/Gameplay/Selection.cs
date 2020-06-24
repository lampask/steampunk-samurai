using System;
using System.Collections.Concurrent;
using Gameplay.Input;
using Management;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Gameplay
{
    public class Selection : MonoBehaviour
    {
        [Utils.ReadOnly] public int id; // MAIN IDENTIFICATION
        [Utils.ReadOnly] [SerializeField] public Control controlledBy;
        public CharacterSelection characterSelection = new CharacterSelection();
        public ColorSelection colorSelection = new ColorSelection();
        private Control.ControlSnapshot savedFrame;
        
        // A - B - C - D

        public void InitializeFrameCache() { savedFrame = controlledBy.GenerateFrameStateClone(); }
        
        public class Arrows
        {
            public GameObject left;
            public GameObject right;

            public Arrows(GameObject left, GameObject right)
            {
                this.left = left;
                this.right = right;
            }

            public void Animate()
            {
                LeanTween.moveLocalX(left, 5, 1).setLoopPingPong();
                LeanTween.moveLocalX(right, -5, 1).setLoopPingPong();
            }
        }

        public enum SelectState
        {
            ColorSelection,
            CharacterSelection
        }
        
        // State
        public SelectState state { get; set; }

        // Structure 
        [HideInInspector] public GameObject identification;
        [HideInInspector] public GameObject device;

        // 
        [Serializable]
        public class CharacterSelection
        {
            public int characterIndex = 0;
            [HideInInspector] public Arrows characterArrows;
            [HideInInspector] public GameObject characterSelection;
            
            public void Next()
            {
                characterIndex = (characterIndex + 1) % 4;
                Assign();
            }

            public void Previous()
            {
                characterIndex = (characterIndex + 3) % 4;
                Assign();
            }

            public void Assign()
            {
                if (MenuManager.instance.charactersAvailable[characterIndex])
                {
                    characterSelection.transform.GetChild(3).GetComponent<Image>().sprite = Imports.Characters[characterIndex];
                }
            }
        }
        
    
        [Serializable]
        public class ColorSelection
        {
            public int colorIndex = 0;
            [HideInInspector] public Arrows colorArrows;
            [HideInInspector] public GameObject colorSelection;
            
            public void Next()
            {
                colorIndex = (colorIndex + 1) % 4;
                Assign();
            }

            public void Previous()
            {
                colorIndex = (colorIndex - 1) % 4;
                Assign();
            }

            public void Assign()
            {
                if (MenuManager.instance.charactersAvailable[colorIndex])
                {
                    colorSelection.GetComponent<Image>().color = Globals.Colors[colorIndex];
                }
            }
        }
       
        private void Awake()
        {
            identification = transform.GetChild(0).gameObject;
            colorSelection.colorSelection = transform.GetChild(1).gameObject;
            colorSelection.colorArrows = new Arrows(colorSelection.colorSelection.transform.GetChild(1).gameObject,
                                    colorSelection.colorSelection.transform.GetChild(0).gameObject);
            characterSelection.characterSelection = transform.GetChild(2).gameObject;
            characterSelection.characterArrows = new Arrows(characterSelection.characterSelection.transform.GetChild(1).gameObject,
                                        characterSelection.characterSelection.transform.GetChild(0).gameObject);
            device = transform.GetChild(3).gameObject;
            state = SelectState.CharacterSelection;
        }

        private void Start()
        {
            // Set up looks
            characterSelection.Assign();
            if (controlledBy != null)
            {
                device.GetComponentInChildren<TMP_Text>().text = controlledBy.name;
            }
        }

        private void Update()
        {
            if (controlledBy != null)
            {
                if (state == SelectState.CharacterSelection)
                {
                    if (controlledBy.leftStick.x < 0 && savedFrame.leftStick.x >= 0) characterSelection.Next();
                    else if (controlledBy.leftStick.x > 0 && savedFrame.leftStick.x <= 0) characterSelection.Previous();
                    
                }
                else if (state == SelectState.ColorSelection)
                {
                    if (controlledBy.leftStick.x < 0 && savedFrame.leftStick.x >= 0) colorSelection.Next();
                    else if (controlledBy.leftStick.x > 0 && savedFrame.leftStick.x <= 0) colorSelection.Previous();

                }

                // Global controls

                if (controlledBy.a && !savedFrame.a) MenuManager.instance.Confirm(id);
                if (controlledBy.b && !savedFrame.b) { MenuManager.instance.Back(); return; }
                
                savedFrame = controlledBy.GenerateFrameStateClone();
            }
        }
    }
}