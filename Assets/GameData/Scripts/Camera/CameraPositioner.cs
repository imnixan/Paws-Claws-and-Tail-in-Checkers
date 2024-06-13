using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

namespace PJTC.CameraControl
{
    public class CameraPositioner : MonoBehaviour
    {
        [SerializeField]
        private float moveTime = 1.5f;

        [SerializeField]
        private Transform startPos;

        [SerializeField]
        private Transform[] playerCamPos;

        private void Start()
        {
            this.transform.position = startPos.position;
            this.transform.rotation = startPos.rotation;
        }

        public void PosCamera(int playerID)
        {
            this.transform.DOMove(playerCamPos[playerID].position, moveTime).Play();
            this.transform.DORotateQuaternion(playerCamPos[playerID].rotation, moveTime).Play();
        }
    }
}
