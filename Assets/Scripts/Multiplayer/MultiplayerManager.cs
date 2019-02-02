﻿using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace SA {

    public class MultiplayerManager : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        private MultiplayerReferences mRef;

        public static MultiplayerManager singleton;

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
            GameObject go = PhotonNetwork.Instantiate("NetworkPrint", Vector3.zero, Quaternion.identity, 0) as GameObject;
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

        #endregion

        #region RPCs
        [PunRPC]
        public void RPC_SceneChange()
        {
           ///TODO: Set spawn positions from master
           MultiplayerLauncher.singleton.LoadCurrentSceneActual(CreateController);
        }

        //[PunRPC]
        //public void RPC_CreateControllers()
        //{
        //    mRef.localPlayer.print.InstantiateController(mRef.localPlayer.spawnPosition);
        //}

        [PunRPC]
        public void RPC_SetSpawnPositionForPlayer(int photonId, int spawnPosition)
        {
            if (photonId == mRef.localPlayer.photonId)
            {
                mRef.localPlayer.spawnPosition = spawnPosition;
            }
        }
        #endregion

       

        public MultiplayerReferences GetMRef()
        {
            return mRef;
        }


    }
}

