using System.ComponentModel;
using PCTC.Enums;
using UnityEngine;

namespace PCTC.Structs
{
    [System.Serializable]
    public class ClientServerMessage
    {
        public int type;
        public string data;
        public int playerId;

        public ClientServerMessage(int type, int playerId, string data)
        {
            this.type = type;
            this.playerId = playerId;
            this.data = data;
        }

        public ClientServerMessage(int type, string data)
        {
            this.type = type;
            this.playerId = -1;
            this.data = data;
        }
    }

    [System.Serializable]
    public class PlayerInitData
    {
        public int playerId;
        public CatData[] gameField;

        public PlayerInitData(int playerId, CatData[] gameField)
        {
            this.playerId = playerId;
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
            int catId,
            Vector2Int position,
            CatsType.Type type = CatsType.Type.None,
            CatsType.Team team = CatsType.Team.None
        )
        {
            this.id = catId;
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

        public Moves(Vector2Int[] possibleMoves)
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

        public MoveResult(MoveData[] moves, CatData[] catsForRemove, CatData[] catsForUpdate)
        {
            this.moves = moves;
            this.catsForRemove = catsForRemove;
            this.catsForUpgrade = catsForUpdate;
        }

        public MoveResult(MoveData[] moves)
        {
            this.moves = moves;
            this.catsForUpgrade = new CatData[0];
            this.catsForRemove = new CatData[0];
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
}
