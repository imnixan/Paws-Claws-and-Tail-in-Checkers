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
            PlAYER_MOVE_FINISH
        }

        public enum ServerRequests
        {
            POSSIBLE_MOVES,
            MANDATORY_MOVES,
            MOVE_RESULT,
            ITEM_CLICK,
            GAME_RESULT,
            PLAYER_INIT,
            SET_PLAYER_ORDER
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
}
