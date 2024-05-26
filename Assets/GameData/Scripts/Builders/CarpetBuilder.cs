using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using PCTC.Camera;
using PCTC.Game;
using PCTC.Handlers;
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

        public List<ClickInputHandler> BuildGameField(GameField gameField)
        {
            List<ClickInputHandler> blackCellsHandlers = BuildCarpet(gameField.GetField());
            float center = (float)(fieldSize - 1) / 2;
            Vector3 fieldCenter = new Vector3(center, floorY, center);
            cameraPositioner.PlaceCamera(fieldCenter, fieldSize);
            return blackCellsHandlers;
        }

        private List<ClickInputHandler> BuildCarpet(int[,] gameField)
        {
            fieldSize = gameField.GetLength(0);
            List<ClickInputHandler> blackCellsHandlers = new List<ClickInputHandler>();
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
                    if (gameField[x, y] > 0)
                    {
                        carpetMaterial = blackCarpet;
                        blackCellsHandlers.Add(carpetCell.AddComponent<ClickInputHandler>());
                        cellColor = "(BLACK)";
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
            return blackCellsHandlers;
        }
    }
}
