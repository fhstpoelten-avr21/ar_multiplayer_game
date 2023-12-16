using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UI;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GameManager :MonoBehaviourPunCallbacks
{

    public static GameManager Instance;

    [Header("UI")]

    public GameObject uI_JoinedRoomInfoGameObject;
    public GameObject uI_InformPanelGameobject;
    public TextMeshProUGUI uI_InformText;

    [Space]
    public GameObject uI_GameFieldPlacement;

    [Space]
    public GameObject searchForGamesButtonGameobject;
    public GameObject adjust_Button;
    public GameObject raycastCenter_Image;

    // max allowed players to join one session
    public int maxPlayer = 12;
    public Dictionary<string, GameObject> team1Players;
    public Dictionary<string, GameObject> team2Players;
    public bool arenaIsSet = false;

    // Called once, before start
    // makes sure it acts as a singleton (only one Instance)
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        uI_JoinedRoomInfoGameObject.SetActive(false);
        uI_InformPanelGameobject.SetActive(true);
        uI_GameFieldPlacement.SetActive(true);

        team1Players = new Dictionary<string, GameObject>();
        team2Players = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        Debug.Log("JoinRandomRoom::");
        uI_InformText.text = "Searching for available rooms...";

        PhotonNetwork.JoinRandomRoom();

        searchForGamesButtonGameobject.SetActive(false);


    }


    public void OnQuitMatchButtonClicked()
    {

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();

        }
        else
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }
        


    }
    #endregion


    #region PHOTON Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
      
        Debug.Log("OnJoinRandomFailed" + message);
        uI_InformText.text = message;

        CreateAndJoinRoom();
    }


    public override void OnJoinedRoom()
    {
        adjust_Button.SetActive(false);
        raycastCenter_Image.SetActive(false);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";


        }
        else
        {
            uI_InformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameobject, 2.0f));
        }

        Debug.Log( " joined to "+ PhotonNetwork.CurrentRoom.Name);
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {

        Debug.Log(newPlayer.NickName + " joined to "+ PhotonNetwork.CurrentRoom.Name+ " Player count "+ PhotonNetwork.CurrentRoom.PlayerCount);
        uI_InformText.text = newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount;

        StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameobject, 2.0f));


    }


    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }


    #endregion


    #region PRIVATE Methods
    void CreateAndJoinRoom()
    {
        Debug.Log("CreateAndJoinRoom:: ");
        string randomRoomName = "Room" + Random.Range(0,1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 6;

        //Creating the room
        PhotonNetwork.CreateRoom(randomRoomName,roomOptions);

    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);

    }


    #endregion

    #region PUBLIC Methods
    public void SetPlayerJoined(int team, string name, GameObject car)
    {
        if (team == 0)
        {
            team1Players.Add(name, car);
        }
        else
        {
            team2Players.Add(name, car);
        }
    }

    public void ShowSearchForPlayersInfoPanel()
    {
        uI_JoinedRoomInfoGameObject.SetActive(true);
        uI_GameFieldPlacement.SetActive(false);
    }
    #endregion
}