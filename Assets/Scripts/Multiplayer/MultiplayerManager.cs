using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace SA {

    public class MultiplayerManager : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        private MultiplayerReferences mRef;

        public static MultiplayerManager singleton;

        public RayBallistics ballistics;

        void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
        {
            Debug.Log("Multiplayer Manager: OnPhotonInstantiate called");
            singleton = this;
            DontDestroyOnLoad(this.gameObject);

            mRef = new MultiplayerReferences();
            DontDestroyOnLoad(mRef.referencesParent.gameObject);
            InstantiateNetworkPrint();
        }

        private void InstantiateNetworkPrint()
        {
            PlayerProfile profile = GameManagers.GetProfile();
            //Size is 1 because there is only gun
            object[] data = new object[1];
            //0 index used for profile as the first item is gun
            data[0] = profile.itemIds[0];

            GameObject go = PhotonNetwork.Instantiate("NetworkPrint", Vector3.zero, Quaternion.identity, 0, data) as GameObject;
        }

        public void AddNewPlayer(NetworkPrint print)
        {
            print.transform.parent = mRef.referencesParent;
            PlayerHolder playerH = mRef.AddNewPlayer(print);
            if (print.isLocal)
            {
                mRef.localPlayer = playerH;
            }
        }


        #region MyCalls
        public void BroadcastSceneChange()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_SceneChange", RpcTarget.All);
            }
        }


        public void CreateController()
        {
            mRef.localPlayer.print.InstantiateController(mRef.localPlayer.spawnPosition);
        }

        public void BroadcastShootWeapon(StateManager states, Vector3 direction, Vector3 origin)
        {
            int photonId = states.photonId;
            photonView.RPC("RPC_ShootWeapon", RpcTarget.All, photonId, direction, origin);
        }

        #endregion

        #region RPCs
        [PunRPC]
        public void RPC_SceneChange()
        {
           ///TODO: Set spawn positions from master
           MultiplayerLauncher.singleton.LoadCurrentSceneActual(CreateController);
        }

        [PunRPC]
        public void RPC_SetSpawnPositionForPlayer(int photonId, int spawnPosition)
        {
            if (photonId == mRef.localPlayer.photonId)
            {
                mRef.localPlayer.spawnPosition = spawnPosition;
            }
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

        #endregion



        #region Get/Set Methods
        public MultiplayerReferences GetMRef()
        {
            return mRef;
        }


        #endregion

    }
}

