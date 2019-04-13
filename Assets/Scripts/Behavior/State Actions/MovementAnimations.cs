using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Movement Animations")]
    public class MovementAnimations : StateActions
    {

        //public string floatName;

        public override void Execute(StateManager states)
        {
            if (states.isAiming || states.autoAim)
            {
                Debug.Log("Aiming animations");
                states.anim.SetFloat(states.hashes.vertical, states.movementValues.vertical, 0.1f, states.delta);
                states.anim.SetFloat(states.hashes.horizontal, states.movementValues.horizontal, 0.1f, states.delta);
            }
            else
            {
                states.anim.SetFloat(states.hashes.vertical, states.movementValues.moveAmount, 0.1f, states.delta);
            }
            
        }
    }
}

