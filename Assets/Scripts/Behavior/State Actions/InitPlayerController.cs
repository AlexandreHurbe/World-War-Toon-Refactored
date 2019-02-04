﻿using UnityEngine;
using System.Collections;


namespace SA {
    [CreateAssetMenu (menuName = "Actions/State Actions/Init Player Controller")]
    public class InitPlayerController : StateActions
    {
        public StateActions initActionsBatch;
        public override void Execute(StateManager states)
        {
            states.mTransform = states.transform;
            states.anim = states.mTransform.GetComponentInChildren<Animator>();
            states.rigidbody = states.mTransform.GetComponent<Rigidbody>();
            states.rigidbody.drag = 4;
            states.rigidbody.angularDrag = 999;
            states.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            states.ignoreLayers = ~(1 << 9 | 1 << 3);

            Debug.Log(states.ignoreLayers);

            if (initActionsBatch != null)
            {
                initActionsBatch.Execute(states);
            }
        }
    }
}


