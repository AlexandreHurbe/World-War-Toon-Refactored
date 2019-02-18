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
        public float enterCoverSpeed = 2f;
        public float enterCoverRotation = 0.4f;

        public float coverPosOffset = 0.08f;

        public bool canStand;

        public float animLength;
        public float enterCoverPosT;
        public float enterCoverRotT;
        public bool isEnteringCoverInit;
        public bool isInit;


    }
}

