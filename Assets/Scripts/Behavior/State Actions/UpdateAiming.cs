﻿using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Update Aiming")]
    public class UpdateAiming : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.anim.SetBool(states.hashes.aiming, states.isAiming);
        }
    }
}
