using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    private Fader fader;

    public string newGame = "PlayScene_Floor_1";
    public string bgmName;
    public TextMeshProUGUI userName;

    private void Awake()
    {
        fader = DataManager.instance.fader;
    }

    // Start is called before the first frame update
    void Start()
    {
        //AudioManager.instance.PlayBGM(bgmName, fader.fadeOutTime);
        StartCoroutine(UserNameTyping());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            fader.SceneLoad(newGame);
        }
    }

    IEnumerator UserNameTyping()
    {
        string name = DataManager.instance.userData.userName;

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < name.Length; i++)
        {
            userName.text += name[i];
            yield return new WaitForSeconds(0.3f);
        }

        yield return null;
    }

    /*

    IEnumerator LoadScene()
    {
        asyncOperation = SceneManager.LoadSceneAsync(newGame);

        asyncOperation.allowSceneActivation = false;

        
        while (!asyncOperation.isDone)
        {
            yield return null;

            if (asyncOperation.progress < 0.9f)
            {
                Debug.Log(asyncOperation.progress);
            }          
        }

        yield return null;
    }

    IEnumerator Nextscene()
    {
        fader.FadeIn();

        yield return new WaitForSeconds(fader.fadeInTime);

        Debug.Log("¿Ï·á");

        DataManager.instance.isSet = false;
        asyncOperation.allowSceneActivation = true;
    }
    */
}
