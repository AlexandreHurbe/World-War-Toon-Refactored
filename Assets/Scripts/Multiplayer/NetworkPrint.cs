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
            Debug.Log("(3) NetworkPrint: OnPhotonInstantiate called");
            MultiplayerManager mm = MultiplayerManager.singleton;
            //photonId = photonView.ownerID;
            //photonId = photonView.ViewID;
            photonId = photonView.OwnerActorNr;
            isLocal = photonView.IsMine;

            mm.AddNewPlayer(this);
        }

        public void InstantiateController(Vector3 pos, Quaternion r)
        {
            
            GameObject inputHandler = Instantiate(Resources.Load("InputHandler")) as GameObject;
            object[] data = new object[2];
            data[0] = photonId;
            data[1] = photonView.InstantiationData[0];

            GameObject go = PhotonNetwork.Instantiate("MultiplayerController", pos, r, 0, data);
            go.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

}
