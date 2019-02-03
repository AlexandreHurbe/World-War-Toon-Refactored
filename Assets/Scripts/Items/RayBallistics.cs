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
            Debug.DrawLine(origin, dir);
            RaycastHit[] hits;

            hits = Physics.RaycastAll(origin, dir, 100, states.ignoreLayers);
            if (hits == null)
            {
                return;
            }
            if (hits.Length == 0)
            {
                return;
            }

            RaycastHit closestHit;
            closestHit = GetClosestHit(origin, hits, states.photonId);

            IHittable isHittable = closestHit.transform.GetComponentInParent<IHittable>();

            if (isHittable == null)
            {
                GameObject hitParticle = GameManagers.GetObjectPool().RequestObject("Bullet_Impact_FX");
                Quaternion rot = Quaternion.LookRotation(-dir);
                hitParticle.transform.position = closestHit.point;
                hitParticle.transform.rotation = rot;
            }
            else
            {
                isHittable.OnHit(states, w, dir, closestHit.point);
            }

 

            MultiplayerManager mm = MultiplayerManager.singleton;
            if (mm != null)
            {
                mm.BroadcastShootWeapon(states, dir, origin);
            }
        }


        public static RaycastHit GetClosestHit(Vector3 o, RaycastHit[] l, int shooter)
        {
            int closest = 0;

            float minDist = float.MaxValue;

            for (int i = 0; i < l.Length; i++)
            {
                float tempDist = Vector3.Distance(o, l[i].point);
                if (tempDist < minDist)
                {
                    StateManager states = l[i].transform.GetComponentInParent<StateManager>();

                    if (states != null)
                    {
                        if (states.photonId == shooter)
                        {
                            continue;
                        }
                    }

                    minDist = tempDist;
                    closest = i;
                }
            }

            return l[closest];

        }

        public void ClientShoot(StateManager states, Vector3 dir, Vector3 origin)
        {
            Ray ray = new Ray(origin, dir);
            RaycastHit hit;
            Debug.DrawLine(origin, dir);


            RaycastHit[] hits;

            hits = Physics.RaycastAll(origin, dir, 100, states.ignoreLayers);
            if (hits == null)
            {
                return;
            }
            if (hits.Length == 0)
            {
                return;
            }

            RaycastHit closestHit;
            closestHit = GetClosestHit(origin, hits, states.photonId);

            IHittable isHittable = closestHit.transform.GetComponentInParent<IHittable>();

            if (isHittable == null)
            {
                GameObject hitParticle = GameManagers.GetObjectPool().RequestObject("Bullet_Impact_FX");
                Quaternion rot = Quaternion.LookRotation(-dir);
                hitParticle.transform.position = closestHit.point;
                hitParticle.transform.rotation = rot;
            }
            else
            {
                isHittable.OnHit(states, states.inventory.currentWeapon, dir, closestHit.point);
            }


        }

    }

}

