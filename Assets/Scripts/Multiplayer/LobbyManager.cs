using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField createRoomInput;
    public InputField joinRoomInput;
    public GameObject roomListContent;
    public GameObject roomListItemPrefab;
    public InputField passwordInput;
    public GameObject playerListContent;
    public GameObject playerListItemPrefab;

    public void CreateRoom()
    {
        string roomName = createRoomInput.text;
        string password = passwordInput.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 4,
                CustomRoomProperties = new Hashtable { { "pw", password } },
                CustomRoomPropertiesForLobby = new string[] { "pw" }
            };
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public void JoinRoom()
    {
        string roomName = joinRoomInput.text;
        string password = passwordInput.text;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerList();
        // Hier könnten Sie die Szene wechseln oder die Lobby-UI aktualisieren
        Debug.Log("Raum beigetreten: " + PhotonNetwork.CurrentRoom.Name);
    }

    void UpdatePlayerList()
    {
        foreach (Transform child in playerListContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            GameObject listItem = Instantiate(playerListItemPrefab, playerListContent.transform);
            listItem.GetComponentInChildren<Text>().text = player.NickName;
        }
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent.transform)
        {
            Destroy(trans.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) continue;
            InstantiateRoomListItem(room);
        }
    }

    void InstantiateRoomListItem(RoomInfo room)
    {
        GameObject listItem = Instantiate(roomListItemPrefab, roomListContent.transform);
        listItem.GetComponentInChildren<Text>().text = room.Name;
    }
    // Weitere Callbacks für Raumlisten-Updates, Fehlerbehandlung usw.
}
