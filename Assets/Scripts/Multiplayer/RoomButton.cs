using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

namespace SA
{
    public class RoomButton : MonoBehaviour
    {
        public bool isRoomCreated;
        public Room room;
        public string scene = "level1";
        public RoomInfo roomInfo;
        public bool isValid;

        public void OnClick()
        {
            GameManagers.GetResourcesManager().currentRoom.SetRoomButton(this);
        }
    }
}
