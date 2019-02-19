using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Cover Movement Animations")]
    public class CoverMovementAnimations : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.anim.SetBool(states.hashes.isCrouching, states.isCrouching);

            if (!states.coverData.atCorner)
            {
                
                states.anim.SetFloat(states.hashes.horizontal, states.movementValues.horizontal, 0.1f, states.delta);
            }
            else
            {
                states.anim.SetFloat(states.hashes.horizontal, 0, 0.05f, states.delta);
            }
            
        }
    }
}
