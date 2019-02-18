using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Load Weapon")]
    public class LoadWeapon : StateActions
    {
        public override void Execute(StateManager states)
        {
            ResourcesManager resourcesManager = GameManagers.GetResourcesManager();

            Weapon targetWeapon = (Weapon) resourcesManager.GetItemInstance(states.inventory.weaponID);
            states.inventory.currentWeapon = targetWeapon;
            targetWeapon.Init();

            Transform rightHand = states.anim.GetBoneTransform(HumanBodyBones.RightHand);
            targetWeapon.runtime.modelInstance.transform.parent = rightHand;
            targetWeapon.runtime.modelInstance.transform.localScale = (Vector3.one * 100);
            targetWeapon.runtime.modelInstance.transform.localPosition = Vector3.zero;
            targetWeapon.runtime.modelInstance.transform.localEulerAngles = Vector3.zero;
            

            states.animHook.LoadWeapon(targetWeapon);
        }
    }
}

