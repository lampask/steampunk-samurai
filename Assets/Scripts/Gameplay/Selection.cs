using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using Definitions;
using Gameplay.Input;
using Management;
using Models;
using TMPro;
using UnityEditor;
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
        private Control.ControlSnapshot _savedFrame;
        private List<int> _currentAnimIds = new List<int>();
        public Vector3 chPosStamp;
        private bool _swapped;
        
        private bool _confirmed = false;
        public bool confirmed
        {
            get => _confirmed;
            set
            {
                if (_confirmed == value) return;
                _confirmed = value;
                if (_confirmed)
                    Confirm();
                else
                    ClearConfirm();
            }
        }
        
        // Tween descriptors
        private LTDescr _characterAnim;
        
        // A - B - C - D

        public void InitializeFrameCache() { _savedFrame = controlledBy.GenerateFrameStateClone(); }
        
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
            CharacterSelection,
            Disabled,
        }
        
        // State
        private SelectState _state;
        public SelectState state
        {
            get => _state;
            set
            {
                _state = value;
                // TODO: yeet
            }
        }

        private void AddArrowLoop(Arrows aObj)
        {
            _currentAnimIds.Add(LeanTween.moveLocalX(aObj.left, -220f, 1).setLoopPingPong().uniqueId);
            _currentAnimIds.Add(LeanTween.moveLocalX(aObj.right, 220f, 1).setLoopPingPong().uniqueId);
        }

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
                if (!Assign())
                    Next();
            }

            public void Previous()
            {
                characterIndex = (characterIndex + 3) % 4;
                if (!Assign())
                    Previous();
            }

            public bool Assign()
            {
                if (MenuManager.instance.charactersAvailable[characterIndex])
                {
                    SetCharacter(characterIndex);
                    return true;
                }

                return false;
            }
            
            public void SetCharacter(int index) {characterSelection.transform.GetChild(3).GetComponent<Image>().sprite = Imports.Characters[index];}
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
                if (!Assign())
                    Next();
            }

            public void Previous()
            {
                colorIndex = (colorIndex + 3) % 4;
                if (!Assign())
                    Previous();
            }

            public bool Assign()
            {
                if (MenuManager.instance.charactersAvailable[colorIndex])
                {
                    SetColor(colorIndex);
                    return true;
                }

                return false;
            }
            
            public void SetColor(int index) { colorSelection.transform.GetChild(2).GetComponent<Image>().color = Globals.Colors[index]; }
        }

        private void SetDeviceText(string text) { device.GetComponentInChildren<TMP_Text>().text = text; }
        
        public void Reset()
        {
            controlledBy = null;
            colorSelection.SetColor(4);
            characterSelection.SetCharacter(4);
            SetDeviceText("X");
        }

        public void ObtainControlUnit(Control c) => ObtainControlUnit(c, false);
        public void ObtainControlUnit(Control c, bool force)
        {
            if (controlledBy != null && !force)
            { 
                Debug.LogWarning("Attempted to overwrite controlling unit.");
                return;
            }

            controlledBy = c;
            SetDeviceText(c.name);
            colorSelection.Assign();
            characterSelection.Assign();
            state = SelectState.CharacterSelection;
        }

        public Tuple<PlayerModel, PlayerDefinition> ConvertToDefiningComponents()
        {
            var model = new PlayerModel(id,100,100, controlledBy);
            var definition = new PlayerDefinition(id, colorSelection.colorIndex, controlledBy);
            return new Tuple<PlayerModel, PlayerDefinition>(model, definition);
        }
        
        private void Awake()
        {
            id = transform.GetSiblingIndex();
            _characterAnim = new LTDescr();
            identification = transform.GetChild(0).gameObject;
            colorSelection.colorSelection = transform.GetChild(1).gameObject;
            colorSelection.colorArrows = new Arrows(colorSelection.colorSelection.transform.GetChild(1).gameObject,
                                    colorSelection.colorSelection.transform.GetChild(0).gameObject);
            characterSelection.characterSelection = transform.GetChild(2).gameObject;
            characterSelection.characterArrows = new Arrows(characterSelection.characterSelection.transform.GetChild(1).gameObject,
                                        characterSelection.characterSelection.transform.GetChild(0).gameObject);
            device = transform.GetChild(3).gameObject;
            state = SelectState.Disabled;
            chPosStamp = characterSelection.characterSelection.transform.GetChild(3).position;
            characterSelection.characterSelection.GetComponent<Image>().color = Globals.DimColor;
        }

        private void Start()
        {
            // Set up default looks
            Reset();

            // Register event listeners
            
            GlobalGameManager.disconnectedInput.AddListener(c =>
            {
                if (c.Equals(controlledBy))
                    if (_swapped)
                    {
                        if (confirmed) confirmed = false;
                        ObtainControlUnit(
                            GlobalGameManager.instance.controls.First(x => x.Value.type == Control.ControlType.Keyboard)
                                .Value, true);
                        _swapped = false;
                    } 
                    else
                        Reset();
            });
            
            GlobalGameManager.connectedInput.AddListener(c =>
            {
                if (controlledBy != null)
                    if (controlledBy.type == Control.ControlType.Keyboard &&
                        GlobalGameManager.instance.controls.Count > 4)
                    {
                        ObtainControlUnit(c, true);
                        _swapped = true;
                    }
            });
            
            MenuManager.instance.playerConfirmed.AddListener((id) =>
            {
                if (id != this.id && controlledBy != null && !confirmed)
                {
                    if (!MenuManager.instance.colorsAvailable[colorSelection.colorIndex]) colorSelection.Next();
                    if (!MenuManager.instance.charactersAvailable[characterSelection.characterIndex]) characterSelection.Next();
                }
            });
        }

        private float tolerance = 2f;
        private void Update()
        {
            if (controlledBy == null) return;
            if (_savedFrame != null && MenuManager.instance.stage == MenuManager.MenuStages.CharacterSelection)
            {
                if (Math.Abs(controlledBy.globalAxis.y) > tolerance || Math.Abs(controlledBy.globalAxis.y) < -tolerance)
                    state = (SelectState) Math.Abs((int) state - 1);

                if (state == SelectState.CharacterSelection)
                {
                    if (controlledBy.globalAxis.x > tolerance && _savedFrame.globalAxis.x <= tolerance)
                    {
                        if (confirmed)
                        {
                            confirmed = false;
                            return;
                        }

                        characterSelection.Next();
                    }
                    else if (controlledBy.globalAxis.x < -tolerance && _savedFrame.globalAxis.x >= -tolerance)
                    {
                        if (confirmed)
                        {
                            confirmed = false;
                            return;
                        }

                        characterSelection.Previous();
                    }
                }
                else if (state == SelectState.ColorSelection)
                {
                    if (controlledBy.globalAxis.x > tolerance && _savedFrame.globalAxis.x <= tolerance)
                    {
                        if (confirmed)
                        {
                            confirmed = false;
                            return;
                        }

                        colorSelection.Next();
                    }
                    else if (controlledBy.globalAxis.x < -tolerance && _savedFrame.globalAxis.x >= -tolerance)
                    {
                        if (confirmed)
                        {
                            confirmed = false;
                            return;
                        }

                        colorSelection.Previous();
                    }

                }

                // Global controls

                if (controlledBy.a && !_savedFrame.a)
                {
                    if (confirmed && MenuManager.instance.confirmed >= 2)
                    {
                        MenuManager.instance.GlobalConfirm();
                    }
                    else
                    {
                        confirmed = !confirmed;
                    }
                }

                if (controlledBy.b && !_savedFrame.b)
                {
                    MenuManager.instance.Back();
                    return;
                }
            }

            _savedFrame = controlledBy.GenerateFrameStateClone();
        }

        public void Confirm()
        {
            characterSelection.characterSelection.GetComponent<UIShiny>().Play();
            var ch = characterSelection.characterSelection.transform.GetChild(3).gameObject;
            ch.transform.localScale = Vector3.one;
            ch.LeanScale(new Vector3(1.05f, 1.05f, 1.05f), 0.2f).setLoopPingPong(1);
            characterSelection.characterSelection.GetComponent<Image>().color = Color.white;
            var chm = characterSelection.characterSelection.transform.GetChild(4);
            chm.transform.localScale = Vector3.one * 2;
            chm.GetComponent<Image>().enabled = true;
            chm.LeanScale(Vector3.one, 0.2f);
            
            // Management
            if (!MenuManager.instance.charactersAvailable[characterSelection.characterIndex] ||
                !MenuManager.instance.colorsAvailable[colorSelection.colorIndex])
                ClearConfirm(false);
            
            MenuManager.instance.charactersAvailable[characterSelection.characterIndex] = false;
            MenuManager.instance.colorsAvailable[colorSelection.colorIndex] = false;
            MenuManager.instance.playerConfirmed.Invoke(id);
            
            Debug.Log("Confirmed");
        }


        public void ClearConfirm() => ClearConfirm(true);
        public void ClearConfirm(bool invoke)
        {
            var ch = characterSelection.characterSelection.transform.GetChild(3).gameObject;
            ch.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2.5f, -15);
            LeanTween.moveLocalX(ch, 2.5f, 0.2f).setLoopPingPong(1).setEaseShake();
            characterSelection.characterSelection.GetComponent<Image>().color = Globals.DimColor;
            characterSelection.characterSelection.transform.GetChild(4).GetComponent<Image>().enabled = false;
            if (invoke)
            {
                MenuManager.instance.charactersAvailable[characterSelection.characterIndex] = true;
                MenuManager.instance.colorsAvailable[colorSelection.colorIndex] = true;
            }
            MenuManager.instance.playerUnconfirmed.Invoke(id);
            Debug.Log("UnConfirmed");
        }
        
    }
}