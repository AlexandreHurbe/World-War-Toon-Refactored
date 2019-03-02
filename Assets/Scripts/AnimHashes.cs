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
        public int coverStandingLocomotionLeft = Animator.StringToHash("Cover Standing Locomotion Left");
        public int coverStandingLocomotionRight = Animator.StringToHash("Cover Standing Locomotion Right");

        public int isEnteringCoverCrouching = Animator.StringToHash("isEnteringCoverCrouching");
        public int coverCrouchLocomotionLeft = Animator.StringToHash("Cover Crouch Locomotion Left");
        public int coverCrouchLocomotionRight = Animator.StringToHash("Cover Crouch Locomotion Right");

        public int leftPivot = Animator.StringToHash("leftPivot");
        public int isCrouching = Animator.StringToHash("crouch");
        public int aiming = Animator.StringToHash("aiming");
    }

}


