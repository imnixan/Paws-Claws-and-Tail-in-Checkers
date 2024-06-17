using DG.Tweening;
using UnityEngine;

namespace PJTC.CameraControl
{
    public class CameraPositioner : MonoBehaviour
    {
        [SerializeField]
        private float moveTime = 1.5f;

        [Tooltip(
            "Precreated position prefab. Used in the transition animation from the menu to the player's view, as an intermediate point"
        )]
        [SerializeField]
        private Transform startPos;

        [Tooltip("Precreated position prefabs per player")]
        [SerializeField]
        private Transform[] playerCamPos;

        public void PosCamera(int playerID)
        {
            float halfTime = moveTime / 2;

            Sequence cameraMove = DOTween.Sequence();
            cameraMove
                .Append(this.transform.DOMove(startPos.position, halfTime))
                .Join(this.transform.DORotateQuaternion(startPos.rotation, halfTime))
                .Append(this.transform.DOMove(playerCamPos[playerID].position, halfTime))
                .Join(this.transform.DORotateQuaternion(playerCamPos[playerID].rotation, halfTime))
                .Restart();
        }
    }
}
