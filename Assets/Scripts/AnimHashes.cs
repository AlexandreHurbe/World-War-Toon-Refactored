using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {

    public class AnimHashes
    {
        public int vertical = Animator.StringToHash("vertical");
        public int horizontal = Animator.StringToHash("horizontal");
        public int VaultWalk = Animator.StringToHash("Vault Walk");
        public int isInteracting = Animator.StringToHash("isInteracting");
        public int locomotionNormal = Animator.StringToHash("Locomotion Normal");
        public int isEnteringCoverStanding = Animator.StringToHash("isEnteringCoverStanding");
        public int isEnteringCoverCrouching = Animator.StringToHash("isEnteringCoverCrouching");
        public int leftPivot = Animator.StringToHash("leftPivot");
        public int isCrouching = Animator.StringToHash("crouch");
        public int aiming = Animator.StringToHash("aiming");
    }

}


