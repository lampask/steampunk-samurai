using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Bar : MonoBehaviour
{
    [Header("Bar settings")]
    [Tooltip("Bar object should have 2 children one for the border || background and the other for bar fill")]
    public GameObject bar_object;
    [Space]
    [ColorUsage(true)]
    public Color bar_fill_color = new Color(1,1,1);
    public float default_bar_height = 20f;
    public float default_bar_width = 200f;
    [Utils.ReadOnly] public bool inverted = false;
    [Space]
    [Tooltip("The bar fill percentage")]
    [Range(0, 100)]
    public float percentage = 100f;
    public bool reversed = false;
    [Space]
    [Header("Target components")]
    public RectTransform r_transform;
    public Image bar_fill;
    public Image bar_bg;

    private RectTransform fill;
    private RectTransform bg;

    void Start()
    {
        r_transform = bar_object.GetComponent<RectTransform>();
        r_transform.anchorMin.half();
        r_transform.anchorMax.half();

        bar_fill = r_transform.GetChild(0).gameObject.GetComponent<Image>();
        bar_bg = r_transform.GetChild(1).GetComponent<Image>();

        fill = bar_fill.GetComponent<RectTransform>();
        bg = bar_bg.GetComponent<RectTransform>();

        foreach(RectTransform g in new RectTransform[] {fill, bg}) {
            g.anchorMin = Vector2.zero;
            g.anchorMax = Vector2.right;
        }
    }

    void Update()
    {
        r_transform.sizeDelta = new Vector2(default_bar_width, default_bar_height);
        fill.sizeDelta = new Vector2(bg.sizeDelta.x, default_bar_height*(5/6f));
        bg.sizeDelta = new Vector2(bg.sizeDelta.x, default_bar_height);

        r_transform.anchoredPosition3D = new Vector3((reversed ? -1 : 1) * (default_bar_width/2-40), r_transform.anchoredPosition3D.y, 0);
        fill.anchoredPosition3D = new Vector3(bg.anchoredPosition3D.x, default_bar_height/2, 0);
        bg.anchoredPosition3D = new Vector3(bg.anchoredPosition3D.x, default_bar_height/2, 0);

        fill.offsetMin = new Vector2(2, 0);
        fill.offsetMax = new Vector2(-2, default_bar_height);

        // Customization
        bar_fill.fillAmount = percentage/100;
        bar_fill.color = bar_fill_color;
    }
}
