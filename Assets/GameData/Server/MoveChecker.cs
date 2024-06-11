using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using PCTC.CatScripts;
using PCTC.Enums;
using PCTC.Game;
using PCTC.Structs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace PCTC.Server
{
    public class MoveChecker
    {
        public GameField gameField;

        public MoveChecker(GameField gameField)
        {
            this.gameField = gameField;
        }

        public Moves GetPossibleMoves(CatData catData)
        {
            List<Vector2Int> possibleMoves = new List<Vector2Int>();
            possibleMoves.AddRange(GetWalkCells(catData));
            Vector2Int[] movesArray = possibleMoves.ToArray();
            return new Moves(movesArray);
        }

        public bool IsCorrectMove(MoveData move)
        {
            Moves moves = GetPossibleMoves(move.catData);
            bool correctMove = moves.possibleMoves.Contains(move.moveEnd);
            return correctMove;
        }

        public bool CheckPlayerStuck(CatsType.Team team)
        {
            int count = 0;
            foreach (var cat in gameField.matrix)
            {
                if (cat.team == team)
                {
                    count += GetPossibleMoves(cat).possibleMoves.Length;
                }
            }
            return count == 0;
        }

        //orange move x++, black move x--;
        private List<Vector2Int> GetWalkCells(CatData cat)
        {
            bool isChonky = cat.type == Enums.CatsType.Type.Chonky;
            bool canMoveUp = cat.team == Enums.CatsType.Team.Orange || isChonky;
            bool canMoveDown = cat.team == Enums.CatsType.Team.Black || isChonky;

            int minX = canMoveDown ? -1 : 0;
            int maxX = canMoveUp ? 1 : 0;

            int moveLenghth = isChonky ? 7 : 1;

            return GetDiagonalMoves(cat, minX, maxX, moveLenghth);
        }

        private List<Vector2Int> GetDiagonalMoves(
            CatData catData,
            int minX,
            int maxX,
            int moveLength
        )
        {
            Vector2Int nullPoint = catData.position;
            List<Vector2Int> diagonalMoves = new List<Vector2Int>();

            for (int xMod = -1; xMod <= 1; xMod++)
            {
                if (xMod == 0)
                    continue;
                for (int yMod = -1; yMod <= 1; yMod++)
                {
                    if (yMod == 0)
                        continue;
                    bool secondJump = false;
                    for (int step = 1; step <= moveLength; step++)
                    {
                        int xMove = xMod * step;
                        int yMove = yMod * step;
                        Vector2Int checkedCell = new Vector2Int(
                            nullPoint.x + xMove,
                            nullPoint.y + yMove
                        );
                        bool canMoveLikeThis = xMod >= minX && xMod <= maxX;
                        if (gameField.GetElement(checkedCell).id == 1)
                        {
                            if (canMoveLikeThis)
                            {
                                diagonalMoves.Add(checkedCell);
                            }
                        }
                        else if (gameField.GetElement(checkedCell).team == catData.team)
                        {
                            break;
                        }
                        else if (!secondJump)
                        {
                            checkedCell.x += xMod;
                            checkedCell.y += yMod;
                            if (gameField.GetElement(checkedCell).id == 1)
                            {
                                diagonalMoves.Add(checkedCell);
                                step++;
                                secondJump = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return diagonalMoves;
        }
    }
}
