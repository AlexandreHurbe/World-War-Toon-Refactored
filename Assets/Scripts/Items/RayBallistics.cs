using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    [CreateAssetMenu (menuName = "Ballistics/Ray")]
    public class RayBallistics : Ballistics
    {
        public override void Execute(StateManager states, Weapon w)
        {
            Vector3 origin = w.runtime.modelInstance.transform.position;
            Vector3 dir = states.movementValues.aimPosition - origin;

            Ray ray = new Ray(origin, dir);
            RaycastHit hit;
            Debug.DrawLine(origin, dir);


            if (Physics.Raycast(ray, out hit, 100, states.ignoreLayers))
            {
                IHittable isHittable = hit.transform.GetComponentInParent<IHittable>();

                Debug.Log(hit.transform.name);

                if (isHittable == null)
                {
                    GameObject hitParticle = GameManagers.GetObjectPool().RequestObject("Bullet_Impact_FX");
                    Quaternion rot = Quaternion.LookRotation(-dir);
                    hitParticle.transform.position = hit.point;
                    hitParticle.transform.rotation = rot;
                }
                else
                {
                    isHittable.OnHit(states, w, dir, hit.point);
                }

                
            }
        }
    }

}

