using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Cover Movement Animations")]
    public class CoverMovementAnimations : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.anim.SetFloat(states.hashes.horizontal, states.movementValues.horizontal, 0.1f, states.delta);
        }
    }
}
