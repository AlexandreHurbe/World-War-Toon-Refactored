﻿using UnityEngine;
using System.Collections;

namespace SA {
    [CreateAssetMenu(menuName = "Actions/State Actions/Movement Animations All")]
    public class UpdateMovementAnimationsAll : StateActions
    {
        public override void Execute(StateManager states)
        {
            if (states.isAiming)
            {
                Debug.Log("Player is aiming");
                states.anim.SetFloat(states.hashes.horizontal, states.movementValues.horizontal, 0.1f, states.delta);
                states.anim.SetFloat(states.hashes.vertical, states.movementValues.vertical, 0.1f, states.delta);

            }
            else
            {
                states.anim.SetFloat(states.hashes.vertical, states.movementValues.vertical, 0.1f, states.delta);
            }
        }
    }


}