using System;
using System.Collections.Generic;
using System.Linq;
using PJTC.Enums;
using PJTC.Game;
using PJTC.General;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Server
{
    public class MoveChecker
    {
        public GameField gameField;
        private ServerGameManager gameManager;

        public MoveChecker(ServerGameManager gameManager, GameField gameField)
        {
            this.gameManager = gameManager;
            this.gameField = gameField;
        }

        public Moves GetPossibleMoves(CatData catData, bool checkAttack = true)
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

        public bool IsPlayerStuck(CatsType.Team team)
        {
            int count = 0;
            foreach (var cat in gameField.matrix)
            {
                if (cat.team == team)
                {
                    Moves moves = GetPossibleMoves(cat);
                    count += GetSuccesMovesCount(cat, moves);
                }
            }
            return count == 0;
        }

        public bool CanBeEaten(CatData attacker, CatData defender)
        {
            bool defenderIsCat = defender.id > 1;
            bool oppositeTeam = attacker.team != defender.team;

            return defenderIsCat && oppositeTeam;
        }

        private int GetSuccesMovesCount(CatData cat, Moves uncuttedMoves)
        {
            List<Vector2Int> moves = new List<Vector2Int>();
            foreach (var move in uncuttedMoves.possibleMoves)
            {
                CatData catchedCat = gameManager.moveMaker.TryCatchCat(new MoveData(cat, move));
                bool canBeat = cat.attackType == AttackMap.attackMap[catchedCat.attackType];
                if (catchedCat.id <= 1 || canBeat)
                {
                    moves.Add(move);
                }
            }
            return moves.Count;
        }

        //orange move x++, black move x--;
        private List<Vector2Int> GetWalkCells(CatData cat)
        {
            bool isChonky = cat.type == Enums.CatsType.Type.Chonky;
            bool canMoveForward = cat.team == Enums.CatsType.Team.Orange || isChonky;
            bool canMoveBack = cat.team == Enums.CatsType.Team.Black || isChonky;

            int minX = canMoveBack ? -1 : 0;
            int maxX = canMoveForward ? 1 : 0;
            //field size
            int maxMoveLenghth = isChonky ? GameField.fieldSize - 1 : 1;

            return GetDiagonalMoves(cat, minX, maxX, maxMoveLenghth);
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
            //named macroses
            for (int xDir = -1; xDir <= 1; xDir++)
            {
                if (xDir == 0)
                    continue;
                for (int yMod = -1; yMod <= 1; yMod++)
                {
                    if (yMod == 0)
                        continue;
                    bool haveEnemyOnDirection = false;
                    for (int step = 1; step <= moveLength; step++)
                    {
                        int xMove = xDir * step;
                        int yMove = yMod * step;
                        Vector2Int checkedCell = new Vector2Int(
                            nullPoint.x + xMove,
                            nullPoint.y + yMove
                        );

                        CatData checkedCat = gameField.GetElement(checkedCell);

                        bool validDirection = xDir >= minX && xDir <= maxX;

                        if (validDirection == false)
                        {
                            break;
                        }
                        if (!CellIsValid(checkedCat))
                        {
                            break;
                        }

                        if (IsSameTeam(catData, checkedCat))
                        {
                            break;
                        }
                        if (CanBeEaten(catData, checkedCat))
                        {
                            if (haveEnemyOnDirection)
                            {
                                break;
                            }
                            haveEnemyOnDirection = true;
                        }
                        if (CellIsEmpty(checkedCat))
                        {
                            diagonalMoves.Add(checkedCell);
                        }
                    }

                    int edgePosX = xDir * moveLength;
                    int edgePosY = yMod * moveLength;
                    Vector2Int edgeCell = new Vector2Int(
                        nullPoint.x + edgePosX,
                        nullPoint.y + edgePosY
                    );

                    CatData edgeCat = gameField.GetElement(edgeCell);
                    if (CellIsValid(edgeCat) && CanBeEaten(catData, edgeCat))
                    {
                        edgeCell.x += xDir;
                        edgeCell.y += yMod;
                        CatData nextCat = gameField.GetElement(edgeCell);
                        if (CellIsValid(nextCat) && CellIsEmpty(nextCat))
                        {
                            diagonalMoves.Add(edgeCell);
                        }
                    }
                }
            }

            return diagonalMoves;
        }

        private bool CellIsEmpty(CatData checkedCat)
        {
            return checkedCat.id == 1;
        }

        private bool CellIsValid(CatData checkedCat)
        {
            return checkedCat.id != -1;
        }

        private bool IsSameTeam(CatData checkedCat1, CatData checkedCat2)
        {
            return checkedCat1.team == checkedCat2.team;
        }
    }
}
