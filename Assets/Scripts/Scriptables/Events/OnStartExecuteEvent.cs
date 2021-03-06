﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace SA {

    public class OnStartExecuteEvent : MonoBehaviour
    {
        public UnityEvent onEnable;

        private void Start()
        {
            ExecuteUnityEvent();
        }

        public void ExecuteUnityEvent()
        {
            onEnable.Invoke();
        }
    }
}

