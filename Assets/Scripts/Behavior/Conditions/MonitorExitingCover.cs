using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu (menuName = "Conditions/Monitor Leaving Cover")]
    public class MonitorExitingCover : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
           
            if (state.coverState == StateManager.CoverState.isWantingToLeaveCover){
                
                state.anim.CrossFade(state.hashes.locomotionNormal, 0.15f);
                state.coverState = StateManager.CoverState.none;
                state.anim.SetBool(state.hashes.isInteracting, false);
                return true;
            }
            else
            {
                
                return false;
            }
        }
    }
}
