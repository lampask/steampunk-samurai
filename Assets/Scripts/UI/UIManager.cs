using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UIManager : MonoBehaviour
{
    public GridLayoutGroup hp_bar;

    private void Update() {
        hp_bar.spacing = Vector2.right*(Screen.width-2*hp_bar.cellSize.x);
    }
}
