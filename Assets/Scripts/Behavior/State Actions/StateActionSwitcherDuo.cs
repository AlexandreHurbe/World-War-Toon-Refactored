using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA {

    [CreateAssetMenu(menuName = "Actions/State Actions/Switcher2")]
    public class StateActionSwitcherDuo : StateActions {
        public SO.BoolVariable targetBool1;
        public SO.BoolVariable targetBool2;
        public StateActions onFalseAction;
        public StateActions onTrueAction;

        public override void Execute(StateManager states) {
            if (targetBool1.value || targetBool2.value) {
                if (onTrueAction != null) {
                    onTrueAction.Execute(states);
                }
            }
            else if (targetBool1.value && targetBool2.value) {
                if (onTrueAction != null) {
                    onTrueAction.Execute(states);
                }
            }
            else {
                if (onFalseAction != null) {
                    onFalseAction.Execute(states);
                }
            }
        }
    }
}

