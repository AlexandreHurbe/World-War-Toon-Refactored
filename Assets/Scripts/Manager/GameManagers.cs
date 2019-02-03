using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public static class GameManagers 
    {
        #region Object Pooler
        private static ObjectPooler objectPooler;
        public static ObjectPooler GetObjectPool()
        {
            if (objectPooler == null)
            {
                objectPooler = Resources.Load("ObjectPooler") as ObjectPooler;
                objectPooler.Init();
            }

            return objectPooler;
        }
        #endregion

        private static PlayerProfile profile;
        public static PlayerProfile GetProfile()
        {
            return Resources.Load("PlayerProfile") as PlayerProfile;
        }

        #region Resources Manager
        private static ResourcesManager resourcesManager;
        public static ResourcesManager GetResourcesManager()
        {
            if (resourcesManager == null)
            {
                resourcesManager = Resources.Load("ResourcesManager") as ResourcesManager;
                resourcesManager.Init();
            }

            return resourcesManager;
        }
        #endregion

        #region Ammo pool
        static AmmoPool ammoPool;
        public static AmmoPool GetAmmoPool()
        {
            if (ammoPool == null)
            {
                ammoPool = Resources.Load("AmmoPool") as AmmoPool;
                ammoPool.Init();
            }

            return ammoPool;
        }
        #endregion
    }
}

