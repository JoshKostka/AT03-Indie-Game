using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameButton : MonoBehaviour
{
    public int sceneIndex = 0;
    public int sceneIndex2 = 2;
    public int sceneIndex3 = 1;

    public void LoadNewGame()
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
    public void LoadAboutScene()
    {
        SceneManager.LoadScene(sceneIndex2);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(sceneIndex3);
    }

}
