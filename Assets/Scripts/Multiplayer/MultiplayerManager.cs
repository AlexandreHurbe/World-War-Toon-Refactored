using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

namespace SA {

    public class MultiplayerManager : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        private MultiplayerReferences mRef;

        public static MultiplayerManager singleton;

        public RayBallistics ballistics;

        private bool isMaster;

        private bool inGame;
        private bool endMatch;

        [SerializeField]
        private float startingTime = 10;
        private float currentTime;
        private float timerInterval;
        private float winKillCount = 20;

        [SerializeField]
        private SO.IntVariable timerInSeconds;
        [SerializeField]
        private SO.GameEvent timerUpdate;


        private List<PlayerHolder> playersToRespawn = new List<PlayerHolder>();

        void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
        {
            Debug.Log("(1) Multiplayer Manager: OnPhotonInstantiate called");
            singleton = this;
            DontDestroyOnLoad(this.gameObject);

            mRef = new MultiplayerReferences();
            DontDestroyOnLoad(mRef.referencesParent.gameObject);
            InstantiateNetworkPrint();
            currentTime = startingTime;

            isMaster = PhotonNetwork.IsMasterClient;
        }

        private void InstantiateNetworkPrint()
        {
            PlayerProfile profile = GameManagers.GetProfile();
            //Size is 1 because there is only gun
            object[] data = new object[1];
            //0 index used for profile as the first item is gun
            data[0] = profile.itemIds[0];
            Debug.Log("(2) MultiplayerManager: InstantiateNetworkPrint called");
            GameObject go = PhotonNetwork.Instantiate("NetworkPrint", Vector3.zero, Quaternion.identity, 0, data) as GameObject;
        }

        public void AddNewPlayer(NetworkPrint print)
        {
            Debug.Log("(4) MultiplayerManager: AddNewPlayerCalled called");
            print.transform.parent = mRef.referencesParent;
            PlayerHolder playerH = mRef.AddNewPlayer(print);
            if (print.isLocal)
            {
                mRef.localPlayer = playerH;
            }
        }

        private void Update()
        {
            if (endMatch)
            {
                return;
            }

            float delta = Time.deltaTime;

            if (inGame)
            {
                currentTime -= delta;
                timerInterval += delta;

                if (timerInterval > 1)
                {
                    timerInterval = 0;
                    timerInSeconds.value = Mathf.RoundToInt(currentTime);
                    timerUpdate.Raise();

                    if (isMaster)
                    {
                        photonView.RPC("RPC_BroadcastTime", RpcTarget.All, currentTime);
                    }

                }

                if (currentTime <= 0)
                {
                    if (isMaster)
                    {
                        TimerRunOut();
                    }
                }
            }
            
            if (!isMaster)
            {
                return;
            }

            for (int i = playersToRespawn.Count - 1; i >= 0; i--)
            {
                //Debug.Log("playersToRespawn.Count: " + playersToRespawn.Count);
                playersToRespawn[i].spawnTimer += delta;
                
                if (playersToRespawn[i].spawnTimer > 5)
                {
                    //Debug.Log("1");
                    playersToRespawn[i].spawnTimer = 0;
                    //Debug.Log("2");
                    playersToRespawn[i].health = 100;
                    //Debug.Log("3");
                    int ran = Random.Range(0, mRef.spawnPositions.Length);
                    //Debug.Log("4");
                    Vector3 pos = mRef.spawnPositions[ran].transform.position;
                    //Debug.Log("5");
                    Quaternion rot = mRef.spawnPositions[ran].transform.rotation;
                    //Debug.Log("6");
                    photonView.RPC("RPC_BroadcastPlayerHealth", RpcTarget.All, playersToRespawn[i].photonId, 100);
                    //Debug.Log("7");
                    photonView.RPC("RPC_SpawnPlayer", RpcTarget.All, playersToRespawn[i].photonId, pos, rot);
                    //Debug.Log("8");
                    playersToRespawn.RemoveAt(i);
                    //Debug.Log("9");
                    
                }
            }
        }


        #region MyCalls
        public void BroadcastSceneChange()
        {
            Debug.Log("MultiplayeManager: BroadcastSceneChange called");
            if (isMaster)
            {
                photonView.RPC("RPC_SceneChange", RpcTarget.All);
            }
        }

