using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI progressText;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        Debug.Log("Start loading the scene");
        AsyncOperation async = SceneManager.LoadSceneAsync("MainGame", LoadSceneMode.Single);
        async.allowSceneActivation = false;

        //THis just a trick that allow you to observe the loading scene
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Progress: " + async.progress);
        while (async.progress < 0.9f)
        {
            progressText.text = Mathf.RoundToInt(async.progress * 100) + "%";
            yield return null;
        }
       
        async.allowSceneActivation = true;
    }
}
