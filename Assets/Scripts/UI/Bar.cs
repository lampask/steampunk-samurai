using Management;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class Bar : MonoBehaviour
    {
        [FormerlySerializedAs("bar_object")]
        [Header("Bar settings")]
        [Tooltip("Bar object should have 2 children one for the border || background and the other for bar fill")]
        public GameObject barObject;
        [FormerlySerializedAs("bar_fill_color")]
        [Space]
        [ColorUsage(true)]
        public Color barFillColor = new Color(1,1,1);
        [FormerlySerializedAs("default_bar_height")] public float defaultBarHeight = 20f;
        [FormerlySerializedAs("default_bar_width")] public float defaultBarWidth = 200f;
        [Utils.ReadOnly] public bool inverted;
        [Space]
        [Tooltip("The bar fill percentage")]
        [Range(0, 100)]
        public float percentage = 100f;
        public bool reversed;
        [FormerlySerializedAs("r_transform")]
        [Space]
        [Header("Target components")]
        public RectTransform rTransform;
        [FormerlySerializedAs("bar_fill")] public Image barFill;
        [FormerlySerializedAs("bar_bg")] public Image barBg;

        public RectTransform border;
        public Vector2 offset = new Vector2(50, 50);
        
        private RectTransform _fill;
        private RectTransform _bg;

        private void Start()
        {
            rTransform = barObject.GetComponent<RectTransform>();
            rTransform.anchorMin.Half();
            rTransform.anchorMax.Half();

            barFill = rTransform.GetChild(0).gameObject.GetComponent<Image>();
            barBg = rTransform.GetChild(1).GetComponent<Image>();

            _fill = barFill.GetComponent<RectTransform>();
            _bg = barBg.GetComponent<RectTransform>();

            foreach(var g in new[] {_fill, _bg}) {
                g.anchorMin = Vector2.zero;
                g.anchorMax = Vector2.one;
            }
        }

        private void Update()
        {
            rTransform.sizeDelta = new Vector2(defaultBarWidth, defaultBarHeight);
            _fill.sizeDelta = new Vector2(_fill.sizeDelta.x, defaultBarHeight);

            _fill.rotation = reversed ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

            var sizeDelta = border.sizeDelta;
            var maxWidth = sizeDelta.x - offset.x - offset.y;
            rTransform.anchoredPosition3D = new Vector3((reversed ? 1 : 0) * (maxWidth-defaultBarWidth) - maxWidth/2, rTransform.anchoredPosition3D.y, 0);
            // Customization
            barFill.fillAmount = Mathf.Lerp(barFill.fillAmount, percentage/100, Time.deltaTime * 3);
            barFill.color = barFillColor;
        }
    }
}
