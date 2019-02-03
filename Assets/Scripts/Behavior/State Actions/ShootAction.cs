using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName ="Actions/State Actions/Shoot Action")]
    public class ShootAction : StateActions
    {


        public override void Execute(StateManager states)
        {
            if (states.isReloading)
            {
                if (states.inventory.currentWeapon.currentBullets < states.inventory.currentWeapon.magazineBullets 
                    && states.inventory.currentWeapon.ammoType.carryingAmount > 0)
                {
                    if (!states.isInteracting)
                    {
                        states.reloadingFlag = true;
                        states.isInteracting = true;
                        states.PlayAnimation("Rifle Reload");
                        states.anim.SetBool("isInteracting", true);
                    }
                    else
                    {
                        if (!states.anim.GetBool("isInteracting"))
                        {
                            states.isReloading = false;
                            states.isInteracting = false;
                            ReloadCurrentWeapon(states.inventory.currentWeapon);
                        }
                    }

                    return;
                }
                else
                {
                    states.isReloading = false;
                }
                
            }


            if (states.isShooting)
            {
                states.shootingFlag = true;
                states.isShooting = false;
                Weapon w = states.inventory.currentWeapon;

                if (w.currentBullets > 0)
                {
                    if (Time.realtimeSinceStartup - w.runtime.weaponHook.lastFired > w.fireRate)
                    {
                        w.runtime.weaponHook.lastFired = Time.realtimeSinceStartup;
                        w.runtime.weaponHook.Shoot();
                        states.animHook.RecoilAnim();

                        if (!w.overrideBallistics)
                        {
                            if (states.ballisticsAction != null)
                            {
                                states.ballisticsAction.Execute(states, w);
                            }
                        }
                        else
                        {
                            w.ballistics.Execute(states, w);
                        }


                        w.currentBullets--;
                        if (w.currentBullets < 0)
                        {
                            w.currentBullets = 0;
                        }
                    }
                }
                else
                {
                    states.isReloading = true;
                }

                
            }
        }

        public void ReloadCurrentWeapon(Weapon currentWeapon)
        {
            currentWeapon.ammoType.carryingAmount += currentWeapon.currentBullets;
            if (currentWeapon.ammoType.carryingAmount > currentWeapon.magazineBullets)
            {
                currentWeapon.currentBullets = currentWeapon.magazineBullets;
                currentWeapon.ammoType.carryingAmount -= currentWeapon.currentBullets;
            }
            else if (currentWeapon.ammoType.carryingAmount > 0)
            {
                currentWeapon.currentBullets = currentWeapon.ammoType.carryingAmount;
                currentWeapon.ammoType.carryingAmount = 0;

            }
            else
            {
                Debug.Log("Completely out of ammo");
            }

            //int target = currentWeapon.magazineBullets;
            //if (target > currentWeapon.ammoType.carryingAmount)
            //{
            //    target = currentWeapon.magazineBullets - currentWeapon.ammoType.carryingAmount;

            //}
            //currentWeapon.ammoType.carryingAmount -= target;
            //currentWeapon.currentBullets = target;
        }
    }
}

