using UnityEngine;

namespace PCTC.Game
{
    public class GameField
    {
        //0 = white cell
        //1 = blackCell
        //2 = player1
        //4 = player1super

        //3 = player2
        //6 = player2super

        private int[,] gameField;
        private int fieldSize;

        public GameField(int size)
        {
            this.fieldSize = size;
            gameField = new int[size, size];
            BuildField();
            FillField();
        }

        private void BuildField()
        {
            bool whiteCell = false;
            for (int x = 0; x < fieldSize; x++)
            {
                whiteCell = !whiteCell;
                for (int y = 0; y < fieldSize; y++)
                {
                    gameField[x, y] = whiteCell ? 0 : 1;
                    whiteCell = !whiteCell;
                }
            }
        }

        private void FillField()
        {
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < fieldSize; x++)
                {
                    if (gameField[x, y] == 1)
                    {
                        gameField[x, y] = 3;
                        gameField[(fieldSize - 1) - x, (fieldSize - 1) - y] = 2;
                    }
                }
            }
        }

        public int GetElement(Vector2Int coords)
        {
            return gameField[coords.x, coords.y];
        }

        public void RemoveElementAt(Vector2Int elementCoords)
        {
            gameField[elementCoords.x, elementCoords.y] = 0;
        }

        public void UpgradeElementAt(Vector2Int elementCoords)
        {
            gameField[elementCoords.x, elementCoords.y] *= 2;
        }

        public bool CanMoveElementTo(Vector2Int elementCoords)
        {
            return gameField[elementCoords.x, elementCoords.y] == 0;
        }

        public int[,] GetField()
        {
            return gameField;
        }
    }
}
