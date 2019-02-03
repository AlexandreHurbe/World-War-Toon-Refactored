using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    
    public class StateManager : MonoBehaviour, IHittable
    {
        

        [System.Serializable]
        public class MovementValues
        {
            public float horizontal;
            public float vertical;
            public float moveAmount;
            public Vector3 moveDirection;
            public Vector3 lookDirection;
            public Vector3 aimPosition;
        }

        public int photonId;

        public bool isLocal;
        public bool isAiming;
        public bool isInteracting;
        public bool isShooting;
        public bool isCrouching;
        public bool isReloading;
        public bool isVaulting;
        public bool isGrounded;

        public bool shootingFlag;
        public bool reloadingFlag;
        public bool vaultingFlag;
        public bool healthChangedFlag = true;

        public PlayerStats stats;
        public MovementValues movementValues;
        public Inventory inventory;

        public State currentState;

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public float delta;
        [HideInInspector]
        public Transform mTransform;
        [HideInInspector]
        public new Rigidbody rigidbody;
        [HideInInspector]
        public LayerMask ignoreLayers;
        [HideInInspector]
        public AnimatorHook animHook;
        [HideInInspector]
        public VaultData vaultData;
        public AnimHashes hashes;

        public Ballistics ballisticsAction;

        public MultiplayerListener multiplayerListener;

        public bool isOfflineController;
        public StateActions offlineActions;


        private void Start()
        {
            mTransform = this.transform;
            rigidbody = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            hashes = new AnimHashes();
            vaultData = new VaultData();
            
            if (isOfflineController)
            {
                offlineActions.Execute(this);
            }

        }

        private void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;

            if (currentState != null)
            {
                currentState.FixedTick(this);
            }
        }

        private void Update()
        {
            delta = Time.deltaTime;
            if(currentState != null)
            {
                currentState.Tick(this);
            }
        }

        public void SetCurrentState(State targetState)
        {
            if (currentState != null)
            {
                currentState.OnExit(this);
            }
            
            currentState = targetState;
            currentState.OnEnter(this);

        }

        public void SetCrouching()
        {
            isCrouching = !isCrouching;
        }

        public void SetReloading()
        {
            isReloading = true;
        }

        public void PlayAnimation(string targetAnim)
        {
            anim.CrossFade(targetAnim, 0.2f);
        }

        public void OnHit(StateManager shooter, Weapon w, Vector3 dir, Vector3 pos)
        {
            stats.health -= w.ammoType.damageValue;
            if (stats.health <= 0)
            {
                stats.health = 0;
                //Raise event for death
            }

            healthChangedFlag = true;
        }
    }
}
