using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class PlayerInfo : MonoBehaviour
    {
        private Image[] imgs;

        [FormerlySerializedAs("player_icon")] public Sprite playerIcon;

        private void Awake() {
            imgs = GetComponentsInChildren<Image>();
            imgs[0].sprite = playerIcon;
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
            imgs[0].gameObject.GetComponent<RectTransform>().anchoredPosition *= Vector2.left;
            foreach(var barObject in GetComponents<Bar>()) {
                barObject.reversed = true;
            }
        }
    }
}
