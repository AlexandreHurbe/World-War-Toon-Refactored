using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;

namespace SA {

    public class MatchMakingManager : MonoBehaviour
    {

        public Transform spawnParent;

        List<MatchSpawnPosition> spawnPos = new List<MatchSpawnPosition>();
        
        //This function not be neccessary thanks to PUN2
        Dictionary<string, RoomButton> roomsDict = new Dictionary<string, RoomButton>();

        public static MatchMakingManager singleton;
        public Transform matchesParent;
        public GameObject matchPrefab;

        private void Awake()
        {
            singleton = this;
        }

        // Use this for initialization
        private void Start()
        {
            Transform[] p = spawnParent.GetComponentsInChildren<Transform>();

            foreach (Transform t in p)
            {
                if (t != spawnParent)
                {
                    MatchSpawnPosition m = new MatchSpawnPosition();
                    m.pos = t;

                    spawnPos.Add(m);
                }
            }
        }

        //This function not be neccessary thanks to PUN2
        private RoomButton GetRoomFromDict(string id)
        {
            RoomButton result = null;
            roomsDict.TryGetValue(id, out result);
            return result;
        }
        //This function not be neccessary thanks to PUN2
        public void AddMatches(List<RoomInfo> rooms)
        {
            SetDirtyRooms();

            foreach (RoomInfo room in rooms)
            {
                RoomButton createdRoom = GetRoomFromDict(room.Name);
                Debug.Log(createdRoom);
                if (createdRoom == null)
                {
                    Debug.Log("New match found");
                    AddMatch(room);
                }
                else
                {
                    Debug.Log("Match is still there");
                    createdRoom.isValid = true;
                }
            }

            ClearNonValidRooms();
        }

        private void SetDirtyRooms()
        {
            List<RoomButton> allRooms = new List<RoomButton>();
            allRooms.AddRange(roomsDict.Values);

            foreach (RoomButton r in allRooms)
            {
                
                r.isValid = false;
                Debug.Log(r.roomInfo.Name + " has been set to: " + r.isValid);
            }
        }
        
        private void ClearNonValidRooms()
        {
            List<RoomButton> allRooms = new List<RoomButton>();
            allRooms.AddRange(roomsDict.Values);

            foreach (RoomButton r in allRooms)
            {
                if (!r.isValid)
                {
                    Debug.Log(r.roomInfo.Name + " is not valid");
                    roomsDict.Remove(r.roomInfo.Name);
                    Destroy(r.gameObject);
                }
                
            }
        }

        public void AddMatch(RoomInfo roomInfo)
        {
            GameObject go = Instantiate(matchPrefab);
            go.transform.SetParent(matchesParent);

            MatchSpawnPosition p = GetSpawnPos();

            p.isUsed = true;
            go.transform.position = p.pos.position;
            go.transform.localScale = Vector3.one;

            RoomButton roomButton = go.GetComponent<RoomButton>();
            roomButton.roomInfo = roomInfo;
            roomButton.isRoomCreated = true;
            roomButton.isValid = true;

            roomButton.room = ScriptableObject.CreateInstance<Room>();

            object sceneObj = null;
            roomInfo.CustomProperties.TryGetValue("scene", out sceneObj);
            string sceneName = (string)sceneObj;

            roomButton.room.sceneName = sceneName;
            roomButton.room.roomName = roomInfo.Name;


            roomsDict.Add(roomInfo.Name, roomButton);
        }

        public MatchSpawnPosition GetSpawnPos()
        {
            List<MatchSpawnPosition> l = GetUnused();

            int random = Random.Range(0, l.Count);
            return l[random];
        }
       
        public List<MatchSpawnPosition> GetUnused()
        {
            List<MatchSpawnPosition> r = new List<MatchSpawnPosition>();
            
            for (int i = 0; i <spawnPos.Count; i++)
            {
                if (!spawnPos[1].isUsed)
                {
                    r.Add(spawnPos[i]);
                }
            }

            return r;
        }
    }

    public class MatchSpawnPosition
    {
        public Transform pos;
        public bool isUsed;

    }


}

