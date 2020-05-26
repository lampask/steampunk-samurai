using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    private Image[] imgs;

    public Sprite player_icon;

    private void Awake() {
        imgs = GetComponentsInChildren<Image>();
        imgs[0].sprite = player_icon;
    }

    private void Start() {
        int index = transform.GetSiblingIndex();
        if (index % 2 == 1) {
            Reverse();
        }
        if (index > 3) {
            Destroy(this.gameObject);
        }
    }

    void Reverse() {
        imgs[0].gameObject.GetComponent<RectTransform>().anchoredPosition *= Vector2.left;
        foreach(Bar barobject in GetComponents<Bar>()) {
            barobject.reversed = true;
        }
    }
}
