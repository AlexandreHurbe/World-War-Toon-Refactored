using UnityEngine;
using System.Collections;


namespace SA {

    [System.Serializable]
    public class CoverData
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public Quaternion startRotation;
        public Quaternion endRotation;
        public float enterCoverSpeed = 1f;
        public float enterCoverRotation = 1f;

        public float coverPosOffset = 0.3f;

        public bool canStand;
        public bool atCorner;

        public float animLength;
        public float enterCoverPosT;
        public float enterCoverRotT;
        public bool isEnteringCoverInit;
        public bool isInit;

        public bool canShootRight;
        public bool canShootLeft;

        public ShootDirection shootDirection;

        public enum ShootDirection
        {
            none,
            left,
            right
        };
    }


   

}

