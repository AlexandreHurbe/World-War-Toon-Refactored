using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    
    public class MultiplayerReferences
    {
        List<PlayerHolder> players = new List<PlayerHolder>();

        public PlayerHolder localPlayer;
        public Transform referencesParent;

        public MultiplayerReferences()
        {
            referencesParent = new GameObject().transform;
            referencesParent.name = "references";
        }

        public int GetPlayerCount()
        {
            return players.Count;
        }

        public List<PlayerHolder> GetPLayers()
        {
            return players;
        }

        public PlayerHolder AddNewPlayer(NetworkPrint p)
        {
            if (!IsUniquePlayer(p.photonId))
            {
                return null;
            }

            PlayerHolder playerHolder = new PlayerHolder
            {
                photonId = p.photonId,
                print = p
            };          
            players.Add(playerHolder);
            return playerHolder;
        }

        public PlayerHolder GetPlayer(int photonId)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].photonId == photonId)
                {
                    return players[i];
                }
            }

            return null;
        }

        public bool IsUniquePlayer(int id)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].photonId == id)
                {
                    Debug.Log("Not a unique player id");
                    return false;
                }
            }
            Debug.Log("ID given is unique"); 
            return true;
        }
    }

}
