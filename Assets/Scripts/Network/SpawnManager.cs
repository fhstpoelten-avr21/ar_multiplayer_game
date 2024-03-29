﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Player;
using System;
using System.Collections.Generic;
using DavidJalbert;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Network
{
    public class SpawnManager : MonoBehaviourPunCallbacks
    {

        public GameObject[] playerPrefabs;

        public GameObject battleArenaGameobject;
        public GameObject fieldAreaGameObject;
        public GameObject carsContainer;

        private GameObject gameFieldGameObject;

        private GameManager gameManager;

        private GameObject team1Area;
        private GameObject team2Area;

        // spawn Points of team1 and team2
        private GameObject[][] spawnPoints;

        // currently available (non-taken) spawn Points of team1 and team2
        private HashSet<GameObject>[] availableSpawnPoints;

        public enum RaiseEventCodes
        {
            PlayerSpawnEventCode = 0
        }

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;

            gameFieldGameObject = fieldAreaGameObject.transform.GetChild(0).gameObject;
            team1Area = gameFieldGameObject.transform.Find("Team1Area").gameObject;
            team2Area = gameFieldGameObject.transform.Find("Team2Area").gameObject;

            spawnPoints = new GameObject[2][]{ GetChildrenGameObjects(team1Area), GetChildrenGameObjects(team2Area)};
            availableSpawnPoints = new HashSet<GameObject>[]{ new HashSet<GameObject>(), new HashSet<GameObject>() };

            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

            InitSpawnPoints();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        private void InitSpawnPoints()
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                for(int j = 0; j < spawnPoints[i].Length; j++)
                {
                    spawnPoints[i][j].transform.LookAt(battleArenaGameobject.transform.position);
                    availableSpawnPoints[i].Add(spawnPoints[i][j]);
                }
            }
        }

        #region Legacy dynamic spawn position calculation
        //private void InitSpawnPositionsOld()
        //{
        //    int maxLanes = 3;
        //    int numberOfLanes = Convert.ToInt32(Math.Ceiling((double)(gameManager.maxPlayer/2) / maxLanes));

        //    Vector3 fieldSize = battleArenaMeshRenderer.bounds.size;

        //    // fieldHeight => must be the smallest val
        //    float fieldHeight = Mathf.Min(fieldSize.x, fieldSize.y, fieldSize.z);

        //    // fieldLength => must be the max of all three values
        //    float fieldLength = Mathf.Max(fieldSize.x, fieldSize.y, fieldSize.z);
        //    float fieldLengthOffset = fieldLength * 0.2f;

        //    // calculate the middle value of all 3 (x, y, z), because this must be the width
        //    float fieldWidth = fieldSize.x + fieldSize.y + fieldSize.z - fieldHeight - fieldLength;
        //    float fieldWidthOffset = fieldWidth * 0.1f;

        //    // calculate space where spawn-slots should be set in each team-side
        //    Vector3 slotContainerSize = new Vector3(fieldWidth - fieldWidthOffset, fieldHeight, fieldLength / 2 - fieldLengthOffset);
        //    Vector3[][] spawnSlots = CalculateSpawnPositions(numberOfLanes, slotContainerSize);
        //    //Vector3[][][] spawnSlots = CalculateSpawnPositions(numberOfLanes, fieldSize, battleArenaGameobject.transform.position);

        //    // set position of parent-spawnPosition-GameObject
        //    for (int i = 0; i < spawnSlots.Length; i++)
        //    {
        //        float zPos = (i == 0) ? fieldLength / 4 : -fieldLength / 4;
        //        Debug.Log("spawnPositions[i].transform.position.z vs zPos:: " + spawnPositions[i].transform.position.z + ", " + zPos);
        //        spawnPositions[i].transform.position = new Vector3(spawnPositions[i].transform.position.x, spawnPositions[i].transform.position.y, spawnPositions[i].transform.position.z + zPos);
        //        spawnPositions[i].transform.LookAt(fieldAreaGameObject.transform, Vector3.up);
        //    }

        //    for (int i = 0; i < spawnPositions.Length; i++)
        //    {
        //        // for each lane
        //        for (int j = 0; j < spawnSlots.Length; j++)
        //        {
        //            // for each slot
        //            for (int k = 0; k < spawnSlots[j].Length; k++)
        //            {
        //                GameObject spawnPoint = new GameObject($"SpawnPosition_{availableSpawnPoints[i].Count + 1}_Team_{i + 1}");
        //                spawnPoint.transform.SetParent(spawnPositions[i].transform);
        //                //spawnPoint.transform.localPosition = new Vector3(spawnPositions[i].transform.position.x + spawnSlots[j][k].x, spawnPositions[i].transform.position.y + spawnSlots[j][k].y, spawnPositions[i].transform.position.z + spawnSlots[j][k].z);
        //                spawnPoint.transform.localPosition = spawnPoint.transform.InverseTransformPoint(spawnSlots[j][k]);
        //                spawnPoint.transform.localScale = Vector3.one;
        //                spawnPoint.transform.LookAt(fieldAreaGameObject.transform, Vector3.up);
        //                availableSpawnPoints[i].Add(spawnPoint);
        //            }
        //        }
        //    }
        //}

        //private Vector3[][][] CalculateSpawnPositions(int numberOfLanes, Vector3 fieldBounds, Vector3 fieldPos)
        //{
        //    // fieldHeight => must be the smallest val
        //    float fieldHeight = Mathf.Min(fieldBounds.x, fieldBounds.y, fieldBounds.z);

        //    // fieldLength => must be the max of all three values
        //    float fieldLength = Mathf.Max(fieldBounds.x, fieldBounds.y, fieldBounds.z);
        //    float fieldLengthOffset = fieldLength * 0.2f;

        //    // calculate the middle value of all 3 (x, y, z), because this must be the width
        //    float fieldWidth = fieldBounds.x + fieldBounds.y + fieldBounds.z - fieldHeight - fieldLength;
        //    float fieldWidthOffset = fieldWidth * 0.1f;

        //    // calculate space where spawn-slots should be set in each team-side
        //    float newSpawnLength = fieldLength / 2 - fieldLengthOffset;
        //    float newSpawnWidth = fieldWidth - fieldWidthOffset;
        //    Vector3[][][] slotGrid = new Vector3[2][][];
        //    int multiplicator = -1;

        //    // for each team
        //    for (int i = 0; i < 2; i++)
        //    {
        //        slotGrid[i] = new Vector3[numberOfLanes][];

        //        for (int j = 0; j < numberOfLanes; j++)
        //        {
        //            slotGrid[i][j] = new Vector3[3];
        //            float lengthPos = (multiplicator * (newSpawnLength / 2)) + (newSpawnLength / numberOfLanes) / 2 + (j * newSpawnLength / numberOfLanes);

        //            for (int k = 0; k < 3; k++)
        //            {
        //                Vector3 pos = new Vector3(fieldPos.z, fieldPos.y, fieldPos.x + lengthPos);
        //                slotGrid[i][j][k] = pos;
        //            }

        //            slotGrid[i][j][0].x += newSpawnWidth/4;
        //            slotGrid[i][j][2].x += -newSpawnWidth/4;
        //        }
        //        multiplicator *= -1;
        //    }

        //    return slotGrid;
        //}
        #endregion

        public GameObject GetSpawnByTeamAndName(int teamIndex, string spawnPointName)
        {
            foreach(GameObject spawnPoint in spawnPoints[teamIndex])
            {
                if(spawnPoint.name == spawnPointName)
                {
                    return spawnPoint;
                }
            }

            return null;
        }

        // Assigns random spawn position of specific Team
        private GameObject AssignSpawnPoint(int teamIndex, GameObject _spawnPoint = null)
        {
            if(_spawnPoint != null)
            {
                availableSpawnPoints[teamIndex].Remove(_spawnPoint);
            } else
            {
                foreach (var spawnPoint in availableSpawnPoints[teamIndex])
                {
                    availableSpawnPoints[teamIndex].Remove(spawnPoint);
                    return spawnPoint;
                }
            }

            // Handle case where no spawn points are available
            return null;
        }

        // returns array of GameObjects that are children of parent GameObject
        private GameObject[] GetChildrenGameObjects(GameObject parent)
        {
            Transform parentTransform = parent.transform;
            GameObject[] children = new GameObject[parentTransform.childCount];

            for (int i = 0; i < parentTransform.childCount; i++)
            {
                children[i] = parentTransform.GetChild(i).gameObject;
            }

            return children;
        }

        #region Photon Callback Methods
        void OnEvent(EventData photonEvent)
        {
            
            // Other Player joined
            if (photonEvent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode)
            {
                Debug.Log("OnEvent:: player spawned!");
                object[] data = (object[])photonEvent.CustomData;
                
                // get player through Id to get custom properties
                int playerId = (int)data[3];
                
                // if player spawned is me => don't continue
                if (playerId == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    return;
                }
                
                Photon.Realtime.Player photonPlayer = PhotonNetwork.CurrentRoom.Players[playerId];
                Hashtable playerCustomProps = photonPlayer.CustomProperties;
                
                int team = (int)playerCustomProps["team"];
                    
                Debug.Log($"OnEvent:: myActor_{PhotonNetwork.LocalPlayer.ActorNumber} thisActor_{photonPlayer.ActorNumber}");
                Debug.Log($"OnEvent:: player found? {gameManager.FindPlayerGameObject(team, photonPlayer.NickName)}");
                
                // if player already spawned on this client => dont spawn again
                if (!gameManager.FindPlayerGameObject(team, photonPlayer.NickName))
                {
                    Vector3 receivedPosition = (Vector3)data[0];
                    Quaternion receivedRotation = (Quaternion)data[1];

                    // get data through custom properties of player
                    int playerSelectionNumber = (int)playerCustomProps[Constants.PLAYER_SELECTION_NUMBER];
                    string spawnPositionName = (string)playerCustomProps["spawnPoint"];

                    // instantiate Player
                    GameObject player = Instantiate(playerPrefabs[playerSelectionNumber], receivedPosition + battleArenaGameobject.transform.position, receivedRotation);
                    Vector3 scale = player.transform.localScale;
                    player.transform.SetParent(carsContainer.transform);
                    player.transform.localScale = scale;
                    
                    // set player is not in control over his car in this side of client
                    // necessary to overwrite rb.rotation
                    TinyCarController controller = player.GetComponent<PlayerSetup>().carController.GetComponent<TinyCarController>();
                    controller.setPlayerInControl(false);

                    // assign spawn Point
                    GameObject spawnPoint = GetSpawnByTeamAndName(team, spawnPositionName);
                    AssignSpawnPoint(team, spawnPoint);
                    PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();
                    playerSetup.SetSpawnPoint(spawnPoint);

                    PhotonView _photonView = player.GetComponent<PhotonView>();
                    _photonView.ViewID = (int)data[2];
                    
                    // updates local data
                    gameManager.SetPlayerJoined(team, photonPlayer.NickName, player);
                }
            }
        }
        #endregion

        #region Private Methods
        // Method for local Player, not when other clients spawned!
        public GameObject SpawnPlayer()
        {
            Debug.Log("SpawnPlayer:: ENTER SpawnPlayer()");
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                // get Team of player
                int team = (int)PhotonNetwork.LocalPlayer.CustomProperties["team"];
                
                // get available spawnPoint and set spawnPointName to properties
                GameObject spawnPoint = AssignSpawnPoint(team);
                PhotonNetwork.LocalPlayer.CustomProperties["spawnPoint"] = spawnPoint.name;
                
                // instantiate player
                GameObject playerGameobject = Instantiate(playerPrefabs[(int)playerSelectionNumber], spawnPoint.transform.position, spawnPoint.transform.rotation);
                Vector3 scale = playerGameobject.transform.localScale;
                playerGameobject.transform.SetParent(carsContainer.transform);
                playerGameobject.transform.localScale = scale;
                
                // set player IS in control over his car in this side of client
                // necessary for UI steering
                TinyCarController controller = playerGameobject.GetComponent<PlayerSetup>().carController.GetComponent<TinyCarController>();
                controller.setPlayerInControl(true);

                // save spawnPoint
                PlayerSetup playerSetup = playerGameobject.GetComponent<PlayerSetup>();
                playerSetup.SetSpawnPoint(spawnPoint);
                gameManager.SetPlayerJoined(team, PhotonNetwork.LocalPlayer.NickName, playerGameobject);
                
                PhotonView _photonView = playerGameobject.GetComponent<PhotonView>();

                if (PhotonNetwork.AllocateViewID(_photonView))
                {
                    // set photonViewID to properties and save properties in network
                    PhotonNetwork.LocalPlayer.CustomProperties["photonViewID"] = _photonView.ViewID;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
                    return playerGameobject;
                }
                else
                {

                    Debug.Log("Failed to allocate a viewID");
                    Destroy(playerGameobject);
                }
            }

            return null;
        }

        // should only be called once when player joins game
        public void SpawnOtherPlayers()
        {
            Dictionary<int, Photon.Realtime.Player> players = PhotonNetwork.CurrentRoom.Players;

            foreach (Photon.Realtime.Player player in players.Values)
            {
                if(player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    Hashtable props = player.CustomProperties;

                    int teamIndex = (int)props["team"];
                    int playerSelectionNumber = (int)props["Player_Selection_Number"];
                    int playerViewID = (int)props["photonViewID"];
                    string spawnPoint = (string)props["spawnPoint"];

                    // assign spawnpoint + remove from available spawn points list
                    GameObject spawnPointGameObject = GetSpawnByTeamAndName(teamIndex, spawnPoint);
                    AssignSpawnPoint(teamIndex, spawnPointGameObject);

                    // spawn other player
                    GameObject playerGameobject = Instantiate(playerPrefabs[playerSelectionNumber], spawnPointGameObject.transform.position, spawnPointGameObject.transform.rotation);
                    Vector3 scale = playerGameobject.transform.localScale;
                    playerGameobject.transform.SetParent(carsContainer.transform);
                    playerGameobject.transform.localScale = scale;
                    
                    // set player is not in control over his car in this side of client
                    // necessary to overwrite rb.rotation
                    TinyCarController controller = playerGameobject.GetComponent<PlayerSetup>().carController.GetComponent<TinyCarController>();
                    controller.setPlayerInControl(false);

                    // assign spawnpoint
                    PlayerSetup playerSetup = playerGameobject.GetComponent<PlayerSetup>();
                    playerSetup.SetSpawnPoint(spawnPointGameObject);
                    gameManager.SetPlayerJoined(teamIndex, player.NickName, playerGameobject);
                    
                    // set viewID
                    var _photonView = playerGameobject.GetComponent<PhotonView>();
                    _photonView.ViewID = playerViewID;
                }
            }
        }

        private float CalcFieldGroundOffset()
        {
            MeshRenderer renderer = gameFieldGameObject.GetComponent<MeshRenderer>();
            return Mathf.Min(renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z);
        } 
        #endregion

        #region Public Methods
        public GameObject[][] GetAllSpawnPoints()
        {
            return spawnPoints;
        }

        public HashSet<GameObject>[] GetAvailableSpawnPoints()
        {
            return availableSpawnPoints;
        }

        public void RaiseNewPlayerEvent(GameObject player)
        {
            PhotonView _photonView = player.GetComponent<PhotonView>();

            int playerId = PhotonNetwork.LocalPlayer.ActorNumber;

            object[] data = {
                player.transform.position - battleArenaGameobject.transform.position, player.transform.rotation, _photonView.ViewID, playerId
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            //Raise Events!
            PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode, data, raiseEventOptions, sendOptions);
        }
        #endregion


    }
}