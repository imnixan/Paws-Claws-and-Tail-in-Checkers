using System;
using UnityEngine;

namespace PCTC.Enums
{
    public static class RequestTypes
    {
        public enum ClientRequests
        {
            PLAYER_CLICK_ITEM,
            PLAYER_CHOOSED_CAT,
            PLAYER_MOVE,
            PLAYER_READY,
            PLAYER_DISCONNECTED
        }

        public enum ServerRequests
        {
            POSSIBLE_MOVES,
            MANDATORY_MOVES,
            MOVE_RESULT,
            ITEM_CLICK,
            GAME_RESULT,
            PLAYER_INIT,
            SET_PLAYER_ORDER,
            START_GAME
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
