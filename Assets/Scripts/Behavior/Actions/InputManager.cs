using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu (menuName = "Inputs/ Input Manager")]
    public class InputManager : Action
    {
        //public bool debugAim;

        public InputAxis horizontal;
        public InputAxis vertical;
        public InputButton aimInput;
        public InputButton shootInput;
        public InputButton crouchInput;
        public InputButton reloadInput;

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
    }
}
