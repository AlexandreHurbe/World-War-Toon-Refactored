using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {

    [CreateAssetMenu(menuName = "Actions/Mono Actions/Action Switcher")]
    public class ActionSwitcher : Action
    {
        public SO.BoolVariable targetBool;
        public Action onFalseAction;
        public Action onTrueAction;

        public override void Execute()
        {
            if (targetBool.value)
            {
                if (onTrueAction != null)
                {
                    onTrueAction.Execute();
                }
            }
            else
            {
                if (onFalseAction != null)
                {
                    onFalseAction.Execute();
                }
            }
        }
    }
}

