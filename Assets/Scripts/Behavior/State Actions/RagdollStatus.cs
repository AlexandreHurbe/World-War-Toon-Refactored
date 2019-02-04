using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu (menuName = "Actions/State Actions/Ragdoll Status")]
    public class RagdollStatus : StateActions
    {
        public bool enableRagdoll;

        public override void Execute(StateManager states)
        {
            if (enableRagdoll)
            {
                for (int i = 0; i < states.ragdollCols.Count; i++)
                {
                    states.ragdollCols[i].isTrigger = false;
                    states.ragdollRB[i].isKinematic = false;
                }

            }
            else
            {
                Rigidbody[] rigidbodies = states.mTransform.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody r in rigidbodies)
                {
                    if (r == states.rigidbody)
                    {
                        continue;
                    }

                    states.ragdollRB.Add(r);
                    r.isKinematic = true;

                    Collider col = r.GetComponent<Collider>();
                    states.ragdollCols.Add(col);
                    col.isTrigger = true;
                }
            }
            
        }
    }
}
