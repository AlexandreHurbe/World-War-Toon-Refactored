using UnityEngine;
using System.Collections;
namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Cover Shooting Animations")]
    public class CoverShootingAnimations : StateActions
    {
        public override void Execute(StateManager states)
        {
            if (states.isAiming)
            {
                
                states.isInteracting = true;
                states.anim.SetBool(states.hashes.isInteracting, true);
            }
            else
            {
                states.isInteracting = false;
                states.anim.SetBool(states.hashes.isInteracting, false);
            }
            states.anim.SetBool(states.hashes.aiming, states.isAiming);
        }
    }

}

