using System;
using System.Drawing.Text;
using PJTC.Game;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Server.MovesCalculation
{
    public class RepeatMovesChecker
    {
        private CatsCount oldCatsCount = new CatsCount();
        private int repeats;

        public int GetMoveRepeats(MoveResult moveResult)
        {
            bool countsSame = moveResult.catsCount.Equals(oldCatsCount);
            bool chonkyMove =
                moveResult.moves[0].moveData.catData.type == Enums.CatsType.Type.Chonky;

            if (countsSame && chonkyMove)
            {
                repeats++;
            }
            else
            {
                repeats = 0;
            }
            return repeats;
        }
    }
}
