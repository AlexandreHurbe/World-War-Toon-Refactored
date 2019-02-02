using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class ExplodingBarrel : MonoBehaviour, IHittable
    {
        public string targetParticle = "BloodSplat_FX";

        public void OnHit(StateManager shooter, Weapon w, Vector3 dir, Vector3 pos)
        {
            GameObject hitParticle = GameManagers.GetObjectPool().RequestObject(targetParticle);
            Quaternion rot = Quaternion.LookRotation(-dir);
            hitParticle.transform.position = pos;
            hitParticle.transform.rotation = rot;
        }
    }

}

