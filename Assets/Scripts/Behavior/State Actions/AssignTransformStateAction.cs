using UnityEngine;
using System.Collections;
using SO;

namespace SA
{
    [CreateAssetMenu (menuName ="Actions/State Actions/AssignTransform")]
    public class AssignTransformStateAction : StateActions
    {
        public TransformVariable transformVariable;

        public override void Execute(StateManager states)
        {
            transformVariable.value = states.mTransform;
        }

        
    }
}

