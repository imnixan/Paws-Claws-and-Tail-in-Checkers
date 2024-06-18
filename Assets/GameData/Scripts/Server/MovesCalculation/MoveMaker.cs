using System.Collections.Generic;
using PJTC.Enums;
using PJTC.Game;
using PJTC.General;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Server
{
    public class MoveMaker
    {
        private GameField gameField;
        private MoveChecker moveChecker;
        private System.Random random = new System.Random();

        public MoveMaker(GameField gameField, MoveChecker moveChecker)
        {
            this.gameField = gameField;
            this.moveChecker = moveChecker;
        }

        public MoveResult MakeMove(
            CompletedMoveData completedMove,
            bool firstAttack = true,
            MoveResult moveResult = new MoveResult()
        )
        {
            List<CompletedMoveData> completedMoves = new List<CompletedMoveData>(
                moveResult.moves ?? new CompletedMoveData[0]
            );
            CatData catchedCat = TryCatchCat(completedMove.moveData);

            bool canBeat =
                completedMove.moveData.catData.attackType
                == AttackMap.attackMap[catchedCat.attackType];

            if (catchedCat.id > 1)
            {
                if (firstAttack && !canBeat)
                {
                    int xDir = System.Math.Sign(
                        catchedCat.position.x - completedMove.moveData.catData.position.x
                    );
                    int yDir = System.Math.Sign(
                        catchedCat.position.y - completedMove.moveData.catData.position.y
                    );
                    completedMove.moveData.moveEnd.x = catchedCat.position.x - xDir;
                    completedMove.moveData.moveEnd.y = catchedCat.position.y - yDir;

                    completedMove.moveData.catData.UpdateAttackerHints(
                        false,
                        completedMove.enemy.attackType
                    );
                    catchedCat.UpdateDefenderHints(
                        false,
                        completedMove.moveData.catData.attackType
                    );

                    completedMoves.Add(
                        new CompletedMoveData(
                            completedMove.moveData,
                            false,
                            true,
                            false,
                            catchedCat
                        )
                    );

                    return new MoveResult(completedMoves.ToArray(), CountCats());
                }

                bool chonky = NeedChonkyUpgrade(
                    completedMove.moveData.catData,
                    completedMove.moveData.moveEnd
                );

                if (chonky)
                {
                    completedMove.moveData.catData.type = CatsType.Type.Chonky;
                }

                completedMove.moveData.catData.UpdateAttackerHints(
                    true,
                    completedMove.enemy.attackType
                );
                catchedCat.UpdateDefenderHints(true, completedMove.moveData.catData.attackType);
                completedMoves.Add(
                    new CompletedMoveData(completedMove.moveData, chonky, true, true, catchedCat)
                );
            }
            else
            {
                bool chonky = NeedChonkyUpgrade(
                    completedMove.moveData.catData,
                    completedMove.moveData.moveEnd
                );

                if (chonky)
                {
                    completedMove.moveData.catData.type = CatsType.Type.Chonky;
                }

                completedMoves.Add(new CompletedMoveData(completedMove.moveData, chonky));

                return new MoveResult(completedMoves.ToArray(), CountCats());
            }

            moveResult = new MoveResult(completedMoves.ToArray(), CountCats());

            gameField.UpdateField(moveResult);

            completedMove.moveData.catData.position = completedMove.moveData.moveEnd;
            Moves possibleMoves = moveChecker.GetPossibleMoves(completedMove.moveData.catData);
            List<MoveData> possibleAttackMoves = new List<MoveData>();

            foreach (var moveEndCoord in possibleMoves.possibleMoves)
            {
                MoveData move = new MoveData(completedMove.moveData.catData, moveEndCoord);
                CatData cat = TryCatchCat(move);

                if (cat.id > 1 && !DoubleCatch(cat, moveResult))
                {
                    possibleAttackMoves.Add(move);
                }
            }

            if (possibleAttackMoves.Count > 0)
            {
                int nextRandomMove = random.Next(possibleAttackMoves.Count);
                completedMove = new CompletedMoveData(possibleAttackMoves[nextRandomMove]);

                return MakeMove(completedMove, false, moveResult);
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

        private bool NeedChonkyUpgrade(CatData cat, Vector2Int finalPos)
        {
            bool orangeOnTop =
                finalPos.x == GameField.fieldSize - 1 && cat.team == CatsType.Team.Orange;
            bool blackOnBot = finalPos.x == 0 && cat.team == CatsType.Team.Black;
            bool chonky = (orangeOnTop || blackOnBot);
            return chonky;
        }

        private bool DoubleCatch(CatData victim, MoveResult moveResult)
        {
            foreach (CompletedMoveData completedMoveData in moveResult.moves)
            {
                if (completedMoveData.battleWin && completedMoveData.enemy.id == victim.id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
