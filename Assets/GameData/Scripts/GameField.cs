using PCTC.CatScripts;
using PCTC.Scripts;
using PCTC.Structs;
using UnityEditor;
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
        public string mapHash { get; private set; }

        private bool server;

        public GameField()
        {
            server = true;
            matrix = new CatData[fieldSize, fieldSize];
            BuildField();
            FillField();
            UpdateHash();
        }

        public GameField(CatData[,] gameField)
        {
            this.matrix = gameField;
            UpdateHash();
        }

        private void UpdateHash(bool rebuild = false)
        {
            Hash128 hash = new Hash128();

            foreach (var item in matrix)
            {
                hash.Append(item.id);
            }
            mapHash = hash.ToString();
            Debug.Log(
                $"server:{server}; rebuild:{rebuild}; update hash {mapHash} rawField {JsonUtility.ToJson(new PlayerInitData(0, ArrayTransformer.Flatten(matrix), new CatsCount()))}"
            );
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

        public void UpdateField(MoveResult moveResult)
        {
            Debug.Log(
                $"moves Server?{server} length{moveResult.moves.Length} first start {moveResult.moves[0].catData.position} end {moveResult.moves[0].moveEnd}"
            );
            foreach (var move in moveResult.moves)
            {
                RemoveElementAt(move.catData.position);
                CatData catData = move.catData;
                catData.position = move.moveEnd;
                SetElement(catData);
            }
            RemoveCats(moveResult);
            UpgradeCats(moveResult);
            UpdateHash();
        }

        private void UpgradeCats(MoveResult moveResult)
        {
            foreach (var upgradeCat in moveResult.catsForUpgrade)
            {
                CatData catData = GetElementById(upgradeCat.id);
                if (catData.type == Enums.CatsType.Type.Normal)
                {
                    catData.type = Enums.CatsType.Type.Chonky;
                }
                SetElement(catData);
            }
        }

        private void RemoveCats(MoveResult moveResult)
        {
            foreach (var carForRemove in moveResult.catsForRemove)
            {
                RemoveElementAt(carForRemove.position);
            }
        }

        //normal
        //private void FillField()
        //{
        //    int catId = 2;
        //    Enums.CatsType.Type defaultType = Enums.CatsType.Type.Normal;
        //    for (int x = 0; x < 3; x++)
        //    {
        //        for (int y = 0; y < fieldSize; y++)
        //        {
        //            if (matrix[x, y].id == 1)
        //            {
        //                matrix[x, y].id = catId++;
        //                matrix[x, y].team = Enums.CatsType.Team.Orange;
        //                matrix[x, y].type = defaultType;
        //                matrix[(fieldSize - 1) - x, (fieldSize - 1) - y].id = catId++;
        //                matrix[(fieldSize - 1) - x, (fieldSize - 1) - y].team = Enums
        //                    .CatsType
        //                    .Team
        //                    .Black;
        //                matrix[(fieldSize - 1) - x, (fieldSize - 1) - y].type = defaultType;
        //            }
        //        }
        //    }
        //}

        #region testnormal
        //test normals
        private void FillField()
        {
            int catId = 2;
            Enums.CatsType.Type defaultType = Enums.CatsType.Type.Normal;

            // Расставляем фишки для тестового поля
            // Оранжевая команда
            matrix[3, 6] = new CatData(
                catId++,
                new Vector2Int(3, 6),
                defaultType,
                Enums.CatsType.Team.Orange
            );

            matrix[5, 6] = new CatData(
                catId++,
                new Vector2Int(5, 6),
                defaultType,
                Enums.CatsType.Team.Orange
            );

            // Черная команда
            matrix[4, 3] = new CatData(
                catId++,
                new Vector2Int(4, 3),
                defaultType,
                Enums.CatsType.Team.Black
            );
            matrix[4, 1] = new CatData(
                catId++,
                new Vector2Int(4, 1),
                defaultType,
                Enums.CatsType.Team.Black
            );
            matrix[4, 5] = new CatData(
                catId++,
                new Vector2Int(4, 5),
                defaultType,
                Enums.CatsType.Team.Black
            );
        }
        #endregion

        #region testnormalMOREWAYS
        //test normals
        //private void FillField()
        //{
        //    int catId = 2;
        //    Enums.CatsType.Type defaultType = Enums.CatsType.Type.Normal;

        //    // Расставляем фишки для тестового поля
        //    // Оранжевая команда

        //    matrix[5, 6] = new CatData(
        //        catId++,
        //        new Vector2Int(5, 6),
        //        defaultType,
        //        Enums.CatsType.Team.Orange
        //    );

        //    // Черная команда
        //    matrix[4, 3] = new CatData(
        //        catId++,
        //        new Vector2Int(4, 3),
        //        defaultType,
        //        Enums.CatsType.Team.Black
        //    );
        //    matrix[4, 5] = new CatData(
        //        catId++,
        //        new Vector2Int(4, 5),
        //        defaultType,
        //        Enums.CatsType.Team.Black
        //    );
        //    matrix[2, 5] = new CatData(
        //        catId++,
        //        new Vector2Int(2, 5),
        //        defaultType,
        //        Enums.CatsType.Team.Black
        //    );
        //}
        #endregion


        #region testChonky
        //    //test chonky
        //    private void FillField()
        //    {
        //        int catId = 2;
        //        Enums.CatsType.Type defaultType = Enums.CatsType.Type.Chonky;

        //        // Расставляем фишки для тестового поля
        //        // Оранжевая команда
        //        matrix[2, 7] = new CatData(
        //            catId++,
        //            new Vector2Int(2, 7),
        //            defaultType,
        //            Enums.CatsType.Team.Orange
        //        );

        //        matrix[6, 7] = new CatData(
        //            catId++,
        //            new Vector2Int(6, 7),
        //            defaultType,
        //            Enums.CatsType.Team.Orange
        //        );

        //        // Черная команда
        //        matrix[4, 3] = new CatData(
        //            catId++,
        //            new Vector2Int(4, 3),
        //            defaultType,
        //            Enums.CatsType.Team.Black
        //        );
        //        matrix[4, 1] = new CatData(
        //            catId++,
        //            new Vector2Int(4, 1),
        //            defaultType,
        //            Enums.CatsType.Team.Black
        //        );
        //        matrix[4, 5] = new CatData(
        //            catId++,
        //            new Vector2Int(4, 5),
        //            defaultType,
        //            Enums.CatsType.Team.Black
        //        );
        //    }
        #endregion

        public CatData GetElement(Vector2Int coords)
        {
            if (coords.x < 0 || coords.y < 0 || coords.x >= fieldSize || coords.y >= fieldSize)
            {
                return new CatData(-1, coords);
            }
            return matrix[coords.x, coords.y];
        }

        public CatData GetElementById(int id)
        {
            foreach (var item in matrix)
            {
                if (item.id == id)
                {
                    return item;
                }
            }
            return new CatData(-1, Vector2Int.zero);
        }

        public void RemoveElementAt(Vector2Int elementCoords)
        {
            Debug.Log($"Server?{server} remove cat at {elementCoords}");
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
