﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu (menuName = "Inputs/ Input Manager")]
    public class InputManager : Action
    {
        public SO.BoolVariable autoAim;
        public SO.BoolVariable combinedAim;
        public InputAxis horizontal;
        public InputAxis vertical;
        public InputButton aimInput;
        public InputButton shootInput;
        public InputButton crouchInput;
        public InputButton reloadInput;
        public InputButton sprintInput;
        public InputButton vaultInput;
        public InputButton coverInput;


        public float moveAmount;
        public Vector3 moveDir;

        

        public SO.TransformVariable cameraTransform;
        public SO.TransformVariable pivotTransform;

        public StatesVariable playerStates;

        public override void Execute()
        {
            
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal.value) + Math.Abs(vertical.value));

            if (cameraTransform.value != null)
            {
                moveDir = cameraTransform.value.forward * vertical.value;
                moveDir += cameraTransform.value.right * horizontal.value;
            }

            if (playerStates.value != null)
            {
                playerStates.value.movementValues.horizontal = horizontal.value;
                playerStates.value.movementValues.vertical = vertical.value;
                playerStates.value.movementValues.moveAmount = moveAmount;
                playerStates.value.movementValues.moveDirection = moveDir;
                

                
                playerStates.value.isShooting = shootInput.isPressed;

                playerStates.value.isAiming = aimInput.isPressed;

                if (!aimInput.isPressed) {
                    autoAim.value = shootInput.isPressed;
                    playerStates.value.autoAim = shootInput.isPressed;
                }
                else {
                    autoAim.value = false;
                    playerStates.value.autoAim = false;
                }

                

                playerStates.value.isWantingToVault = vaultInput.isPressed;
                //Debug.Log("Vault input pressed: " + vaultInput.isPressed);
                //playerStates.value.isWantingToEnterCover = coverInput.isPressed;

                if (coverInput.isPressed)
                {

                    if (playerStates.value.coverState == StateManager.CoverState.isInCover)
                    {
                        playerStates.value.coverState = StateManager.CoverState.isWantingToLeaveCover;
                        Debug.Log("player is in cover and wants to leave now");
                    }
                    else if(playerStates.value.coverState == StateManager.CoverState.isEnteringCover || playerStates.value.coverState == StateManager.CoverState.isLeavingCover)
                    {
                        return;
                    }
                    else if (playerStates.value.canEnterCover)
                    {
                        playerStates.value.coverState = StateManager.CoverState.isWantingToEnterCover;
                        Debug.Log("player wants to enter cover");
                    }
                    else
                    {
                        return;

                    }
                    
                }


                if (crouchInput.isPressed)
                {
                    playerStates.value.SetCrouching();
                    crouchInput.targetBoolVariable.value = playerStates.value.isCrouching; 
                }

                if (reloadInput.isPressed)
                {
                    playerStates.value.SetReloading();  
                }
                reloadInput.targetBoolVariable.value = playerStates.value.isReloading;


                SetSprinting(sprintInput.isPressed);

                //Sets this for the animator
                combinedAim.value = (playerStates.value.isAiming || autoAim.value);

                if (cameraTransform.value != null)
                {
                    playerStates.value.movementValues.lookDirection = cameraTransform.value.forward;
                }

                if (pivotTransform.value != null)
                {
                    Ray ray = new Ray(pivotTransform.value.position, pivotTransform.value.forward);
                    playerStates.value.movementValues.aimPosition = ray.GetPoint(100);
                }
                
            }
        }

        private void SetSprinting(bool active) {
            if (active) {
                //Sprint settings
                sprintInput.targetBoolVariable.value = true;
                playerStates.value.isSprinting = true;
                //Shoot settings
                shootInput.targetBoolVariable.value = false;
                playerStates.value.isShooting = false;
                //Aim settings
                aimInput.targetBoolVariable.value = false;
                playerStates.value.isAiming = false;
                //Auto settings
                autoAim.value = false;
                playerStates.value.autoAim = false;
            }

            else {
                sprintInput.targetBoolVariable.value = false;
                playerStates.value.isSprinting = false;
            }
        }

    }
}
