using UnityEngine;
using System.Collections;


namespace SA {

    [CreateAssetMenu (menuName = "Conditions/Monitor Entering Cover")]
    public class MonitorCover : Condition
    {
        public float origin1Offset = 0.75f;
        public float origin2Offset = 0.2f;
        public float rayForwardDis = 1;
        public float rayHigherForwardDis = 1;

        public AnimationClip enterCoverStandingClip;
        public AnimationClip enterCoverCrouchingClip;

        public override bool CheckCondition(StateManager states)
        {
            bool result = false;
            states.canEnterCover = result;

            RaycastHit hit;
            Vector3 origin = states.mTransform.position;
            origin.y += origin1Offset;
            Vector3 direction = states.mTransform.forward;

            Debug.DrawRay(origin, direction * rayForwardDis, Color.blue);

            if (Physics.Raycast(origin, direction, out hit, rayForwardDis, states.ignoreLayers))
            {
                result = true;

                Vector3 origin2 = origin;
                origin2.y += origin2Offset;

                Vector3 firstHit = hit.point;
                firstHit.y -= origin1Offset;
                Vector3 normalDir = -hit.normal;
                Debug.DrawRay(origin2, direction * rayForwardDis, Color.cyan);

                if (Physics.Raycast(origin2, direction, out hit, rayForwardDis, states.ignoreLayers))
                {
                    states.coverData.canStand = true;
                }

                if (states.coverState == StateManager.CoverState.isWantingToEnterCover)
                {
                    states.anim.SetBool(states.hashes.isInteracting, true);
                    if (states.coverData.canStand)
                    {
                        states.anim.CrossFade(states.hashes.isEnteringCoverStanding, 0.15f);
                        states.coverData.animLength = enterCoverStandingClip.length;
                    }
                    else
                    {
                        states.anim.CrossFade(states.hashes.isEnteringCoverCrouching, 0.15f);
                        states.coverData.animLength = enterCoverCrouchingClip.length;
                    }

                    states.coverData.isEnteringCoverInit = false;
                    //states.isEnteringCover = true;
                    states.coverState = StateManager.CoverState.isEnteringCover;
                    states.coverData.startPosition = states.mTransform.position;
                    //Debug.Log("The object hit was: " + hit.point.GetType());
                    Vector3 endPosition = firstHit;
                    endPosition = firstHit - (states.mTransform.forward * (states.coverData.coverPosOffset));
                    //endPosition += normalDir * vaultOffsetPosition;
                    endPosition.y = states.mTransform.position.y;
                    states.coverData.endPosition = endPosition;
                    Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                    Quaternion finalRotation = Quaternion.RotateTowards(states.mTransform.rotation, targetRotation, float.PositiveInfinity);
                    finalRotation.x = Mathf.Abs(finalRotation.x);
                    states.coverData.startRotation = states.mTransform.rotation;

                    states.coverData.endRotation = finalRotation;
                    

                    return true;
                }
                else
                {
                    states.canEnterCover = result;
                }

            }
            return false;
 
        }

        
    }
}


 