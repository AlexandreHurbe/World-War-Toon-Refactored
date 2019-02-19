using UnityEngine;
using System.Collections;


namespace SA {

    [CreateAssetMenu(menuName = "Actions/State Actions/Cover Movement")]
    public class CoverMovement : StateActions
    {
        public float frontRayOffset = 0.5f;
        public float rayForwardDis;

        private Vector3 targetVelocity;

        public float sprintSpeed = 4f;
        public float movementSpeed = 2;
        public float adaptSpeed = 10;
        public float crouchSpeed = 2f;

        public float threshold = 2f;

        public override void Execute(StateManager states)
        {
            
            if (states.coverState == StateManager.CoverState.isInCover && !states.isAiming)
            {
                Debug.DrawRay(states.mTransform.position, states.mTransform.right * 10f, Color.red);
                Debug.DrawRay(states.mTransform.position, -states.mTransform.right * 10f, Color.green);
                Debug.DrawRay(states.mTransform.position, states.mTransform.forward * 10f, Color.blue);

                if (states.movementValues.moveAmount > 0.1f)
                {
                    states.rigidbody.drag = 0;
                }
                else
                {
                    states.rigidbody.drag = 4;
                }

                float targetSpeed = movementSpeed;

                if (states.isCrouching)
                {
                    targetSpeed = crouchSpeed;
                }

                Vector3 velocity = (states.mTransform.right * -states.movementValues.horizontal) * (states.movementValues.moveAmount * targetSpeed);
                //Debug.Log(velocity);
                states.rigidbody.velocity = velocity;

                if (states.movementValues.horizontal < 0)
                {
                    states.leftPivot.value = true;
                    Vector3 predictedPoint = (states.mTransform.position + (states.mTransform.right * threshold));
                    if (!withinLimit(predictedPoint, -states.mTransform.forward, states.ignoreLayers)) {
                        states.rigidbody.velocity = Vector3.zero;
                        states.coverData.atCorner = true;
                    }
                    else
                    {
                        states.coverData.atCorner = false;
                    }

                }
                else if (states.movementValues.horizontal > 0)
                {
                    states.leftPivot.value = false;
                    Vector3 predictedPoint = (states.mTransform.position + (-states.mTransform.right * threshold));
                    if (!withinLimit(predictedPoint, -states.mTransform.forward, states.ignoreLayers))
                    {
                        states.rigidbody.velocity = Vector3.zero;
                        states.coverData.atCorner = true;
                    }
                    else
                    {
                        states.coverData.atCorner = false;
                    }
                }
                else
                {
                    return;
                }

            }
            
        }

        private bool withinLimit(Vector3 predictedPoint, Vector3 direction, LayerMask ignoreLayers)
        {
            RaycastHit hit;
            Debug.DrawRay(predictedPoint, direction, Color.blue);
            if (Physics.Raycast(predictedPoint, direction, out hit, 0.3f, ignoreLayers))
            {
                //Debug.Log("In contact with wall");
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }

}
