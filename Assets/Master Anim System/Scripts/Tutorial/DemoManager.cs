using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class DemoManager : MonoBehaviour {
    public Text LoadingText;
    public GameObject loadingSign;


    public void PlayButton(int SceneIndex) {
        StartCoroutine(LoadAsynchronously(SceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = Application.LoadLevelAsync(sceneIndex);
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
           
            float progress = Mathf.Clamp01(operation.progress / .9f);
            LoadingText.text = "Loading " + (progress * 100).ToString("F0") + "%";

            yield return null;
        }
        yield return operation;
    }

    public void QuitButton() {
        Application.Quit();
    }

    public void OpenURLButton(string URL) {
        Application.OpenURL(URL);
    }
}
