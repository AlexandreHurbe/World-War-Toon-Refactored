using UnityEngine;
using System.Collections;


namespace SA
{
    [CreateAssetMenu (menuName = "Actions/State Actions/Cover Shooting")]
    public class CoverShooting : StateActions
    {
        public override void Execute(StateManager states)
        {
            if (states.isAiming)
            {
                //
            }
        }
    }

}
