using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using PCTC.Camera;
using PCTC.Game;
using PCTC.Handlers;
using PCTC.Structs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCTC.Builders
{
    public class CarpetBuilder : MonoBehaviour
    {
        private int fieldSize;

        [SerializeField]
        private float floorY;

        [SerializeField]
        private GameObject floorPrefab;

        [SerializeField]
        private Material whiteCarpet,
            blackCarpet;

        [SerializeField]
        private CameraPositioner cameraPositioner;

        private ClickInputHandler[,] handlersMap;

        public void BuildGameField(CatData[,] catData, int playerId)
        {
            BuildCarpet(catData);
            float center = (float)(fieldSize - 1) / 2;
            Vector3 fieldCenter = new Vector3(center, floorY, center);
            cameraPositioner.PlaceCamera(fieldCenter, playerId);
        }

        private void BuildCarpet(CatData[,] gameField)
        {
            handlersMap = new ClickInputHandler[8, 8];
            fieldSize = gameField.GetLength(0);
            Vector3 cellPosition = new Vector3();
            cellPosition.y = floorY;
            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    cellPosition.x = x;
                    cellPosition.z = y;
                    MeshRenderer carpetCell = Instantiate(
                            floorPrefab,
                            cellPosition,
                            new Quaternion(),
                            transform
                        )
                        .GetComponentInChildren<MeshRenderer>();

                    Material carpetMaterial;

                    string cellColor = "(COLOR UNDEFINED)";
                    if (gameField[x, y].id > 0)
                    {
                        carpetMaterial = blackCarpet;
                        ClickInputHandler cellHandler =
                            carpetCell.AddComponent<ClickInputHandler>();
                        cellHandler.position = new Vector2Int(x, y);

                        cellColor = "(BLACK)";
                        handlersMap[x, y] = cellHandler;
                    }
                    else
                    {
                        carpetMaterial = whiteCarpet;
                        cellColor = "(WHITE)";
                    }
                    carpetCell.material = carpetMaterial;
                    string cellName = $"Carpet [{x},{y}]({cellColor})";
                    carpetCell.name = cellName;
                }
            }
        }

        public ClickInputHandler[,] GetHandlersMap()
        {
            return handlersMap;
        }
    }
}
