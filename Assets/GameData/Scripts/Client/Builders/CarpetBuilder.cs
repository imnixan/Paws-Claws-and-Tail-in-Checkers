using PJTC.Handlers;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Builders
{
    public class CarpetBuilder : MonoBehaviour
    {
        [Tooltip("floor Y coordinate")]
        [SerializeField]
        private float floorY;

        [Header("Visual castomisation")]
        [SerializeField]
        private GameObject floorPrefab;

        [SerializeField]
        private Material whiteCarpet;

        [SerializeField]
        private Material blackCarpet;

        private ClickInputHandler[,] handlersMap;
        private GameObject carpet;
        private int fieldSize;

        public ClickInputHandler[,] GetHandlersMap()
        {
            return handlersMap;
        }

        public void BuildGameField(CatData[,] catData)
        {
            if (carpet != null)
            {
                Destroy(carpet);
            }

            carpet = new GameObject("Carpet");
            carpet.transform.SetParent(transform);

            BuildCarpet(catData);
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
                            carpet.transform
                        )
                        .GetComponentInChildren<MeshRenderer>();

                    Material carpetMaterial;
                    string cellColor = "(COLOR UNDEFINED)";

                    if (gameField[x, y].id > 0)
                    {
                        carpetMaterial = blackCarpet;
                        ClickInputHandler cellHandler =
                            carpetCell.gameObject.AddComponent<ClickInputHandler>();
                        cellHandler.position = new Vector2Int(x, y);
                        cellColor = "(BLACK)";

                        handlersMap[x, y] = cellHandler;
                    }
                    else
                    {
                        carpetMaterial = whiteCarpet;
                        cellColor = "(WHITE)";
                    }

                    carpetCell.material = new Material(carpetMaterial);
                    string cellName = $"Carpet [{x},{y}]({cellColor})";
                    carpetCell.name = cellName;
                }
            }
        }
    }
}
