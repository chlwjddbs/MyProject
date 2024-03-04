using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMoveManager : MonoBehaviour
{
    static string nextSceneName;

    //private AsyncOperation asyncOperation;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadSceneInfo(string _nextSceneName)
    {
        nextSceneName = _nextSceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextSceneName);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            yield return null;

            if (asyncOperation.progress < 0.9f)
            {
                Debug.Log(asyncOperation.progress);
            }
            else
            {
                asyncOperation.allowSceneActivation = true;
            }
        }

        yield return null;
    }
}
