using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu (menuName = "Managers/Resources Manager")]
    public class ResourcesManager : ScriptableObject
    {
        public List<Item> allItems = new List<Item>();
        Dictionary<string, Item> itemDict;

        public RoomVariable currentRoom;

        public void Init()
        {
            itemDict = new Dictionary<string, Item>();

            Debug.Log("ALl items length: " + allItems.Count);
            //Debug.Log("itemDict length: +" + itemDict.Count);
            for (int i = 0; i < allItems.Count; i++)
            {
                Debug.Log(allItems[i].name);
                if (!itemDict.ContainsKey(allItems[i].name))
                {
                    itemDict.Add(allItems[i].name, allItems[i]);
                }
                else
                {
                    Debug.Log("Theres two items with name: " + allItems[i].name);
                }
            }
        }

        public Item GetItemInstance(string targetID)
        {
            Item defaultItem = GetItem(targetID);
            Item newItem = Instantiate(defaultItem);
            //Potentially uncomment the bottom part
            newItem.name = defaultItem.name;

            return newItem;
        }

        private Item GetItem(string targetID)
        {
            Item retVal = null;
            itemDict.TryGetValue(targetID, out retVal);
            return retVal;
        }
    }
}

