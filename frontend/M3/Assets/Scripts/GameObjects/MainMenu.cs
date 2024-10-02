using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //networking manager should establish a connection here
    }

    public void exitGame()
    {
        Debug.Log("exiting game");
        Application.Quit();
    }
}