        public void TimerRunOut()
        {
            List<PlayerHolder> players = mRef.GetPlayers();
            int killCount = 0;
            int winnerId = -2;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].killCount > killCount)
                {
                    killCount = players[i].killCount;
                    winnerId = players[i].photonId;
                }
            }

            BroadcastMatchOver(winnerId);
        }

        public void LevelLoadedCallback()
        {
            Debug.Log("MultiplayeManager: LevelLoadedCallback called");
            //after scene is loaded
            if (isMaster)
            {
                FindSpawnPositionsOnLevel();
                AssignSpawnPositions();
            }

            inGame = true;
        }

        private void AssignSpawnPositions()
        {
            Debug.Log("MultiplayeManager: AssignSpawnPositions called");
            List<PlayerHolder> players = mRef.GetPlayers();

            for (int i = 0; i < players.Count; i++)
            {
                int index = i % mRef.spawnPositions.Length;
                SpawnPosition spawnPosition = mRef.spawnPositions[index];
                photonView.RPC("RPC_BroadcastCreateControllers", RpcTarget.All, players[i].photonId, spawnPosition.transform.position + (Vector3.up * 0.1f), spawnPosition.transform.rotation);
            }
        }

        public void BroadcastShootWeapon(StateManager states, Vector3 direction, Vector3 origin)
        {
            Debug.Log("MultiplayeManager: BroadcastShootWeapon called");
            int photonId = states.photonId;
            photonView.RPC("RPC_ShootWeapon", RpcTarget.All, photonId, direction, origin);
        }

        public void BroadcastKillPlayer(int photonId)
        {
            photonView.RPC("RPC_KillPlayer", RpcTarget.All, photonId);
            //photonView.RPC("RPC_ReceiveKillPlayer", RpcTarget.MasterClient, photonId, shooter);
        }

        public void ClearReferences()
        {
            if (mRef.referencesParent != null)
            {
                Destroy(mRef.referencesParent.gameObject);
                Destroy(this.gameObject);
            }
        }

        public void BroadCastPlayerIsHitBy(int photonId, int shooterId)
        {
            PlayerHolder p = mRef.GetPlayer(shooterId);
            p.killCount++;
            photonView.RPC("RPC_SyncKillCount", RpcTarget.All, shooterId, p.killCount);

            if (p.killCount >= winKillCount)
            {
                Debug.Log("Match Over");
                BroadcastMatchOver(shooterId);
            }
        }

        public void BroadcastMatchOver(int photonId)
        {
            photonView.RPC("RPC_BroadcastMatchOver", RpcTarget.All, photonId);
            endMatch = true;
        }

        public void BroadcastPlayerHealth(int photonId, int health, int shooter)
        {
            if (health <= 0)
            {
                playersToRespawn.Add(mRef.GetPlayer(photonId));
                BroadCastPlayerIsHitBy(photonId, shooter);
            }

            photonView.RPC("RPC_BroadcastPlayerHealth", RpcTarget.All, photonId, health);
        }

        public void FindSpawnPositionsOnLevel()
        {
            Debug.Log("MultiplayeManager: FindSpawnPositionsOnLevel called");
            mRef.spawnPositions = FindObjectsOfType<SpawnPosition>();
            
        }

        #endregion

        #region RPCs
        [PunRPC]
        public void RPC_BroadcastTime(float masterTime)
        {
            if (!isMaster)
            {
                this.currentTime = masterTime;
            }
        }


        [PunRPC]
        public void RPC_BroadcastMatchOver(int photonId)
        {
            bool isWinner = false;

            if (mRef.localPlayer.photonId == photonId)
            {
                isWinner = true;
                Debug.Log("Winner!");
            }

            MultiplayerLauncher.singleton.EndMatch(this, isWinner);
        }

        [PunRPC]
        public void RPC_BroadcastCreateControllers(int photonId, Vector3 pos, Quaternion rot)
        {
            if (photonId == mRef.localPlayer.photonId)
            {
                mRef.localPlayer.print.InstantiateController(pos, rot);
            }
        }

        [PunRPC]
        public void RPC_SyncKillCount(int photonId, int killCount)
        {
            if (photonId == mRef.localPlayer.photonId)
            {
                mRef.localPlayer.killCount = killCount;
            }
        }

        [PunRPC]
        public void RPC_BroadcastPlayerHealth(int photonId, int health)
        {
            
            PlayerHolder p = mRef.GetPlayer(photonId);
            p.health = health;

            if (p == mRef.localPlayer)
            {
                if (p.health <= 0)
                {
                    BroadcastKillPlayer(photonId);
                }
                    
            }

        }

        [PunRPC]
        public void RPC_SceneChange()
        {
            ///TODO: Set spawn positions from master
            MultiplayerLauncher.singleton.LoadCurrentSceneActual(LevelLoadedCallback);
        }

        [PunRPC]
        public void RPC_ShootWeapon(int photonId, Vector3 dir, Vector3 origin)
        {
            if (photonId == mRef.localPlayer.photonId)
            {
                return;
            }

            PlayerHolder shooter = mRef.GetPlayer(photonId);
            if (shooter == null)
            {
                return;
            }

            ballistics.ClientShoot(shooter.states, dir, origin);

        }

        [PunRPC]
        public void RPC_SpawnPlayer(int photonId, Vector3 targetPosition, Quaternion targetRot)
        {
            PlayerHolder p = mRef.GetPlayer(photonId);

            if (p.states != null)
            {
                p.states.SpawnPlayer(targetPosition, targetRot);
            }

        }

        //[PunRPC]
        //public void RPC_ReceiveKillPlayer(int photonId, int shooter)
        //{
        //    //Master client
        //    photonView.RPC("RPC_KillPlayer", RpcTarget.All, photonId, shooter);
        //    playersToRespawn.Add(mRef.GetPlayer(photonId));
        //}

        [PunRPC]
        public void RPC_KillPlayer(int photonId)
        {
            PlayerHolder p = mRef.GetPlayer(photonId);
            if (p.states != null)
            { 
                p.states.KillPlayer();          
            }
            else
            {
                Debug.LogError("Player holder is missing for: " + photonId); 
            }
        }

        #endregion

        #region Get/Set Methods
        public MultiplayerReferences GetMRef()
        {
            return mRef;
        }


        #endregion

    }
}

