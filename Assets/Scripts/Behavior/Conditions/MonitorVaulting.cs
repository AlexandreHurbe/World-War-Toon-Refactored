using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu (menuName = "Conditions/Monitor Vaulting")]
    public class MonitorVaulting : Condition
    {
        public float origin1Offset = 0.75f;
        public float rayForwardDis = 1;
        public float rayHigherForwardDis = 1;
        public float origin2Offset = 0.2f;
        public float rayDownDis = 1.5f;
        public float vaultOffsetPosition = 2;

        public AnimationClip vaultWalkClip;
        

        public override bool CheckCondition(StateManager states)
        {
            bool result = false;
            states.canVault = result;

            RaycastHit hit;
            Vector3 origin = states.mTransform.position;
            origin.y += origin1Offset;
            Vector3 direction = states.mTransform.forward;

            Debug.DrawRay(origin, direction * rayForwardDis);
            if (Physics.Raycast(origin, direction, out hit, rayForwardDis, states.ignoreLayers))
            {
                Vector3 origin2 = origin;
                origin2.y += origin2Offset;

                Vector3 firstHit = hit.point;
                firstHit.y -= origin1Offset;
                Vector3 normalDir = -hit.normal;

                Debug.DrawRay(origin2, direction * rayForwardDis);
                if (Physics.Raycast(origin2, direction, out hit, rayHigherForwardDis, states.ignoreLayers))
                {

                }
                else
                {
                    Vector3 origin3 = origin2 + (direction * rayHigherForwardDis);
                    Debug.DrawRay(origin3, -Vector3.up * rayDownDis);
                    if (Physics.Raycast(origin3, -Vector3.up, out hit, rayDownDis, states.ignoreLayers))
                    {
                        //Ground is hit
                        result = true;
                        if (states.isWantingToVault)
                        {
                            states.anim.SetBool(states.hashes.isInteracting, true);
                            states.anim.CrossFade(states.hashes.VaultWalk, 0.15f);
                            states.vaultData.animLength = vaultWalkClip.length;
                            states.vaultData.isInit = false;
                            states.isVaulting = true;
                            states.vaultData.startPosition = states.mTransform.position;
                            Vector3 endPosition = firstHit;
                            endPosition += normalDir * vaultOffsetPosition;
                            states.vaultData.endingPosition = endPosition;
                        }
                    }
                }

            }

            states.canVault = result;

            


            return (result && states.isWantingToVault);
        }

    }
}

