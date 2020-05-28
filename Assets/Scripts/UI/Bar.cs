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
        [Utils.ReadOnly] public bool inverted
        ;
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

        private RectTransform fill;
        private RectTransform bg;

        void Start()
        {
            rTransform = barObject.GetComponent<RectTransform>();
            rTransform.anchorMin.Half();
            rTransform.anchorMax.Half();

            barFill = rTransform.GetChild(0).gameObject.GetComponent<Image>();
            barBg = rTransform.GetChild(1).GetComponent<Image>();

            fill = barFill.GetComponent<RectTransform>();
            bg = barBg.GetComponent<RectTransform>();

            foreach(var g in new[] {fill, bg}) {
                g.anchorMin = Vector2.zero;
                g.anchorMax = Vector2.right;
            }
        }

        void Update()
        {
            rTransform.sizeDelta = new Vector2(defaultBarWidth, defaultBarHeight);
            var sizeDelta = bg.sizeDelta;
            fill.sizeDelta = new Vector2(sizeDelta.x, defaultBarHeight*(5/6f));
            sizeDelta = new Vector2(sizeDelta.x, defaultBarHeight);
            bg.sizeDelta = sizeDelta;

            rTransform.anchoredPosition3D = new Vector3((reversed ? -1 : 1) * (defaultBarWidth/2-40), rTransform.anchoredPosition3D.y, 0);
            var anchoredPosition3D = bg.anchoredPosition3D;
            fill.anchoredPosition3D = new Vector3(anchoredPosition3D.x, defaultBarHeight/2, 0);
            anchoredPosition3D = new Vector3(anchoredPosition3D.x, defaultBarHeight/2, 0);
            bg.anchoredPosition3D = anchoredPosition3D;

            fill.offsetMin = new Vector2(2, 0);
            fill.offsetMax = new Vector2(-2, defaultBarHeight);

            // Customization
            barFill.fillAmount = percentage/100;
            barFill.color = barFillColor;
        }
    }
}
