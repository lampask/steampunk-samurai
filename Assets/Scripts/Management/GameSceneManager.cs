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
        GAME = 2,
        CREDITS = 3
    }

    private void Awake() {
        if (!instance)
            instance = this;
        else
            Destroy(this);

        SceneManager.LoadSceneAsync((int) SceneIndexs.MENU_SCREEN, LoadSceneMode.Additive);
    }

    List<AsyncOperation> loading = new List<AsyncOperation>(); 

    public void LoadGame(SceneIndexs current) {
        loadingScreen.SetActive(true);

        loading.Add(SceneManager.UnloadSceneAsync((int) current));
        loading.Add(SceneManager.LoadSceneAsync((int) SceneIndexs.GAME, LoadSceneMode.Additive));

        StartCoroutine(GetLoadProgress());
    }

    // default overload
    public void LoadGame() {
        LoadGame(SceneIndexs.MENU_SCREEN);
    }

    float totalLoadingProgress;
    public IEnumerator GetLoadProgress() {
        for(int i = 0; i<loading.Count; i++) {
            while (!loading[i].isDone) {
                totalLoadingProgress = 0;

                foreach(AsyncOperation operation in loading) {
                    totalLoadingProgress += operation.progress;
                }

                totalLoadingProgress /= loading.Count;

                yield return null;
            }
        }
        //yield return new WaitForSeconds(2);
        loading.Clear();
        loadingScreen.SetActive(false);
    }
}
