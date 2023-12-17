using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Network;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UI;
using Unity.Tutorials.Core.Editor;
using UnityEngine;

public class GameManager :MonoBehaviourPunCallbacks
{

    public static GameManager Instance;

    [Header("UI")]

    public GameObject uI_JoinedRoomInfoGameObject;
    public GameObject uI_InformPanelGameobject;
    public TextMeshProUGUI uI_InformText;
    public GameObject uI_TeamInfoPanel;
    private TextMeshProUGUI[][] uI_TeamPlayers;

    [Space]
    public GameObject uI_GameFieldPlacement;

    [Space]
    public GameObject searchForGamesButtonGameobject;
    public GameObject adjust_Button;
    public GameObject raycastCenter_Image;

    // max allowed players to join one session
    public int maxPlayer = 12;

    private Dictionary<string, GameObject>[] teamsPlayers;

    private SpawnManager spawnManager;

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
        uI_TeamInfoPanel.SetActive(false);

        // get Team-Panel Text
        GameObject uI_Team1 = uI_TeamInfoPanel.transform.Find("Team1").gameObject;
        GameObject uI_Team2 = uI_TeamInfoPanel.transform.Find("Team2").gameObject;
        uI_TeamPlayers = new TextMeshProUGUI[][]{ GetChildrenOfComponent<TextMeshProUGUI>(uI_Team1.transform.Find("Players").gameObject), GetChildrenOfComponent<TextMeshProUGUI>(uI_Team2.transform.Find("Players").gameObject)};

        teamsPlayers[0] = new Dictionary<string, GameObject>();
        teamsPlayers[1] = new Dictionary<string, GameObject>();

        spawnManager = GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        Debug.Log("GM:: JoinRandomRoom::");
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

    public void JoinTeam(int team)
    {
        uI_TeamInfoPanel.SetActive(false);

        string name = PhotonNetwork.LocalPlayer.NickName;

        // join Team
        PhotonNetwork.LocalPlayer.CustomProperties["team"] = team;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

        // set name into panel
        foreach (TextMeshProUGUI textMesh in uI_TeamPlayers[team])
        {
            if (string.IsNullOrWhiteSpace(textMesh.text))
            {
                textMesh.text = name;
            }
        }

        // spawn and position his car
        GameObject player = spawnManager.SpawnPlayer();

        // add PlayerName as key and his car as value to dictionary of players
        teamsPlayers[team][name] = player;

        // update General Network properties
        SetRoomProperties();

        // TODO
        // send event of spawnedPlayer
        spawnManager.RaiseNewPlayerEvent(player);
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

        // let player choose his team
        uI_TeamInfoPanel.SetActive(true);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";
        }
        else
        {
            uI_InformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameobject, 2.0f));

            // retrieve room properties and update local variables
            RetrieveRoomProperties();

            // spawn all other players
            spawnManager.SpawnOtherPlayers();
        }

        Debug.Log("GM::OnJoinedRoom:: - joined to " + PhotonNetwork.CurrentRoom.Name);
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {

        Debug.Log("GM - " + newPlayer.NickName + " joined to "+ PhotonNetwork.CurrentRoom.Name+ " Player count "+ PhotonNetwork.CurrentRoom.PlayerCount);
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
        string randomRoomName = "Room" + UnityEngine.Random.Range(0,1000);

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

    public void RetrieveRoomProperties()
    {
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;

        string[] team1Players = (string[])properties["team1Players"];
        string[] team2Players = (string[])properties["team2Players"];
        string[] team1AvailableSpawnPoints = (string[])properties["team1AvailableSpawnPoints"];
        string[] team2AvailableSpawnPoints = (string[])properties["team2AvailableSpawnPoints"];

        // TODO spawn for each team all players
        // TODO convert names of spawnPoints into SpawnPoint GameObjects and assign spawnPoints
    }

    public void SetRoomProperties()
    {
        ExitGames.Client.Photon.Hashtable props = PhotonNetwork.CurrentRoom.CustomProperties;

        props["team1Players"] = teamsPlayers[0].Keys;
        props["team2Players"] = teamsPlayers[1].Keys;

        // add available spawn points
        HashSet<GameObject>[] availableSpawnPoints = spawnManager.GetAvailableSpawnPoints();
        string[][] availableSpawnPointsNames = new string[2][];

        for(int i = 0; i < availableSpawnPoints.Length; i++)
        {
            availableSpawnPointsNames[i] = new string[availableSpawnPoints[i].Count];
            int index = 0;

            foreach(GameObject gameObject in availableSpawnPoints[i])
            {
                availableSpawnPointsNames[i][index++] = gameObject.name;
            }
        }

        props["team1AvailableSpawnPoints"] = availableSpawnPointsNames[0];
        props["team2AvailableSpawnPoints"] = availableSpawnPointsNames[1];

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private T[] GetChildrenOfComponent<T>(GameObject parent)
    {
        Transform parentTransform = parent.transform;
        T[] children = new T[parentTransform.childCount];

        for (int i = 0; i < parentTransform.childCount; i++)
        {
            children[i] = parentTransform.GetChild(i).GetComponent<T>();
        }

        return children;
    }

    public Dictionary<string, GameObject>[] GetTeamsPlayers() { return teamsPlayers; }
    #endregion

    #region PUBLIC Methods
    public void SetPlayerJoined(int team, string name, GameObject car)
    {
        teamsPlayers[team][name] = car;
    }

    public void ShowSearchForPlayersInfoPanel()
    {
        uI_JoinedRoomInfoGameObject.SetActive(true);
        uI_GameFieldPlacement.SetActive(false);
    }
    #endregion
}