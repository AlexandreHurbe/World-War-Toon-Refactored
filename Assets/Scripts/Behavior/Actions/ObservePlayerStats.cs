﻿using UnityEngine;
using System.Collections;


namespace SA {

   
    [CreateAssetMenu(menuName = "Actions/Mono Actions/Observe Player Stats")]
    public class ObservePlayerStats : Action
    {
        public StatesVariable states;

        public SO.GameEvent healthUpdate;
        public SO.IntVariable playerHealth;
        

        public SO.GameEvent curAmmoUpdate;
        public SO.IntVariable curAmmo;

        public override void Execute()
        {
            if (states.value == null)
            {
                return;
            }

            if (states.value.healthChangedFlag)
            {
                states.value.healthChangedFlag = false;
                playerHealth.value = states.value.stats.health;
                healthUpdate.Raise();
            }

            if (curAmmo.value != states.value.inventory.currentWeapon.currentBullets)
            {
                
                curAmmo.value = states.value.inventory.currentWeapon.currentBullets;
                curAmmoUpdate.Raise();
            }
        }
    }
}
