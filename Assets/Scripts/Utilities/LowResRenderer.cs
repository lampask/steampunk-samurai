using UnityEngine;
using UnityEngine.Serialization;

namespace Utilities
{
    public class LowResRenderer : MonoBehaviour
    {
        [FormerlySerializedAs("all_cameras")] public Camera[] allCameras;
        private RenderTexture[] allRendTextures;
        private int yRes = 150;

        // Start is called before the first frame update
        private void Start()
        {
            SetupRenderTextures();
        }

        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), allRendTextures[0], ScaleMode.ScaleToFit);
        }

        private void SetupRenderTextures() {
            allRendTextures = new RenderTexture[allCameras.Length];

            var ratio = Screen.height / yRes;
            var newScreenWidth = Screen.width / ratio;
            var newScreenHeight = Screen.height / ratio;

            for (var i = 0; i < allRendTextures.Length; i++) {
                allRendTextures[i] = new RenderTexture(newScreenWidth, newScreenHeight, 0, RenderTextureFormat.ARGB32);
                allRendTextures[i].antiAliasing = 1;
                allRendTextures[i].filterMode = FilterMode.Point;
                allRendTextures[i].isPowerOfTwo = false;
                allCameras[i].targetTexture = allRendTextures[i];
            }
        }
    }
}
