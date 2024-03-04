
using UI;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;





namespace Network
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        

        

        
        [Header("Login UI")]
        public InputField playerNameInputField;
        public GameObject uI_LoginGameobject;

        [Header("Lobby UI")]
        public GameObject uI_LobbyGameobject;
        public GameObject uI_3DGameobject;

        [Header("Connection Status UI")]
        public GameObject uI_ConnectionStatusGameobject;
        public Text connectionStatusText;
        public bool showConnectionStatus = false;
        
        [SerializeField] private GameObject roomListItemPrefab; // Verknüpfe dies im Editor mit deinem Button Prefab
        [SerializeField] private Transform roomListContent; // Verknüpfe dies mit dem Content-Objekt der ScrollView
        
        private void UpdateRoomListView(List<RoomInfo> roomList)
        {
            // Vorhandene Raum-Items entfernen
            foreach (Transform child in roomListContent)
            {
                Destroy(child.gameObject);
            }

            // Für jeden verfügbaren Raum ein neues UI-Element erstellen
            foreach (RoomInfo room in roomList)
            {
                if(room.IsVisible && room.PlayerCount < room.MaxPlayers)
                {
                    GameObject roomListItem = Instantiate(roomListItemPrefab, roomListContent);
                    roomListItem.SetActive(true);
                    roomListItem.GetComponentInChildren<Text>().text = $"{room.Name} ({room.PlayerCount}/{room.MaxPlayers})";
                    Button roomListButton = roomListItem.GetComponent<Button>();
                    roomListButton.onClick.AddListener(() => JoinRoom(room.Name));
                }
            }
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("CreateRoom fehlgeschlagen: " + message + ". Versuche, erneut mit dem MasterServer zu verbinden...");
            PhotonNetwork.Disconnect(); // Trenne die aktuelle Verbindung
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Vom Server getrennt: " + cause + ". Versuche, erneut zu verbinden...");
            PhotonNetwork.ConnectUsingSettings(); // Verbinde erneut mit dem MasterServer
        }

        private void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        #region UNITY Methods
        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                //Activating only Lobby UI
                uI_LobbyGameobject.SetActive(true);
                uI_3DGameobject.SetActive(true);

                uI_ConnectionStatusGameobject.SetActive(false);

                uI_LoginGameobject.SetActive(false);
            }
            else
            {
                //Activating only Login UI since we did noy connect to Photon yet.

                uI_LobbyGameobject.SetActive(false);
                uI_3DGameobject.SetActive(false);
                uI_ConnectionStatusGameobject.SetActive(false);

                uI_LoginGameobject.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (showConnectionStatus)
            {
                connectionStatusText.text =
                    "Connection Status: " + PhotonNetwork.NetworkClientState;
            }
        }
        
        public void CreateLobby()
        {
            var roomOptions = new RoomOptions()
            {
                IsVisible = true,
                IsOpen = true,
                MaxPlayers = 20 // Setze die maximale Anzahl von Spielern
            };

            PhotonNetwork.CreateRoom(null, roomOptions); // Erstellt einen Raum mit einem zufälligen Namen
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            CreateLobby(); // Erstellt eine Lobby, wenn keine gefunden wurde
        }

        public void JoinLobby()
        {
            PhotonNetwork.JoinRandomRoom(); // Versucht, einer zufälligen Lobby beizutreten
        }
        

        

        #endregion



        #region UI Callback Methods
        public void OnEnterGameButtonClicked()
        {
            string playerName = playerNameInputField.text;

            if (!string.IsNullOrEmpty(playerName))
            {
                uI_LobbyGameobject.SetActive(false);
                uI_3DGameobject.SetActive(false);
                uI_LoginGameobject.SetActive(false);

                showConnectionStatus = true;
                uI_ConnectionStatusGameobject.SetActive(true);

                if (!PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.LocalPlayer.NickName = playerName;

                    PhotonNetwork.ConnectUsingSettings();
                }
            }
            else
            {
                Debug.Log("Player name is invalid or empty!");
            }
        }


 

        public void OnQuickMatchButtonClicked()
        {
            //SceneManager.LoadScene("Scene_Loading");
            SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
        }

        #endregion




        #region PHOTON Callback Methods
        public override void OnConnected()
        {
            Debug.Log("We connected to Internet");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server");

            uI_LobbyGameobject.SetActive(true);
            uI_3DGameobject.SetActive(true);

            uI_LoginGameobject.SetActive(false);
            uI_ConnectionStatusGameobject.SetActive(false);
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            UpdateRoomListView(roomList);
        }
        
        

        #endregion
        
        
    }
    

}
