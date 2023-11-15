using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Verbindet mit Photon Server
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("RoomName", new RoomOptions { MaxPlayers = 4 }, null);
        // Versucht, einem Raum beizutreten oder einen zu erstellen
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Beigetreten oder Raum erstellt");
        // Hier könnte Code zum Initialisieren des Spiels stehen
    }
}
