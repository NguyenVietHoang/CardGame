using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroView : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Loading");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
