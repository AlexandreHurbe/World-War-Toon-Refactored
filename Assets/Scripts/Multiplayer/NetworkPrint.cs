using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace SA
{
    public class NetworkPrint : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        public int photonId;
        public bool isLocal;

        string weapon;
        string modelId;


        void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
        {
            
            MultiplayerManager mm = MultiplayerManager.singleton;
            photonId = photonView.OwnerActorNr;
            isLocal = photonView.IsMine;

            //This data is then passed to Instantiate Controller-ish
            object[] data = photonView.InstantiationData;
            weapon = (string)data[0];
            modelId = (string)data[1];

            mm.AddNewPlayer(this);
        }

        public void InstantiateController(Vector3 pos, Quaternion r)
        {
            
            GameObject inputHandler = Instantiate(Resources.Load("InputHandler")) as GameObject;

            //This data is passed into the Multiplayer Listener-ish
            object[] data = new object[3];
            data[0] = photonId;
            data[1] = weapon;
            data[2] = modelId;

            GameObject go = PhotonNetwork.Instantiate("MultiplayerController", pos, r, 0, data);
            go.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

}
