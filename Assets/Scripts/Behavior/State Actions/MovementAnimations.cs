using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Movement Animations")]
    public class MovementAnimations : StateActions
    {

        public string floatName;

        public override void Execute(StateManager states)
        {
            if (states.isAiming)
            {
                //These values have been hardcoded
                states.anim.SetFloat(floatName, states.movementValues.vertical, 0.1f, states.delta);
                states.anim.SetFloat("horizontal", states.movementValues.horizontal, 0.1f, states.delta);
            }
            else
            {
                states.anim.SetFloat(floatName, states.movementValues.moveAmount, 0.1f, states.delta);
            }
            
        }
    }
}

