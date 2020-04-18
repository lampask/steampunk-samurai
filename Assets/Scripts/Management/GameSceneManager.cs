using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;
    public GameObject loadingScreen;

    public enum SceneIndexs {
        MANAGER = 0,
        MENU_SCREEN = 1,
        GAME = 2
    }

    private void Awake() {
        if (!instance)
            instance = this;
        else
            Destroy(this);

        SceneManager.LoadSceneAsync((int) SceneIndexs.MENU_SCREEN, LoadSceneMode.Additive);
    }

    List<AsyncOperation> loading = new List<AsyncOperation>(); 

    public void LoadScene(SceneIndexs current, SceneIndexs target) {
        loadingScreen.SetActive(true);

        loading.Add(SceneManager.UnloadSceneAsync((int) current));
        loading.Add(SceneManager.LoadSceneAsync((int) SceneIndexs.GAME, LoadSceneMode.Additive));

        StartCoroutine(GetLoadProgress());
    }

    // default overload
    public void LoadGame() {
        LoadScene(SceneIndexs.MENU_SCREEN, SceneIndexs.GAME);
    }


    public void LoadCharacterSelection() {
        
    }

    public float totalLoadingProgress;
    public IEnumerator GetLoadProgress() {
        for(int i = 0; i<loading.Count; i++) {
            while (!loading[i].isDone) {
                totalLoadingProgress = 0;

                foreach(AsyncOperation operation in loading) {
                    totalLoadingProgress += operation.progress;
                }

                totalLoadingProgress /= loading.Count;

                // Update progress

                yield return null;
            }
        }
        loading.Clear();
        loadingScreen.SetActive(false);
    }
}
