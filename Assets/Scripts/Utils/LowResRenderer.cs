using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowResRenderer : MonoBehaviour
{
    public Camera[] all_cameras;
    private RenderTexture[] allRendTextures;
    private int yRes = 150;

    // Start is called before the first frame update
    void Start()
    {
        SetupRenderTextures();
    }

    private void OnGUI() {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), allRendTextures[0], ScaleMode.ScaleToFit);
    }

    private void SetupRenderTextures() {
        allRendTextures = new RenderTexture[all_cameras.Length];

        int ratio = Screen.height / yRes;
        int newScreenWidth = Screen.width / ratio;
        int newScreenHeight = Screen.height / ratio;

        for (int i = 0; i < allRendTextures.Length; i++) {
            allRendTextures[i] = new RenderTexture(newScreenWidth, newScreenHeight, 0, RenderTextureFormat.ARGB32);
            allRendTextures[i].antiAliasing = 1;
            allRendTextures[i].filterMode = FilterMode.Point;
            allRendTextures[i].isPowerOfTwo = false;
            all_cameras[i].targetTexture = allRendTextures[i];
        }
    }
}
