using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private void OnEnable() {
        int index = transform.GetSiblingIndex();
        Debug.Log(index);
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
