using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void EnterLobby()
    {
        SceneManager.LoadScene("LobbyScene"); // Name der Lobby-Szene
    }

    public void StartGame()
    {
        // Code zum Starten des Spiels
    }

    public void OpenSettings()
    {
        // Code zum Öffnen der Einstellungen
    }

    // Weitere Methoden für andere Menüoptionen
}
