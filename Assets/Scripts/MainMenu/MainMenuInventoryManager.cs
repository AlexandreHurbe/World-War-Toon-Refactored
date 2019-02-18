using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA {

    public class MainMenuInventoryManager : MonoBehaviour
    {
        public new Transform camera;
        public Transform[] cameraPositions;
        public int camIndex;
        public float speed = 5f;
        

        private float t;
        private Vector3 startPos;
        private Vector3 endPos;
        private Quaternion startRot;
        private Quaternion endRot;
        private bool isInit;
        private bool isLerping;

        private CategorySelection currentCat = CategorySelection.none;

        private int clothIndex;
        private int primaryWeaponIndex;
        private int secondaryWeaponIndex;
        List<ClothItem> clothes;
        List<Weapon> weapons;

        public StateManager offlineState;

        //Primary Weapon in Inventory
        public Transform primaryWeaponParent;
        private GameObject primaryWeaponObject;

        //Secondary Weapon in Inventory
        public Transform secondaryWeaponParent;
        private GameObject secondaryWeaponObject;


        public enum CategorySelection
        {
            none,
            clothes,
            primaryWeapon,
            secondaryWeapon
        }

        private void OnEnable()
        {
            camera.position = cameraPositions[0].position;
            camera.rotation = cameraPositions[0].rotation;
            currentCat = CategorySelection.none;

            clothes = GameManagers.GetResourcesManager().GetAllClothItems();
            weapons = GameManagers.GetResourcesManager().GetAllWeapons();

            string modelId = GameManagers.GetProfile().modelId;
            string primaryWeapon = GameManagers.GetProfile().itemIds[0];
            string secondaryWeapon = GameManagers.GetProfile().itemIds[1];

            for (int i = 0; i < clothes.Count; i++)
            {
                if (string.Equals(clothes[i].name, modelId))
                {
                    clothIndex = i;
                    break;
                }
            }

            for (int i = 0; i < weapons.Count; i++)
            {
                if (string.Equals(weapons[i].name, primaryWeapon))
                {
                    primaryWeaponIndex = i;
                    break;
                }

                if (string.Equals(weapons[i].name, secondaryWeapon))
                {
                    secondaryWeaponIndex = i;
                    break;
                }
            }

            offlineState.LoadCharacterModel(clothes[clothIndex].name);

            if (primaryWeaponObject != null)
            {
                Destroy(primaryWeaponObject);
            }
            primaryWeaponObject = CreateWeapon(weapons[primaryWeaponIndex], primaryWeaponParent);

            if (secondaryWeaponObject != null)
            {
                Destroy(secondaryWeaponObject);
            }
            secondaryWeaponObject = CreateWeapon(weapons[secondaryWeaponIndex], secondaryWeaponParent);
        }

        public void AssignCameraPosition(int index)
        {
            switch (index)
            {
                case 0:
                    currentCat = CategorySelection.none;
                    break;
                case 1:
                    currentCat = CategorySelection.primaryWeapon;
                    break;
                case 2:
                    currentCat = CategorySelection.secondaryWeapon;
                    break;
                case 4:
                    currentCat = CategorySelection.clothes;
                    break;
                default:
                    break;
            }

            camIndex = index;
            isLerping = true;
            isInit = false;
        }


        public void PreviousOrNextItem(bool prev)
        {
            int modifier = (prev) ? -1 : 1;

            switch (currentCat)
            {
                case CategorySelection.none:
                    break;

                case CategorySelection.clothes:
                    clothIndex += modifier;
                    if (clothIndex > clothes.Count - 1)
                    {
                        clothIndex = 0;
                    }
                    if (clothIndex < 0)
                    {
                        clothIndex = clothes.Count - 1;
                    }
                    GameManagers.GetProfile().modelId = clothes[clothIndex].name;
                    offlineState.LoadCharacterModel(clothes[clothIndex].name);
                    break;

                case CategorySelection.primaryWeapon:
                    primaryWeaponIndex += modifier;
                    if (primaryWeaponIndex > weapons.Count - 1)
                    {
                        primaryWeaponIndex = 0;
                    }
                    if (primaryWeaponIndex < 0)
                    {
                        primaryWeaponIndex = weapons.Count - 1;
                    }
                    GameManagers.GetProfile().itemIds[0] = weapons[primaryWeaponIndex].name;

                    if (primaryWeaponObject != null)
                    {
                        Destroy(primaryWeaponObject);
                    }
                    primaryWeaponObject = CreateWeapon(weapons[primaryWeaponIndex], primaryWeaponParent);

                    break;

                case CategorySelection.secondaryWeapon:
                    secondaryWeaponIndex += modifier;
                    if (secondaryWeaponIndex > weapons.Count - 1)
                    {
                        secondaryWeaponIndex = 0;
                    }
                    if (secondaryWeaponIndex < 0)
                    {
                        secondaryWeaponIndex = weapons.Count - 1;
                    }

                    if (secondaryWeaponObject != null)
                    {
                        Destroy(secondaryWeaponObject);
                    }
                    secondaryWeaponObject = CreateWeapon(weapons[secondaryWeaponIndex], secondaryWeaponParent);

                    GameManagers.GetProfile().itemIds[1] = weapons[secondaryWeaponIndex].name;
                    break;

                default:
                    break;
            }
        }

        private void Update()
        {
            MoveCameraToPosition();
        }


        private void MoveCameraToPosition()
        {
            if (!isLerping)
            {
                return;
            }

            if (!isInit)
            {
                startPos = camera.position;
                endPos = cameraPositions[camIndex].position;
                startRot = camera.rotation;
                endRot = cameraPositions[camIndex].rotation;
                isInit = true;
                t = 0;
            }

            t += Time.deltaTime * speed;
            if (t > 1)
            {
                t = 1;
                isInit = false;
                isLerping = false;
            }

            Vector3 tp = Vector3.Lerp(startPos, endPos, t);
            Quaternion tr = Quaternion.Slerp(startRot, endRot, t);

            camera.position = tp;
            camera.rotation = tr;

        }

        private GameObject CreateWeapon(Weapon w, Transform p)
        {
            GameObject go = Instantiate(w.modelPrefab, p.position, p.rotation, p);
            go.transform.GetChild(0).localPosition = Vector3.zero;
            go.transform.GetChild(0).localEulerAngles = Vector3.zero;
            
            return go;
        }
    }
}