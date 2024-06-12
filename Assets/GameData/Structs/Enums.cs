using System;
using UnityEngine;

namespace PCTC.Enums
{
    public static class CSMRequest
    {
        public enum Type
        {
            PLAYER_INIT,
            GAME_START,
            SET_PLAYER_ORDER,
            POSSIBLE_MOVES,
            MAKE_MOVE,
            PLAYER_READY,
            ITEM_CLICK,
            ACK,
            MESSAGE_REQUEST,
            GAME_END
        }
    }

    public static class CatsType
    {
        public enum Team
        {
            None,
            Orange,
            Black
        }

        public enum Type
        {
            None,
            Normal,
            Chonky
        }
    }

    public static class GameData
    {
        public enum GameState
        {
            Idle,
            GameStart,
            Game,
            GameEnd
        }

        public enum EndGameReason
        {
            Disconnect,
            GiveUp,
            Clear,
            Stuck
        }
    }
}
