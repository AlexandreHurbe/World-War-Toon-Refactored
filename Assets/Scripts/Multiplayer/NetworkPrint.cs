using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace SA
{
    public class NetworkPrint : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        public int photonId;
        public bool isLocal;

        void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
        {
            MultiplayerManager mm = MultiplayerManager.singleton;
            //photonId = photonView.ownerID;
            //photonId = photonView.ViewID;
            photonId = photonView.OwnerActorNr;
            isLocal = photonView.IsMine;
            Debug.Log("OnPhotonInstantiate called");
            mm.AddNewPlayer(this);
        }

        public void InstantiateController(int spawnIndex)
        {
            GameObject inputHandler = Instantiate(Resources.Load("InputHandler")) as GameObject;
            PhotonNetwork.Instantiate("MultiplayerController", Vector3.zero, Quaternion.identity, 0);

        }
    }

}
