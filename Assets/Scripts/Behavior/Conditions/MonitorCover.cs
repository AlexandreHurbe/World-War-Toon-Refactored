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

        private Vector3 flip = new Vector3(0, 0, -1);

        public override bool CheckCondition(StateManager states)
        {
            bool result = false;
            states.canEnterCover = result;

            RaycastHit hit;
            Vector3 origin = states.mTransform.position;
            origin.y += origin1Offset;
            Vector3 direction = states.movementValues.lookDirection;

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
                else
                {
                    states.coverData.canStand = false;
                }

                //If the player can enter cover and wants to
                if (states.coverState == StateManager.CoverState.isWantingToEnterCover)
                {
                    states.anim.SetBool(states.hashes.isInteracting, true);

                    //If the player is standing
                    if (states.coverData.canStand)
                    {
                        //Play the animation with movement
                        if (Vector3.Distance(states.mTransform.position, firstHit) > 0.4f)
                        {
                            Debug.Log("Distance longer than 0.4f");
                            states.anim.CrossFade(states.hashes.isEnteringCoverStanding, 0.15f);
                            states.coverData.animLength = enterCoverStandingClip.length;
                        }
                        //Just go straight into cover blend tree
                        else
                        {
                            states.anim.CrossFade(states.hashes.coverStandingLocomotionLeft, 0.05f);
                        }
                    }
                    //If the player is crouching
                    else
                    {
                        states.isCrouching = true;
                        if (Vector3.Distance(states.mTransform.position, firstHit) > 0.4f)
                        {
                            states.anim.CrossFade(states.hashes.isEnteringCoverCrouching, 0.15f);
                            states.coverData.animLength = enterCoverCrouchingClip.length;
                        }
                        else
                        {
                            states.anim.CrossFade(states.hashes.coverCrouchLocomotionLeft, 0.05f);
                        }
                        
                    }

                    states.coverData.isEnteringCoverInit = false;
                    
                    states.coverState = StateManager.CoverState.isEnteringCover;
                    states.coverData.startPosition = states.mTransform.position;
                    
                    Vector3 endPosition = firstHit;
                    endPosition = firstHit - (states.mTransform.forward * (states.coverData.coverPosOffset));
                    
                    endPosition.y = states.mTransform.position.y;
                    states.coverData.endPosition = endPosition;
                    Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                    Debug.Log(hit.normal);
                    Quaternion finalRotation = Quaternion.RotateTowards(states.mTransform.rotation, targetRotation, float.PositiveInfinity);
                    finalRotation.x = Mathf.Abs(finalRotation.x);
                    if (finalRotation.x == 1)
                    {
                        Debug.Log("rotations changed");
                        finalRotation.x = 0;
                        finalRotation.y = 180;
                    }
                    
                    Debug.Log(finalRotation.x);
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


 