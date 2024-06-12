using System.ComponentModel;
using PCTC.Enums;
using PCTC.Server;
using UnityEngine;

namespace PCTC.Structs
{
    [System.Serializable]
    public struct ClientServerMessage
    {
        public int type;
        public string data;
        public int messageID;

        public ClientServerMessage(int type, string data, int messageID = -1)
        {
            this.type = type;
            this.data = data;
            this.messageID = messageID;
        }
    }

    [System.Serializable]
    public struct DataFromPlayer
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
    public struct PlayerInitData
    {
        public int playerID;
        public CatData[] gameField;
        public CatsCount catsCount;

        public PlayerInitData(int playerID, CatData[] gameField, CatsCount catsCount)
        {
            this.playerID = playerID;
            this.gameField = gameField;
            this.catsCount = catsCount;
        }
    }

    [System.Serializable]
    public struct CatData
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
    public struct PlayerOrder
    {
        public bool playerOrder;

        public PlayerOrder(bool playerOrder)
        {
            this.playerOrder = playerOrder;
        }
    }

    [System.Serializable]
    public struct Moves
    {
        public Vector2Int[] possibleMoves;

        public Moves(Vector2Int[] possibleMoves = null)
        {
            this.possibleMoves = possibleMoves;
        }
    }

    [System.Serializable]
    public struct MoveResult
    {
        public MoveData[] moves;
        public CatData[] catsForRemove;
        public CatData[] catsForUpgrade;
        public CatsCount catsCount;

        public MoveResult(
            MoveData[] moves,
            CatData[] catsForRemove = null,
            CatData[] catsForUpgrade = null,
            CatsCount catsCount = new CatsCount()
        )
        {
            this.moves = moves;
            this.catsForRemove = catsForRemove;
            this.catsForUpgrade = catsForUpgrade;
            this.catsCount = catsCount;
        }
    }

    [System.Serializable]
    public struct CatsCount
    {
        public int orangeCats;
        public int orangeChonkyCats;
        public int blackCats;
        public int blackChonkyCats;

        public CatsCount(
            int orangeCats = 0,
            int orangeChonkyCats = 0,
            int blackCats = 0,
            int blackChonkyCats = 0
        )
        {
            this.orangeCats = orangeCats;
            this.blackCats = blackCats;
            this.orangeChonkyCats = orangeChonkyCats;
            this.blackChonkyCats = blackChonkyCats;
        }
    }

    [System.Serializable]
    public struct MoveData
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
    public struct GameResult
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
    public struct MapHash
    {
        public string maphash;

        public MapHash(string maphash)
        {
            this.maphash = maphash;
        }
    }

    [System.Serializable]
    public struct StringData
    {
        public string data;

        public StringData(string data)
        {
            this.data = data;
        }
    }

    public class AckWaiter
    {
        public ClientServerMessage csm;
        public float timeStamp;

        public AckWaiter(ClientServerMessage csm, float timeStamp)
        {
            this.csm = csm;
            this.timeStamp = timeStamp;
        }
    }
}
