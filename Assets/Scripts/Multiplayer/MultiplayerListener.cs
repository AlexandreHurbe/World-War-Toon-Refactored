using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SA
{
    public class MultiplayerListener : MonoBehaviourPun, IPunInstantiateMagicCallback, IPunObservable
    {
        public State local;
        public State client;
        public StateActions initLocalPlayer;
        public StateActions initClientPlayer;

        public State vaultClient;
        public State coverClient;

        private StateManager states;
        private Transform mTransform;


        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            states = GetComponent<StateManager>();
            states.InitReferences();
            mTransform = this.transform;
            object[] data = photonView.InstantiationData;

            states.photonId = (int)data[0];
            string modelId = (string)data[2];
            states.LoadCharacterModel(modelId);

            MultiplayerManager m = MultiplayerManager.singleton;
            this.transform.parent = m.GetMRef().referencesParent;

            PlayerHolder playerHolder = m.GetMRef().GetPlayer(states.photonId);
            playerHolder.states = states;

            string weaponId = (string)data[1];
            Debug.Log("Weapon ID: " + weaponId);
            states.inventory.weaponID = weaponId;

            if (photonView.IsMine)
            {
                states.isLocal = true;
                states.SetCurrentState(local);
                initLocalPlayer.Execute(states);
            }
            else
            {
                states.isLocal = false;
                states.SetCurrentState(client);
                initClientPlayer.Execute(states);
                states.multiplayerListener = this;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (states.isDead)
            {
                return;
            }

            //sending data
            if (stream.IsWriting)
            {
                

                //Sending Position and rotation
                stream.SendNext(mTransform.position);
                stream.SendNext(mTransform.rotation);

                //Sending booleans
                stream.SendNext(states.isVaulting);
                if (!states.isVaulting)
                {
                    stream.SendNext(states.coverState);
                    stream.SendNext(states.isCrouching);
                    stream.SendNext(states.isSprinting);
                    stream.SendNext(states.isAiming);
                    stream.SendNext(states.shootingFlag);
                    states.shootingFlag = false;
                    stream.SendNext(states.reloadingFlag);
                    states.reloadingFlag = false;

                    //Sending horizontal and vertical movement values
                    stream.SendNext(states.movementValues.horizontal);
                    stream.SendNext(states.movementValues.vertical);
                }

                //Sending aim position
                stream.SendNext(states.movementValues.aimPosition);

            }

            //receiving data
            else
            {
                //Receiving Position and rotation and related functions
                Vector3 position = (Vector3)stream.ReceiveNext();
                Quaternion rotation = (Quaternion)stream.ReceiveNext();
                ReceivePositionRotation(position, rotation);

                //Receving booleans
                states.isVaulting = (bool)stream.ReceiveNext();
                
                if (states.isVaulting)
                {
                    //Setting other booleans as false while vaulting since they cannot happen
                    states.isSprinting = false;
                    states.isCrouching = false;
                    states.isAiming = false;
                    states.isShooting = false;
                    states.isReloading = false;

                    //Setting horizontal and vertical movement values
                    states.movementValues.horizontal = 0;
                    states.movementValues.vertical = 0;
                    //Setting move amounts while vaulting
                    states.movementValues.moveAmount = 0;

                    if (!states.vaultingFlag)
                    {
                        states.vaultingFlag = true;
                        states.anim.CrossFade(states.hashes.VaultWalk, 0.15f);
                        states.currentState = vaultClient;
                    }
                }

                else {
                    states.coverState = (StateManager.CoverState)stream.ReceiveNext();
                    //Receving booleans if not vaulting
                    states.isCrouching = (bool)stream.ReceiveNext();
                    states.isSprinting = (bool)stream.ReceiveNext();
                    states.isAiming = (bool)stream.ReceiveNext();
                    states.isShooting = (bool)stream.ReceiveNext();
                    states.isReloading = (bool)stream.ReceiveNext();

                    //Receiving horizontal and vertical movement values
                    states.movementValues.horizontal = (float)stream.ReceiveNext();
                    states.movementValues.vertical = (float)stream.ReceiveNext();
                    //Calculating move amounts
                    states.movementValues.moveAmount = Mathf.Clamp01(Mathf.Abs(states.movementValues.horizontal) + Mathf.Abs(states.movementValues.vertical));

                    //If the player happens to be in cover
                    if (states.coverState == StateManager.CoverState.isEnteringCover) {
                        //Setting horizontal and vertical movement values
                        states.movementValues.horizontal = 0;
                        states.movementValues.vertical = 0;
                        //Setting move amounts while entering cover
                        states.movementValues.moveAmount = 0;
                    }
                    else if (states.coverState == StateManager.CoverState.isInCover) {
                        states.currentState = coverClient;
                    }
                    else if (states.coverState == StateManager.CoverState.isLeavingCover) {
                        //Setting horizontal and vertical movement values
                        states.movementValues.horizontal = 0;
                        states.movementValues.vertical = 0;
                        //Setting move amounts while leaving cover
                        states.movementValues.moveAmount = 0;
                    }

                    //This means player is in default state
                    else {
                        if (states.vaultingFlag) {
                            states.vaultingFlag = false;
                            states.currentState = client;
                        }

                        
                    }
                }


                //Receiving aim position
                states.movementValues.aimPosition = (Vector3)stream.ReceiveNext();

            }
        }


        #region Prediction
        Vector3 lastPosition;
        Quaternion lastRotation;
        Vector3 lastDirection;
        Vector3 targetAimPosition;

        public float snapDistance = 4;
        public float snapAngle = 40;
        public float predictionSpeed = 10;
        public float movementThreshold = 0.05f;
        public float angleThreshold = 0.05f;

        public void Prediction()
        {
            Vector3 curPos = mTransform.position;
            Quaternion curRot = mTransform.rotation;

            float distance = Vector3.Distance(lastPosition, curPos);
            float angle = Vector3.Angle(lastRotation.eulerAngles, curRot.eulerAngles);

            if (distance > snapDistance)
            {
                mTransform.position = lastPosition;
            }
            if (angle > snapAngle)
            {
                mTransform.rotation = lastRotation;
            }

            curPos += lastDirection;
            curRot *= lastRotation;

            Vector3 targetPosition = Vector3.Lerp(curPos, lastPosition, predictionSpeed * states.delta);
            mTransform.position = targetPosition;

            Quaternion targetRotation = Quaternion.Slerp(mTransform.rotation, lastRotation, predictionSpeed * states.delta);
            mTransform.rotation = targetRotation;

        }

        private void ReceivePositionRotation(Vector3 p, Quaternion r)
        {
            lastDirection = p - lastPosition;
            lastDirection /= 10;

            if (lastDirection.magnitude > movementThreshold)
            {
                lastDirection = Vector3.zero;
            }

            Vector3 lastEuler = lastRotation.eulerAngles;
            Vector3 newEuler = r.eulerAngles;

            if (Quaternion.Angle(lastRotation, r) < angleThreshold)
            {
                lastRotation = Quaternion.Euler((newEuler - lastEuler) / 10);
            }
            else
            {
                lastRotation = Quaternion.identity;
            }

            lastPosition = p;
            lastRotation = r;
        }


        #endregion

    }
}
