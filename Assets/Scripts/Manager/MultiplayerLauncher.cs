using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace SA
{
    public class MultiplayerLauncher : MonoBehaviourPunCallbacks
    {
        public delegate void OnSceneLoaded();
        public List<RoomInfo> roomInfoList = new List<RoomInfo>();
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<string, RoomInfo> cachedRoomList;
        public GameObject RoomListEntryPrefab;
        

        bool isLoading;
        bool isInGame;

        public static MultiplayerLauncher singleton;
        public PunLogLevel logLevel = PunLogLevel.ErrorsOnly;
        public int gameVersion = 1;
        public SO.GameEvent onConnectedToMaster;
        public SO.GameEvent onBackToMenuFromGame;
        public SO.GameEvent onJoinedRoom;
        public SO.BoolVariable isConnected;
        public SO.BoolVariable isMultiplayer;
        public SO.BoolVariable isWinner;

        #region Init
        private void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            isConnected.value = false;
            ConnectToServer();
        }

        public void ConnectToServer()
        {
            PhotonNetwork.LogLevel = logLevel;
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.ConnectUsingSettings();
        }

        #endregion

        #region Photon Callbacks
        public override void OnConnectedToMaster()
        {
            isConnected.value = true;
            onConnectedToMaster.Raise();
        }

        private void OnFailedToConnectToMasterServer()
        {
            isConnected.value = false;
            //Debug.Log(error.ToString());
           onConnectedToMaster.Raise();
        }

        public override void OnCreatedRoom()
        {
            Room r = ScriptableObject.CreateInstance<Room>();
            object sceneName;
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("scene", out sceneName);
            r.sceneName = (string) sceneName;
            r.roomName = PhotonNetwork.CurrentRoom.Name;
            Debug.Log("Roomname: " + r.roomName + " PhotonNetworkCurrentRoomName: " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("Scenename: " + r.sceneName);

            GameManagers.GetResourcesManager().currentRoom.value = r;
            Debug.Log("OnCreatedRoom");
        }

        public void JoinRoom(RoomInfo roomInfo)
        {
            PhotonNetwork.JoinRoom(roomInfo.Name);
            isInGame = true;
        }


        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom");
            onJoinedRoom.Raise();
            InstantiateMultiplayerManager();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby");
            //OnRoomListUpdate(roomList);
            //StartCoroutine(RoomCheck());
        }

        IEnumerator RoomCheck()
        {
            yield return new WaitForSeconds(5f);
            
            if (!isInGame)
            {

            }
        }


        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("roomList updating, number of lobbies found: " + roomList.Count);
            foreach (RoomInfo room in roomList)
            {
                if (room.IsOpen)
                {
                    Debug.Log("room is open");
                }
                else if (room.RemovedFromList || !room.IsOpen || !room.IsVisible)
                {
                    Debug.Log("Room has been removed from list");
                }
            }
            //UpdateRoomList(roomList);
            MatchMakingManager m = MatchMakingManager.singleton;
            m.AddMatches(roomList);
        }

        
        

        

        #endregion


        #region Manager Methods

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
        }

        private void InstantiateMultiplayerManager()
        {
            Debug.Log("(0) Multiplayer Launcher: InstantiateMultiplayerManager");
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate("MultiplayerManager", Vector3.zero, Quaternion.identity, 0);
            }
        }

        public void CreateRoom(RoomButton b)
        {
            if (isMultiplayer.value)
            {
                if (!isConnected.value)
                {
                    // Cannot connect to server but tried to acces multiplayer

                }
                else
                {
                    RoomOptions roomOptions = new RoomOptions();
                    roomOptions.MaxPlayers = 4;

                    ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
                    {
                        {"scene", b.scene }
                    };

                    roomOptions.CustomRoomPropertiesForLobby = new string[] { "scene" };
                    roomOptions.CustomRoomProperties = properties;

                    PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
                }
            }
            else
            {
                //This will be single player
                Room r = ScriptableObject.CreateInstance<Room>();
                r.sceneName = b.scene;
                GameManagers.GetResourcesManager().currentRoom.Set(r);
                
            }
        }

        public void LoadMainMenu()
        {
            StartCoroutine(LoadScene("MainMenu", OnMainMenu));
        }

        //Gets called by an event
        public void LoadCurrentRoom()
        {
            Debug.Log("(7) Multiplayer Launcher: LoadCurrentRoom called");
            if (isConnected)
            {
                MultiplayerManager.singleton.BroadcastSceneChange();
                
            }
            else
            {
                Room r = GameManagers.GetResourcesManager().currentRoom.value;
                if (!isLoading)
                {
                    isLoading = true;
                    StartCoroutine(LoadScene(r.sceneName));
                }
            }

            
            
        }

        public void LoadCurrentSceneActual(OnSceneLoaded callback = null)
        {
            Room r = GameManagers.GetResourcesManager().currentRoom.value;
            if (!isLoading)
            {
                isLoading = true;
                StartCoroutine(LoadScene(r.sceneName, callback));
            }
        }


        IEnumerator LoadScene(string targetLevel, OnSceneLoaded callback = null)
        {

            yield return SceneManager.LoadSceneAsync(targetLevel, LoadSceneMode.Single);
            isLoading = false;
            if (callback != null)
            {
                callback.Invoke();
            }
        }

        #endregion

        #region Setup Methods
        
        public void EndMatch(MultiplayerManager mm, bool isWinner)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }

            this.isWinner.value = isWinner;
            mm.ClearReferences();
            LoadMainMenuFromGame();
        }

        private void OnMainMenuLoadedCallback()
        {
            onBackToMenuFromGame.Raise();


            isWinner.value = false;
        }

        private void LoadMainMenuFromGame()
        {
            StartCoroutine(LoadScene("MainMenu", OnMainMenuLoadedCallback));
        }

        public void OnMainMenu()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            isConnected.value = PhotonNetwork.IsConnected;
            if (isConnected.value)
            {
                OnConnectedToMaster();
            }
            else
            {
                ConnectToServer();
            }
        }

        #endregion
    }
}

