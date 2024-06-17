using System;
using UnityEngine;

namespace PJTC.Enums
{
    public static class CSMRequest
    {
        public enum Type
        {
            PLAYER_INIT,
            PLAYER_SYNC,
            SET_ATTACK,
            GAME_START,
            SET_PLAYER_ORDER,
            POSSIBLE_MOVES,
            MAKE_MOVE,
            PLAYER_READY,
            ITEM_CLICK,
            ACK,
            MESSAGE_REQUEST,
            DRAW_ALARM,
            GAME_END
        }
    }

    public static class CatsType
    {
        public enum Team
        {
            Orange,
            Black,
            None
        }

        public enum Type
        {
            None,
            Normal,
            Chonky
        }

        public enum Attack
        {
            None,
            Paws,
            Jaws,
            Tail
        }
    }

    public static class GameData
    {
        public enum GameState
        {
            None,
            PlayerInit,
            WaitingMatchStart,
            Game,
            GameEnd
        }

        public enum EndGameReason
        {
            Disconnect,
            Draw,
            GiveUp,
            Clear,
            Stuck
        }
    }
}
