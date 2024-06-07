using PCTC.CatScripts;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Game
{
    public class GameField
    {
        //0 = white cell
        //1 = blackCell
        //1+n - catsId;

        public CatData[,] matrix { get; private set; }
        private int fieldSize = 8;

        public GameField()
        {
            matrix = new CatData[fieldSize, fieldSize];
            BuildField();
            FillField();
        }

        public GameField(CatData[,] gameField)
        {
            this.matrix = gameField;
        }

        private void BuildField()
        {
            bool whiteCell = false;
            for (int x = 0; x < fieldSize; x++)
            {
                whiteCell = !whiteCell;
                for (int y = 0; y < fieldSize; y++)
                {
                    matrix[x, y] = new CatData(whiteCell ? 0 : 1, new Vector2Int(x, y));

                    whiteCell = !whiteCell;
                }
            }
        }

        private void FillField()
        {
            int catId = 2;
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    if (matrix[x, y].id == 1)
                    {
                        matrix[x, y].id = catId++;
                        matrix[x, y].team = Enums.CatsType.Team.Orange;
                        matrix[x, y].type = Enums.CatsType.Type.Normal;
                        matrix[(fieldSize - 1) - x, (fieldSize - 1) - y].id = catId++;
                        matrix[(fieldSize - 1) - x, (fieldSize - 1) - y].team = Enums
                            .CatsType
                            .Team
                            .Black;
                        matrix[(fieldSize - 1) - x, (fieldSize - 1) - y].type = Enums
                            .CatsType
                            .Type
                            .Normal;
                    }
                }
            }
        }

        public CatData GetElement(Vector2Int coords)
        {
            if (coords.x < 0 || coords.y < 0 || coords.x >= fieldSize || coords.y >= fieldSize)
            {
                return new CatData(-1, coords);
            }
            return matrix[coords.x, coords.y];
        }

        public void RemoveElementAt(Vector2Int elementCoords)
        {
            matrix[elementCoords.x, elementCoords.y] = new CatData(1, elementCoords);
        }

        public void SetElement(CatData element)
        {
            matrix[element.position.x, element.position.y] = element;
        }

        public void UpgradeElementAt(Vector2Int elementCoords)
        {
            matrix[elementCoords.x, elementCoords.y].type = Enums.CatsType.Type.Chonky;
        }
    }
}
