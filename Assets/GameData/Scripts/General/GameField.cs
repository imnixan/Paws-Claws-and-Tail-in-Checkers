using PJTC.Enums;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Game
{
    public class GameField
    {
        //0 = white cell
        //1 = blackCell
        //1+n - catsId;

        public CatData[,] matrix { get; private set; }
        public const int fieldSize = 8;
        public string mapHash { get; private set; }

        private bool server;

        public enum FieldTypes
        {
            Normal8x8,
            TestNormal,
            TestNormalMulti,
            TestChonky
        }

        public GameField(FieldTypes type = FieldTypes.Normal8x8)
        {
            server = true;
            matrix = new CatData[fieldSize, fieldSize];

            BuildField();

            FillField(type);

            UpdateHash();
        }

        private void FillField(FieldTypes type)
        {
            switch (type)
            {
                case FieldTypes.Normal8x8:
                    FillFieldNormal();
                    break;
                case FieldTypes.TestNormal:
                    FillFieldTestNormal();
                    break;
                case FieldTypes.TestNormalMulti:
                    FillFieldTestNormalMultiway();
                    break;
                case FieldTypes.TestChonky:
                    FillFieldTestChonky();
                    break;
            }
        }

        public GameField(CatData[,] gameField)
        {
            this.matrix = gameField;

            UpdateHash();
        }

        public CatData[,] GetSecureField(CatsType.Team playerTeam)
        {
            CatData[,] secureField = matrix;
            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    if (secureField[x, y].team != playerTeam)
                    {
                        if (!secureField[x, y].attackHints.solved)
                        {
                            secureField[x, y].attackType = CatsType.Attack.None;
                        }
                    }
                }
            }

            return secureField;
        }

        private void UpdateHash(bool rebuild = false)
        {
            Hash128 hash = new Hash128();

            foreach (var item in matrix)
            {
                hash.Append(item.id);
            }
            mapHash = hash.ToString();
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
            foreach (CompletedMoveData move in moveResult.moves)
            {
                CatData movedCat = move.moveData.catData;

                RemoveElementAt(movedCat.position);

                movedCat.position = move.moveData.moveEnd;
                if (move.moveWithBattle)
                {
                    if (move.battleWin)
                    {
                        RemoveElementAt(move.enemy.position);
                    }
                    else
                    {
                        SetElement(move.enemy);
                    }
                }

                if (move.moveWithUpgrade)
                {
                    movedCat.type = CatsType.Type.Chonky;
                }

                SetElement(movedCat);
            }

            UpdateHash();
        }

        public MoveResult CensureMoveResultForPlayer(
            CatsType.Team playerTeam,
            MoveResult moveResult
        )
        {
            CompletedMoveData[] moves = moveResult.moves;
            CompletedMoveData[] censureMoves = new CompletedMoveData[moves.Length];

            for (int i = 0; i < moves.Length; i++)
            {
                censureMoves[i] = moves[i];
                censureMoves[i].moveData.catData = CensureCat(
                    moves[i].moveData.catData,
                    playerTeam
                );

                if (censureMoves[i].moveWithBattle)
                {
                    censureMoves[i].enemy = CensureCat(moves[i].enemy, playerTeam);
                }
            }

            return new MoveResult(censureMoves, moveResult.catsCount);
        }

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

        private CatData CensureCat(CatData catData, CatsType.Team playerTeam)
        {
            if (catData.team != playerTeam && !catData.attackHints.solved)
            {
                catData.attackType = CatsType.Attack.None;
            }

            return catData;
        }

        private void FillFieldNormal()
        {
            int catId = 2;
            Enums.CatsType.Type defaultType = Enums.CatsType.Type.Normal;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    if (matrix[x, y].id == 1)
                    {
                        matrix[x, y].id = catId++;
                        matrix[x, y].team = Enums.CatsType.Team.Orange;
                        matrix[x, y].type = defaultType;
                        matrix[(fieldSize - 1) - x, (fieldSize - 1) - y].id = catId++;
                        matrix[(fieldSize - 1) - x, (fieldSize - 1) - y].team = Enums
                            .CatsType
                            .Team
                            .Black;
                        matrix[(fieldSize - 1) - x, (fieldSize - 1) - y].type = defaultType;
                    }
                }
            }
        }

        #region testnormal
        //test normals
        private void FillFieldTestNormal()
        {
            int catId = 2;
            Enums.CatsType.Type defaultType = Enums.CatsType.Type.Normal;

            // ����������� ����� ��� ��������� ����
            // ��������� �������
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

            // ������ �������
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
        private void FillFieldTestNormalMultiway()
        {
            int catId = 2;
            Enums.CatsType.Type defaultType = Enums.CatsType.Type.Normal;

            // ����������� ����� ��� ��������� ����
            // ��������� �������

            matrix[5, 6] = new CatData(
                catId++,
                new Vector2Int(5, 6),
                defaultType,
                Enums.CatsType.Team.Orange
            );

            // ������ �������
            matrix[4, 3] = new CatData(
                catId++,
                new Vector2Int(4, 3),
                defaultType,
                Enums.CatsType.Team.Black
            );
            matrix[4, 5] = new CatData(
                catId++,
                new Vector2Int(4, 5),
                defaultType,
                Enums.CatsType.Team.Black
            );
            matrix[2, 5] = new CatData(
                catId++,
                new Vector2Int(2, 5),
                defaultType,
                Enums.CatsType.Team.Black
            );
        }
        #endregion


        #region testChonky
        //test chonky
        private void FillFieldTestChonky()
        {
            int catId = 2;
            Enums.CatsType.Type defaultType = Enums.CatsType.Type.Chonky;

            // ����������� ����� ��� ��������� ����
            // ��������� �������
            matrix[2, 7] = new CatData(
                catId++,
                new Vector2Int(2, 7),
                defaultType,
                Enums.CatsType.Team.Orange,
                Enums.CatsType.Attack.Paws
            );

            matrix[6, 7] = new CatData(
                catId++,
                new Vector2Int(6, 7),
                defaultType,
                Enums.CatsType.Team.Orange,
                Enums.CatsType.Attack.Jaws
            );

            // ������ �������
            matrix[4, 3] = new CatData(
                catId++,
                new Vector2Int(4, 3),
                defaultType,
                Enums.CatsType.Team.Black,
                Enums.CatsType.Attack.Tail
            );
            matrix[4, 1] = new CatData(
                catId++,
                new Vector2Int(4, 1),
                defaultType,
                Enums.CatsType.Team.Black,
                Enums.CatsType.Attack.Tail
            );
            matrix[4, 5] = new CatData(
                catId++,
                new Vector2Int(4, 5),
                defaultType,
                Enums.CatsType.Team.Black,
                Enums.CatsType.Attack.Tail
            );
        }
        #endregion
    }
}
