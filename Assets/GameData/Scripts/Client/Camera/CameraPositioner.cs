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
            //this.transform.position = startPos.position;
            //this.transform.rotation = startPos.rotation;
        }

        public void PosCamera(int playerID)
        {
            Sequence cameraMove = DOTween.Sequence();
            cameraMove
                .Append(this.transform.DOMove(startPos.position, moveTime / 2))
                .Join(this.transform.DORotateQuaternion(startPos.rotation, moveTime / 2))
                .Append(this.transform.DOMove(playerCamPos[playerID].position, moveTime))
                .Join(this.transform.DORotateQuaternion(playerCamPos[playerID].rotation, moveTime))
                .Restart();
        }
    }
}
