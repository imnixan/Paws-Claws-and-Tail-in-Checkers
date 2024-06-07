using System;
using System.Collections.Generic;
using NUnit.Framework;
using PCTC.Game;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Server
{
    public class MoveChecker
    {
        private GameField gameField;

        public MoveChecker(GameField gameField)
        {
            this.gameField = gameField;
        }

        public Moves GetPossibleMoves(CatData catData)
        {
            List<Vector2Int> possibleMoves = new List<Vector2Int>();
            possibleMoves.AddRange(GetNearCells(catData));
            possibleMoves.AddRange(GetAttackCells(catData));
            Vector2Int[] movesArray = possibleMoves.ToArray();
            return new Moves(movesArray);
        }

        public bool IsCorrectMove(MoveData move)
        {
            Moves moves = GetPossibleMoves(move.catData);
            return (Array.Exists(moves.possibleMoves, cell => cell == move.moveEnd));
        }

        //orange move y++, black move y--;
        private List<Vector2Int> GetNearCells(CatData cat)
        {
            List<Vector2Int> possibleMoves = new List<Vector2Int>();

            bool canMoveUp =
                cat.team == Enums.CatsType.Team.Orange || cat.type == Enums.CatsType.Type.Chonky;
            bool canMoveDown =
                cat.team == Enums.CatsType.Team.Black || cat.type == Enums.CatsType.Type.Chonky;
            int xMin = canMoveDown ? -1 : 1;
            int XMax = canMoveUp ? 1 : -1;
            for (int x = xMin; x <= XMax; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    Vector2Int checkedCell = new Vector2Int(cat.position.x + x, cat.position.y + y);
                    if (gameField.GetElement(checkedCell).id == 1)
                    {
                        possibleMoves.Add(checkedCell);
                    }
                }
            }

            return possibleMoves;
        }

        private List<Vector2Int> GetAttackCells(CatData cat)
        {
            List<Vector2Int> possibleMoves = new List<Vector2Int>();
            return possibleMoves;
        }
    }
}
