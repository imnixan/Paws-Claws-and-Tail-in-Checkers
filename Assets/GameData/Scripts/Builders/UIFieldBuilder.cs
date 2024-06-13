using System;
using System.Data.SqlTypes;
using System.Drawing.Text;
using PJTC.Game;
using PJTC.Structs;
using UnityEngine;
using UnityEngine.UI;

namespace PJTC.Builders
{
    public class UIFieldBuilder : MonoBehaviour
    {
        [SerializeField]
        private Sprite orangeCat,
            blackCat;

        [SerializeField]
        private RectTransform window;

        public void BuildUIField(CatData[,] fieldData, int playerID)
        {
            Vector2 windowSize = window.sizeDelta;
            float cellSize = windowSize.x / GameField.fieldSize;
            float startX = windowSize.x * -0.5f;
            float startY = windowSize.y * 0.5f;
            float posX = startX;
            float posY = startY;

            fieldData =
                playerID == 0 ? Rotate90Clockwise(fieldData) : Rotate90CounterClockwise(fieldData);
            //fieldData = Rotate180(fieldData);

            for (int x = GameField.fieldSize - 1; x >= 0; x--)
            {
                for (int y = 0; y < GameField.fieldSize; y++)
                {
                    CatData cat = fieldData[x, y];
                    GameObject newCell = new GameObject(cat.id == 0 ? "WhiteCell" : "BlackCell");
                    newCell.transform.SetParent(window);
                    newCell.transform.localScale = new Vector2(1, 1);
                    RectTransform cellRect = newCell.AddComponent<RectTransform>();
                    Image cellImage = newCell.AddComponent<Image>();
                    cellImage.color = cat.id == 0 ? Color.white : Color.blue;
                    cellRect.sizeDelta = new Vector2(cellSize, cellSize);
                    cellRect.anchorMax = Vector2.up;
                    cellRect.anchorMin = cellRect.anchorMax;
                    cellRect.pivot = cellRect.anchorMax;
                    cellRect.localPosition = new Vector2(posX, posY);
                    posY -= cellSize;
                    if (cat.id > 1)
                    {
                        GameObject uiCat = new GameObject(
                            $"{cat.team.ToString()} {cat.type.ToString()}"
                        );
                        RectTransform catRect = uiCat.AddComponent<RectTransform>();
                        Image catImage = uiCat.AddComponent<Image>();
                        catImage.sprite =
                            cat.team == Enums.CatsType.Team.Orange ? orangeCat : blackCat;
                        catRect.SetParent(cellRect);
                        catRect.localScale = new Vector2(1, 1);
                        catImage.SetNativeSize();
                        float maxSideSize = cellSize - cellSize / 10;
                        float scale =
                            maxSideSize / Mathf.Max(catRect.sizeDelta.x, catRect.sizeDelta.y);
                        catRect.sizeDelta *= scale;
                        catRect.localPosition = new Vector2(cellSize * 0.5f, cellSize * -0.5f);
                    }
                }
                posY = startY;
                posX += cellSize;
            }
        }

        public T[,] Rotate90Clockwise<T>(T[,] array)
        {
            int n = array.GetLength(0);
            T[,] rotatedArray = new T[n, n];

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    rotatedArray[y, n - 1 - x] = array[x, y];
                }
            }

            return rotatedArray;
        }

        public T[,] Rotate90CounterClockwise<T>(T[,] array)
        {
            int n = array.GetLength(0);
            T[,] rotatedArray = new T[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    rotatedArray[n - 1 - j, i] = array[i, j];
                }
            }

            return rotatedArray;
        }

        public T[,] Rotate180<T>(T[,] array)
        {
            int n = array.GetLength(0);
            T[,] rotatedArray = new T[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    rotatedArray[n - 1 - i, n - 1 - j] = array[i, j];
                }
            }

            return rotatedArray;
        }
    }
}
