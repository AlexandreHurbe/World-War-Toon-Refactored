using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Mono Actions/Handle Camera Scrolling")]
    public class CameraPivotSwitcher : Action
    {
        public SO.BoolVariable leftPivot;
        public SO.BoolVariable isAiming;

        public SO.TransformVariable targetTransform;

        public float defaultValue;
        public float affectedValue;

        public float speed = 5;
        private float actualValue;

        public override void Execute()
        {
            if (leftPivot.value)
            {
                float targetValue = -(2 *defaultValue);
                

                if (isAiming.value)
                {
                    targetValue = -(2 *affectedValue);
                }

                actualValue = Mathf.Lerp(actualValue, targetValue, speed * Time.deltaTime);
                Vector3 targetPosition = targetTransform.value.localPosition;
                targetPosition.x = actualValue;

                targetTransform.value.localPosition = targetPosition;
            }
            else
            {
                float targetValue = defaultValue;
                
                if (isAiming.value)
                {
                    targetValue = affectedValue;
                }

                actualValue = Mathf.Lerp(actualValue, targetValue, speed * Time.deltaTime);
                Vector3 targetPosition = targetTransform.value.localPosition;
                targetPosition.x = actualValue;

                targetTransform.value.localPosition = targetPosition;
            }
            
        }
    }

}