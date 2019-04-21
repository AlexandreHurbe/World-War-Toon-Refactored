using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {

    [CreateAssetMenu (menuName = "Items/Weapon")]
    public class Weapon : Item
    {
        public int currentBullets = 30;
        public int magazineBullets = 30;
        public float fireRate = 0.2f;
        public Ammo ammoType;

        public SO.Vector3Variable rightHandPosition;
        public SO.Vector3Variable rightHandEulers;
        public GameObject modelPrefab;

        public RuntimeWeapon runtime;

        public AnimationCurve recoilY;
        public AnimationCurve recoilZ;

        public bool overrideBallistics;
        public Ballistics ballistics;

        public AudioClip shootAudio;
        public AudioClip reloadAudio;


        public void Init()
        {
            runtime = new RuntimeWeapon();
            runtime.modelInstance = Instantiate(modelPrefab) as GameObject;
            runtime.weaponHook = runtime.modelInstance.GetComponent<WeaponHook>();
            runtime.weaponHook.Init(shootAudio, reloadAudio);

            ammoType = GameManagers.GetAmmoPool().GetAmmo(ammoType.name);
        }

        
        public class RuntimeWeapon
        {

            public GameObject modelInstance;
            public WeaponHook weaponHook;

        }
    }

    
}


