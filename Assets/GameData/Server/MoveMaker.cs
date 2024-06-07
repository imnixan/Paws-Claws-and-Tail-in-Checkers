using System;
using System.Collections.Generic;
using NUnit.Framework;
using PCTC.Game;
using PCTC.Scripts;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Server
{
    public class MoveMaker
    {
        private GameField gameField;

        public MoveMaker(GameField gameField)
        {
            this.gameField = gameField;
        }

        public MoveResult MakeMove(MoveData moveData)
        {
            List<MoveData> moves = new List<MoveData>();
            if (IsJustMove(moveData.catData.position, moveData.moveEnd))
            {
                moves.Add(moveData);
                gameField.RemoveElementAt(moveData.catData.position);
                CatData newCatData = new CatData(
                    moveData.catData.id,
                    moveData.moveEnd,
                    moveData.catData.type,
                    moveData.catData.team
                );
                gameField.SetElement(newCatData);
            }

            return new MoveResult(moves.ToArray());
        }

        private bool IsJustMove(Vector2Int start, Vector2Int end)
        {
            int deltaX = Math.Abs(end.x - start.x);
            int deltaY = Math.Abs(end.y - start.y);
            return (deltaX == 1 && deltaY == 1);
        }
    }
}
