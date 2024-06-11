using System.ComponentModel;
using PCTC.Enums;
using PCTC.Server;
using UnityEngine;

namespace PCTC.Structs
{
    [System.Serializable]
    public class ClientServerMessage
    {
        public int type;
        public string data;

        public ClientServerMessage(int type, string data)
        {
            this.type = type;

            this.data = data;
        }
    }

    [System.Serializable]
    public class DataFromPlayer
    {
        public ClientServerMessage message;
        public int playerID;

        public DataFromPlayer(ClientServerMessage message, int playerID)
        {
            this.message = message;
            this.playerID = playerID;
        }
    }

    [System.Serializable]
    public class PlayerInitData
    {
        public int playerID;
        public CatData[] gameField;

        public PlayerInitData(int playerID, CatData[] gameField)
        {
            this.playerID = playerID;
            this.gameField = gameField;
        }
    }

    [System.Serializable]
    public class CatData
    {
        public int id;
        public Vector2Int position;
        public CatsType.Type type;
        public CatsType.Team team;

        public CatData(
            int catID,
            Vector2Int position,
            CatsType.Type type = CatsType.Type.None,
            CatsType.Team team = CatsType.Team.None
        )
        {
            this.id = catID;
            this.position = position;
            this.type = type;
            this.team = team;
        }
    }

    [System.Serializable]
    public class PlayerOrder
    {
        public bool playerOrder;

        public PlayerOrder(bool playerOrder)
        {
            this.playerOrder = playerOrder;
        }
    }

    [System.Serializable]
    public class Moves
    {
        public Vector2Int[] possibleMoves;

        public Moves(Vector2Int[] possibleMoves = null)
        {
            this.possibleMoves = possibleMoves;
        }
    }

    [System.Serializable]
    public class MoveResult
    {
        public MoveData[] moves;
        public CatData[] catsForRemove;
        public CatData[] catsForUpgrade;
        public CatsCount catsCount;

        public MoveResult(
            MoveData[] moves,
            CatData[] catsForRemove = null,
            CatData[] catsForUpgrade = null,
            CatsCount catsCount = null
        )
        {
            this.moves = moves;
            this.catsForRemove = catsForRemove;
            this.catsForUpgrade = catsForUpgrade;
            this.catsCount = catsCount;
        }
    }

    [System.Serializable]
    public class CatsCount
    {
        public int orangeCats;
        public int orangeChonkyCats;
        public int blackCats;
        public int blackChonkyCats;

        public CatsCount(
            int orangeCats = 0,
            int orangeChonkyCats = 0,
            int blackCats = 0,
            int orangeBlackCats = 0
        )
        {
            this.orangeCats = orangeCats;
            this.blackCats = blackCats;
            this.orangeChonkyCats = orangeChonkyCats;
            this.blackChonkyCats = blackChonkyCats;
        }
    }

    [System.Serializable]
    public class MoveData
    {
        public CatData catData;
        public Vector2Int moveEnd;
        public string battleAnimationId;

        public MoveData(CatData catData, Vector2Int moveEnd, string battleAnimationId = "")
        {
            this.catData = catData;
            this.moveEnd = moveEnd;
            this.battleAnimationId = battleAnimationId;
        }
    }

    [System.Serializable]
    public class GameResult
    {
        public int winnerID;
        public int reason;

        public GameResult(int winnerID, int reason)
        {
            this.winnerID = winnerID;
            this.reason = reason;
        }
    }

    [System.Serializable]
    public class MapHash
    {
        public string maphash;

        public MapHash(string maphash)
        {
            this.maphash = maphash;
        }
    }
}
