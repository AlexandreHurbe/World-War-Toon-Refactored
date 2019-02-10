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
        public bool isDead;
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
        public List<Rigidbody> ragdollRB = new List<Rigidbody>();
        [HideInInspector]
        public List<Collider> ragdollCols = new List<Collider>();
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
            stats.health = 100;
            healthChangedFlag = true;

            if (isOfflineController)
            {
                offlineActions.Execute(this);
            }

        }

        private void FixedUpdate()
        {
            if (isDead)
            {
                return;
            }


            delta = Time.fixedDeltaTime;

            if (currentState != null)
            {
                currentState.FixedTick(this);
            }
        }

        private void Update()
        {
            if (isDead)
            {
                return;
            }

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

        public void SpawnPlayer(Vector3 spawnPosition, Quaternion rotation)
        {
            if (isLocal)
            {
                //If this is the local player then set these values
                
            }

            //Set the client/local to the following variables
            stats.health = 100;
            healthChangedFlag = true;
            mTransform.position = spawnPosition;
            mTransform.rotation = rotation;
            anim.Play("Locomotion Normal");
            anim.Play("Empty Override");
            isDead = false;
        }

        public void KillPlayer()
        {
            isDead = true;
            anim.CrossFade("Death from Front Headshot", 0.4f);
        }

        public void OnHit(StateManager shooter, Weapon w, Vector3 dir, Vector3 pos)
        {
            if (shooter == this)
            {
                return;
            }

            Debug.Log("Player has been hit by: " + shooter.photonId);
            GameObject hitParticle = GameManagers.GetObjectPool().RequestObject("BloodSplat_FX");
            Quaternion rot = Quaternion.LookRotation(-dir);
            hitParticle.transform.position = pos;
            hitParticle.transform.rotation = rot;

            if (Photon.Pun.PhotonNetwork.IsMasterClient)
            {
                if (!isDead)
                {
                    
                    stats.health -= w.ammoType.damageValue;
                    MultiplayerManager mm = MultiplayerManager.singleton;
                    mm.BroadcastPlayerHealth(photonId, stats.health, shooter.photonId);

                    if (stats.health <= 0)
                    {
                        Debug.Log("Player: " + this.photonId + " has been killed by: " + shooter.photonId);
                        isDead = true;
                    }
                }
            }
            
        }
    }
}
