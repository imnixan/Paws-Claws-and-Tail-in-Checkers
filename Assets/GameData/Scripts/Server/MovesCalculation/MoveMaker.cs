using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using PJTC.CatScripts;
using PJTC.Enums;
using PJTC.Game;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Server
{
    public class MoveMaker
    {
        private GameField gameField;
        private MoveChecker moveChecker;
        System.Random random = new System.Random();

        public Dictionary<CatsType.Attack, CatsType.Attack> attackMap = new Dictionary<
            CatsType.Attack,
            CatsType.Attack
        >()
        {
            { CatsType.Attack.None, CatsType.Attack.None },
            { CatsType.Attack.Paws, CatsType.Attack.Tail },
            { CatsType.Attack.Tail, CatsType.Attack.Jaws },
            { CatsType.Attack.Jaws, CatsType.Attack.Paws },
        };

        public MoveMaker(GameField gameField, MoveChecker moveChecker)
        {
            this.gameField = gameField;
            this.moveChecker = moveChecker;
        }

        public MoveResult MakeMove(
            MoveData moveData,
            bool firstAttack = true,
            MoveResult moveResult = new MoveResult()
        )
        {
            List<MoveData> moves = new List<MoveData>(moveResult.moves ?? new MoveData[0]);
            List<CatData> catsForRemove = new List<CatData>(
                moveResult.catsForRemove ?? new CatData[0]
            );
            List<CatData> catsForUpgrade = new List<CatData>(
                moveResult.catsForUpgrade ?? new CatData[0]
            );

            CatData catchedCat = TryCatchCat(moveData);

            bool canBeat = moveData.catData.attackType == attackMap[catchedCat.attackType];

            if (catchedCat.id > 1)
            {
                if (firstAttack && !canBeat)
                {
                    int xDir = System.Math.Sign(
                        catchedCat.position.x - moveData.catData.position.x
                    );
                    int yDir = System.Math.Sign(
                        catchedCat.position.y - moveData.catData.position.y
                    );
                    moveData.moveEnd.x = catchedCat.position.x - xDir;
                    moveData.moveEnd.y = catchedCat.position.y - yDir;
                    moves.Add(moveData);

                    return new MoveResult(
                        moves.ToArray(),
                        catsForRemove.ToArray(),
                        catsForUpgrade.ToArray(),
                        CountCats()
                    );
                }

                moves.Add(moveData);
                catsForRemove.Add(catchedCat);
            }
            else
            {
                moves.Add(moveData);

                return new MoveResult(
                    moves.ToArray(),
                    catsForRemove.ToArray(),
                    catsForUpgrade.ToArray(),
                    CountCats()
                );
            }

            catsForUpgrade = UpdgradeCats(moves);
            moveResult = new MoveResult(
                moves.ToArray(),
                catsForRemove.ToArray(),
                catsForUpgrade.ToArray(),
                CountCats()
            );

            gameField.UpdateField(moveResult);

            moveData.catData.position = moveData.moveEnd;
            Moves possibleMoves = moveChecker.GetPossibleMoves(moveData.catData);
            List<MoveData> possibleAttackMoves = new List<MoveData>();

            foreach (var moveEndCoord in possibleMoves.possibleMoves)
            {
                MoveData move = new MoveData(moveData.catData, moveEndCoord);
                CatData cat = TryCatchCat(move);

                if (cat.id > 1 && !catsForRemove.Contains(cat))
                {
                    possibleAttackMoves.Add(move);
                }
            }

            if (possibleAttackMoves.Count > 0)
            {
                int nextRandomMove = random.Next(possibleAttackMoves.Count);
                moveData = possibleAttackMoves[nextRandomMove];

                return MakeMove(moveData, false, moveResult);
            }

            return moveResult;
        }

        public CatsCount CountCats()
        {
            CatsCount cats = new CatsCount();
            foreach (var cat in gameField.matrix)
            {
                switch (cat.type)
                {
                    case CatsType.Type.Chonky:
                        if (cat.team == CatsType.Team.Orange)
                            cats.orangeChonkyCats++;
                        if (cat.team == CatsType.Team.Black)
                            cats.blackChonkyCats++;
                        break;
                    case CatsType.Type.Normal:
                        if (cat.team == CatsType.Team.Orange)
                            cats.orangeCats++;
                        if (cat.team == CatsType.Team.Black)
                            cats.blackCats++;
                        break;
                }
            }

            return cats;
        }

        private Vector2Int[] CropStartPoint(Vector2Int[] moves, Vector2Int startMove)
        {
            List<Vector2Int> croppedMoves = new List<Vector2Int>();
            foreach (var move in moves)
            {
                if (move != startMove)
                {
                    croppedMoves.Add(move);
                }
            }
            return croppedMoves.ToArray();
        }

        public CatData TryCatchCat(MoveData move)
        {
            CatData catchedCat = new CatData(-1, new Vector2Int(0, 0));
            List<Vector2Int> path = GetPath(move.catData.position, move.moveEnd);
            foreach (var cellCoords in path)
            {
                CatData catForAttack = gameField.GetElement(cellCoords);

                if (moveChecker.CanBeEaten(move.catData, catForAttack))
                {
                    catchedCat = catForAttack;
                    break;
                }
            }

            return catchedCat;
        }

        private List<Vector2Int> GetPath(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            int dirX = (end.x > start.x) ? 1 : -1;
            int dirY = (end.y > start.y) ? 1 : -1;

            int x = start.x + dirX;
            int y = start.y + dirY;

            while (x != end.x && y != end.y)
            {
                path.Add(new Vector2Int(x, y));
                x += dirX;
                y += dirY;
            }

            return path;
        }

        private List<CatData> UpdgradeCats(List<MoveData> moves)
        {
            List<CatData> catsForUpgrade = new List<CatData>();
            foreach (var move in moves)
            {
                bool orangeOnTop =
                    move.moveEnd.x == GameField.fieldSize - 1
                    && move.catData.team == CatsType.Team.Orange;
                bool blackOnBot = move.moveEnd.x == 0 && move.catData.team == CatsType.Team.Black;
                bool chonky = (orangeOnTop || blackOnBot) && !catsForUpgrade.Contains(move.catData);
                if (chonky)
                {
                    catsForUpgrade.Add(move.catData);
                }
            }

            return catsForUpgrade;
        }
    }
}
