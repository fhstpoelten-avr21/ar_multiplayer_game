using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void Lobby()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
    
    public void BackMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("main_menu");
    }
    
    public void BackLobby()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_Lobby");
    }
}

