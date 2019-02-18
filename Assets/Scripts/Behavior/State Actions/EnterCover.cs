using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Enter Cover")]
    public class EnterCover : StateActions
    {
        public override void Execute(StateManager states)
        {
            if (states.coverState == StateManager.CoverState.isEnteringCover)
            {
                
                //Debug.Log("Player wants to and is entering cover");

                CoverData coverData = states.coverData;
                //Debug.Log("Cover states is active");
                //Debug.Log(coverData.enterCoverRotation);

                Debug.DrawRay(states.mTransform.position, states.mTransform.right * 10f, Color.red);
                Debug.DrawRay(states.mTransform.position, -states.mTransform.right * 10f, Color.green);
                Debug.DrawRay(states.mTransform.position, states.mTransform.forward * 10f, Color.blue);

                if (!coverData.isEnteringCoverInit)
                {
                    coverData.isEnteringCoverInit = true;
                    coverData.enterCoverPosT = 0;
                    coverData.enterCoverRotT = 0;
                    
                }

                coverData.enterCoverPosT += states.delta * coverData.enterCoverSpeed;
                coverData.enterCoverRotT += states.delta * coverData.enterCoverRotation;
                //Debug.Log(coverData.enterCoverPosT);

                //Use to be enterCoverPosT
                if (coverData.enterCoverPosT > 1)
                {
                    states.coverState = StateManager.CoverState.isInCover;
                    states.mTransform.rotation = coverData.endRotation;
                    //states.mTransform.rotation = states.coverData.endRotation;
                    Debug.Log("Player is in cover");
                    //states.mTransform.rotation = coverData.endRotation;
                    //states.rigidbody.velocity = Vector3.zero;
                    //states.rigidbody.isKinematic = true;
                    
                }
                else
                {
                    
                    Vector3 targetPosition = Vector3.Lerp(coverData.startPosition, coverData.endPosition, coverData.enterCoverPosT);
                    //Quaternion targetRotation = Quaternion.Slerp(coverData.startRotation, coverData.endRotation, coverData.enterCoverRotT);
                    states.mTransform.position = targetPosition;
                    //states.mTransform.rotation = targetRotation;
                }
                
            }
        }
    }
}
