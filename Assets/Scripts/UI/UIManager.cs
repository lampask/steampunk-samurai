using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    [ExecuteAlways]
    public class UIManager : MonoBehaviour
    {
        [FormerlySerializedAs("hp_bar")] public GridLayoutGroup hpBar;

        private void Update() {
            hpBar.spacing = new Vector2(Screen.width-2*hpBar.cellSize.x, hpBar.spacing.y);
        }
    }
}
