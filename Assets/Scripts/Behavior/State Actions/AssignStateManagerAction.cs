using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/AssignStateManager")]
    public class AssignStateManagerAction : StateActions
    {
        public StatesVariable variable;

        public override void Execute(StateManager states)
        {
            variable.value = states;
        }


    }
}
