using System;
using UnityEngine;

namespace PJTC.Enums
{
    public static class CSMRequest
    {
        public enum Type
        {
            PLAYER_INIT,
            FILL_ATTACKS,
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
            Idle,
            GameStart,
            PlayerInit,
            AttackInit,
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
