using UnityEngine;
using System.Collections;

namespace SA
{
    public class PlayerHolder
    {
        public int photonId;
        public string username;
        public int spawnPosition;
        public int health;
        public int killCount;
        public NetworkPrint print;
        public StateManager states;

        public float spawnTimer;

    }
}

