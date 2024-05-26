using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

namespace PCTC.Camera
{
    public class CameraPositioner : MonoBehaviour
    {
        [SerializeField]
        private float angle;

        [SerializeField]
        private float animLength;

        private int currentside = 1;

        private Vector3 gameFieldCenter;
        private int fieldSize;

        public void PlaceCamera(Vector3 gameFieldCenter, int fieldSize)
        {
            this.gameFieldCenter = gameFieldCenter;
            this.fieldSize = fieldSize;

            float viewZoneSize = fieldSize * 1.5f;
            Debug.Log($"center {gameFieldCenter}");
            // Угол в радианах
            float angleInRadians = angle * Mathf.Deg2Rad;

            // Рассчитываем горизонтальное расстояние от камеры до объекта
            float horizontalDistance = (viewZoneSize / 2) / Mathf.Tan(angleInRadians);

            // Рассчитываем высоту камеры
            float height = Mathf.Tan(angleInRadians) * horizontalDistance;
            Vector3 cameraPosition =
                gameFieldCenter + new Vector3(horizontalDistance * currentside, height, 0);

            Transform cameraPlaceHolder = new GameObject("cameraPlaceHolder").transform;
            cameraPlaceHolder.position = cameraPosition;
            cameraPlaceHolder.LookAt(gameFieldCenter);

            cameraPosition = cameraPlaceHolder.position;

            Quaternion cameraEndRotation = cameraPlaceHolder.transform.rotation;
            Destroy(cameraPlaceHolder.gameObject);

            transform.DOMove(cameraPosition, animLength).SetEase(Ease.InOutQuad);

            transform.DORotateQuaternion(cameraEndRotation, animLength).SetEase(Ease.InOutQuad);
        }

        public void Update()
        {
            currentside *= -1;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlaceCamera(gameFieldCenter, fieldSize);
            }
        }
    }
}
