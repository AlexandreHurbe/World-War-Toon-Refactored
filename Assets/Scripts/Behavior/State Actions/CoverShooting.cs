using UnityEngine;
using System.Collections;


namespace SA
{
    [CreateAssetMenu (menuName = "Actions/State Actions/Cover Shooting")]
    public class CoverShooting : StateActions
    {
        public float threshold;
        public float yOffset;

        public override void Execute(StateManager states)
        {
            if (states.coverState == StateManager.CoverState.isInCover)
            {
                RaycastHit hit;
                //Aim right side
                Vector3 checkRightPoint = states.mTransform.position;
                checkRightPoint.y += yOffset;
                checkRightPoint += (states.mTransform.right * threshold);
                Debug.DrawRay(checkRightPoint, -states.mTransform.forward, Color.blue);
                if (Physics.Raycast(checkRightPoint, -states.mTransform.forward, out hit, 1f, states.ignoreLayers))
                {
                    //Cannot shoot as the view is blocked
                    states.coverData.canShootRight = false;
                    //Debug.Log("Cannot shoot on right side as it is blocked");
                }
                else
                {
                    //Debug.Log("Can shoot on right side as it is blocked");
                    states.coverData.canShootRight = true;
                }

                //Aim left side
                Vector3 checkLeftPoint = states.mTransform.position;
                checkLeftPoint.y += yOffset;
                checkLeftPoint += (-states.mTransform.right * threshold);
                Debug.DrawRay(checkLeftPoint, -states.mTransform.forward, Color.red);
                if (Physics.Raycast(checkLeftPoint, -states.mTransform.forward, out hit, 1f, states.ignoreLayers))
                {
                    //Cannot shoot as the view is blocked
                    //Debug.Log("Cannot shoot on left side as it is blocked");
                    states.coverData.canShootLeft = false;
                }
                else
                {
                    //Debug.Log("Can shoot on left side as it is blocked");
                    states.coverData.canShootLeft = true;
                }

                states.coverData.shootDirection = CoverData.ShootDirection.none;
                if (states.isShooting || states.isAiming)
                {
                    AssignShootDirection(states);

                }
            }
            
        }

        private void AssignShootDirection(StateManager states)
        {
            if (states.leftPivot)
            {
                //Because this is flipped based on the character's orientation
                if (states.coverData.canShootRight)
                {
                    states.coverData.shootDirection = CoverData.ShootDirection.right;
                    //Debug.Log("Left pivot shoot direction: right");
                }
                else if (states.coverData.canShootLeft)
                {
                    states.coverData.shootDirection = CoverData.ShootDirection.left;
                    //Debug.Log("Left pivot shoot direction: left");
                }
                else
                {
                    states.coverData.shootDirection = CoverData.ShootDirection.none;
                    //Debug.Log("Left pivot shoot direction: none");
                }
            }
            else
            {
                //Because this is flipped based on the character's orientation
                if (states.coverData.canShootLeft)
                {
                    states.coverData.shootDirection = CoverData.ShootDirection.left;
                    //Debug.Log("Right pivot shoot direction: left");
                }
                else if (states.coverData.canShootRight)
                {
                    states.coverData.shootDirection = CoverData.ShootDirection.right;
                    //Debug.Log("Right pivot shoot direction: right");
                }
                else
                {
                    states.coverData.shootDirection = CoverData.ShootDirection.none;
                    //Debug.Log("Right pivot shoot direction: none");
                }
            }
        }
    }

}
