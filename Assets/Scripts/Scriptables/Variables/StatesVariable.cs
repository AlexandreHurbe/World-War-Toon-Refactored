using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu (menuName = "Variables/State Manager")]
    public class StatesVariable : ScriptableObject
    {
        public StateManager value;
    }
}

