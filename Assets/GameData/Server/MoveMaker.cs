using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using PCTC.Enums;
using PCTC.Game;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Server
{
    public class MoveMaker
    {
        private GameField gameField;
        private MoveChecker moveChecker;
        System.Random random = new System.Random();

        public MoveMaker(GameField gameField, MoveChecker moveChecker)
        {
            this.gameField = gameField;
            this.moveChecker = moveChecker;
        }

        public MoveResult MakeMove(MoveData moveData)
        {
            List<MoveData> moves = new List<MoveData>();
            List<CatData> catsForRemove = new List<CatData>();
            List<CatData> catsForUpgrade = new List<CatData>();
            MoveData currentMove = moveData;
            bool needChoice = false;
            bool findNewPaths = true;
            while (findNewPaths)
            {
                moves.Add(currentMove);
                CatData catData = TryCatchCat(currentMove);
                if (catData != null)
                {
                    if (catsForRemove.Contains(catData))
                    {
                        moves.Remove(currentMove);
                    }
                    else
                    {
                        catsForRemove.Add(catData);
                    }
                    CatData newCatData = new CatData(
                        currentMove.catData.id,
                        currentMove.moveEnd,
                        currentMove.catData.type,
                        currentMove.catData.team
                    );
                    Moves newMove = moveChecker.GetPossibleMoves(newCatData);
                    newMove.possibleMoves = CropStartPoint(
                        newMove.possibleMoves,
                        currentMove.catData.position
                    );
                    List<MoveData> movesWithCombo = new List<MoveData>();

                    foreach (var move in newMove.possibleMoves)
                    {
                        MoveData newMoveData = new MoveData(newCatData, move);
                        CatData catchedCat = TryCatchCat(newMoveData);
                        if (catchedCat != null)
                        {
                            movesWithCombo.Add(newMoveData);
                        }
                    }
                    Debug.Log($"COMBO {movesWithCombo.Count}");
                    if (movesWithCombo.Count > 0)
                    {
                        int nextMoveNum = random.Next(0, movesWithCombo.Count);
                        Debug.Log($"nextRandom {nextMoveNum}");
                        currentMove = movesWithCombo[nextMoveNum];
                    }
                    else
                    {
                        findNewPaths = false;
                    }
                }
                else
                {
                    Debug.Log("no cats catched");
                    findNewPaths = false;
                }
            }

            catsForUpgrade = UpdgradeCats(moves);
            MoveResult moveResult = new MoveResult(
                moves.ToArray(),
                catsForRemove.ToArray(),
                catsForUpgrade.ToArray()
            );
            gameField.UpdateField(moveResult);

            return moveResult;
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

        private CatData TryCatchCat(MoveData move)
        {
            CatData catchedCat = null;

            List<Vector2Int> path = GetPath(move.catData.position, move.moveEnd);
            foreach (var cellCoords in path)
            {
                CatData cell = gameField.GetElement(cellCoords);
                CatsType.Team team = cell.team;
                bool teamNotNone = team != CatsType.Team.None;
                bool oppositeTeam = team != move.catData.team;
                if (teamNotNone && oppositeTeam)
                {
                    catchedCat = cell;
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
                bool orangeOnTop = move.moveEnd.x == 7 && move.catData.team == CatsType.Team.Orange;
                bool blackOnBot = move.moveEnd.x == -7 && move.catData.team == CatsType.Team.Black;
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
