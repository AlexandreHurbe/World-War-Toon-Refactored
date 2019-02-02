﻿using System.Collections;
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

        private StateManager states;
        private Transform mTransform;


        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            states = GetComponent<StateManager>();
            mTransform = this.transform;

            MultiplayerManager m = MultiplayerManager.singleton;
            this.transform.parent = m.GetMRef().referencesParent;

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
            //sending data
            if (stream.IsWriting)
            {
                //Sending Position and rotation
                stream.SendNext(mTransform.position);
                stream.SendNext(mTransform.rotation);

                //Sending booleans
                stream.SendNext(states.isAiming);

                //Sending horizontal and vertical movement values
                stream.SendNext(states.movementValues.horizontal);
                stream.SendNext(states.movementValues.vertical);
                
            }
            //receiving data
            else
            {
                //Receiving Position and rotation and related functions
                Vector3 position = (Vector3)stream.ReceiveNext();
                Quaternion rotation = (Quaternion)stream.ReceiveNext();
                ReceivePositionRotation(position, rotation);

                //Receving booleans
                states.isAiming = (bool)stream.ReceiveNext();
                
                

                //Receiving horizontal and vertical movement values
                states.movementValues.horizontal = (float)stream.ReceiveNext();
                states.movementValues.vertical = (float)stream.ReceiveNext();
                states.movementValues.moveAmount = Mathf.Clamp01(Mathf.Abs(states.movementValues.horizontal) + Mathf.Abs(states.movementValues.vertical));
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
