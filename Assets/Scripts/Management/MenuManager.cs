using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    private void Awake() {
        if (!instance)
            instance = this;
        else
            Destroy(this);
    }
}
