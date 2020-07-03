using System;
using System.Collections.Generic;
using Management;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class PlayerInfo : MonoBehaviour
    {
        public int id;
        private Image[] _imgs;

        [FormerlySerializedAs("player_icon")] public Sprite playerIcon;

        private RectTransform _rt;

        public Bar hBar;
        public Bar eBar;
        
        private void Awake() {
            _imgs = GetComponentsInChildren<Image>();
            _imgs[0].sprite = playerIcon;
            _rt = GetComponent<RectTransform>();
        }

        private void Start() {
            var index = transform.GetSiblingIndex();
            if (index % 2 == 1) {
                Reverse();
            }
            if (index > 3) {
                Destroy(this.gameObject);
            }
        }

        private void Reverse() {
            new List<Image>() {_imgs[0], _imgs[1]}.ForEach(i => i.gameObject.GetComponent<RectTransform>().anchoredPosition *= Vector2.left);
            foreach(var barObject in GetComponents<Bar>()) {
                barObject.reversed = true;
            }
        }

        private void Update()
        {
            transform.localScale = Vector3.one;
            var anchoredPosition = _rt.anchoredPosition;
            _rt.anchoredPosition3D = new Vector3(anchoredPosition.x, anchoredPosition.y, 0);
        }
    }
}
