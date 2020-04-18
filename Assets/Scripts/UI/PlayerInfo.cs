using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    private Image[] imgs;

    public Sprite player_icon;
    [Header("Health bar")]
    public Sprite hbar_bg;
    public Sprite hbar_fill;
    [Header("Energy bar")]
    public Sprite ebar_bg;
    public Sprite ebar_fill;

    private void Awake() {
        imgs = GetComponentsInChildren<Image>();
        imgs[0].sprite = player_icon;
        imgs[1].sprite = hbar_bg;
        imgs[2].sprite = hbar_fill;
        imgs[3].sprite = ebar_bg;
        imgs[4].sprite = ebar_fill;
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
        foreach(RectTransform trans in GetComponentsInChildren<RectTransform>()){
            if (trans.anchorMax == new Vector2(.5f,.5f) && trans.anchorMin == new Vector2(.5f,.5f)) 
                trans.anchoredPosition = new Vector2(-trans.anchoredPosition.x, trans.anchoredPosition.y);
            else
                trans.anchoredPosition = new Vector2(0, trans.anchoredPosition.y);
        }
    }
}
