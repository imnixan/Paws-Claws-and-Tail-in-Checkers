using PJTC.Enums;
using PJTC.General;
using UnityEngine;

namespace PJTC.Structs
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
        public AttacksPool attacksPool;

        public PlayerInitData(
            int playerID,
            CatData[] gameField,
            CatsCount catsCount,
            AttacksPool attacksPool
        )
        {
            this.playerID = playerID;
            this.gameField = gameField;
            this.catsCount = catsCount;
            this.attacksPool = attacksPool;
        }
    }

    [System.Serializable]
    public struct PlayerAttackTypesData
    {
        public AttackTypeData[] attackTypes;

        public PlayerAttackTypesData(AttackTypeData[] attackTypes)
        {
            this.attackTypes = attackTypes;
        }
    }

    [System.Serializable]
    public struct AttackTypeData
    {
        public int catID;
        public int attackType;

        public AttackTypeData(int catID, int attackType)
        {
            this.catID = catID;
            this.attackType = attackType;
        }
    }

    [System.Serializable]
    public struct CatData
    {
        public int id;
        public Vector2Int position;
        public CatsType.Type type;
        public CatsType.Team team;
        public CatsType.Attack attackType;
        public AttackHint attackHints;

        public CatData(
            int catID,
            Vector2Int position,
            CatsType.Type type = CatsType.Type.None,
            CatsType.Team team = CatsType.Team.None,
            CatsType.Attack attackType = CatsType.Attack.None,
            AttackHint attackHints = new AttackHint()
        )
        {
            this.id = catID;
            this.position = position;
            this.type = type;
            this.team = team;
            this.attackType = attackType;
            this.attackHints = attackHints;
        }

        public void UpdateDefenderHints(bool defeated, CatsType.Attack attackerType)
        {
            CatsType.Attack newExcludedAttack = AttackMap.attackMap[attackerType];
            if (
                defeated
                || (
                    attackHints.excludedAttack != CatsType.Attack.None
                    && attackHints.excludedAttack != newExcludedAttack
                )
            )
            {
                attackHints.solved = true;
            }
            else
            {
                attackHints.excludedAttack = newExcludedAttack;
            }
        }

        public void UpdateAttackerHints(bool won)
        {
            CatsType.Attack newExcludedAttack = AttackMap.attackMap[this.attackType];
            if (
                won
                || (
                    attackHints.excludedAttack != CatsType.Attack.None
                    && attackHints.excludedAttack != newExcludedAttack
                )
            )
            {
                attackHints.solved = true;
            }
            else
            {
                attackHints.excludedAttack = newExcludedAttack;
            }
        }
    }

    [System.Serializable]
    public struct AttackHint
    {
        public bool solved;
        public CatsType.Attack excludedAttack;

        public AttackHint(
            CatsType.Attack excludedAttack = CatsType.Attack.None,
            bool solved = false
        )
        {
            this.solved = solved;
            this.excludedAttack = excludedAttack;
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

        public Moves(Vector2Int[] possibleMoves)
        {
            this.possibleMoves = possibleMoves;
        }
    }

    [System.Serializable]
    public struct MoveResult
    {
        public CompletedMoveData[] moves;
        public CatsCount catsCount;

        public MoveResult(CompletedMoveData[] moves, CatsCount catsCount)
        {
            this.moves = moves;
            this.catsCount = catsCount;
        }

        public MoveResult(MoveResult moveResult)
        {
            this.moves = moveResult.moves;
            this.catsCount = moveResult.catsCount;
        }
    }

    [System.Serializable]
    public struct BattleData { }

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

        public MoveData(CatData catData, Vector2Int moveEnd)
        {
            this.catData = catData;
            this.moveEnd = moveEnd;
        }
    }

    [System.Serializable]
    public struct CompletedMoveData
    {
        public MoveData moveData;
        public bool moveWithUpgrade;
        public bool moveWithBattle;
        public bool battleWin;
        public CatData enemy;

        public CompletedMoveData(
            MoveData moveData,
            bool moveWithUpgrade = false,
            bool moveWithBattle = false,
            bool battlelewin = false,
            CatData enemy = new CatData()
        )
        {
            this.moveData = moveData;
            this.moveWithUpgrade = moveWithUpgrade;
            this.moveWithBattle = moveWithBattle;
            this.battleWin = battlelewin;
            this.enemy = enemy;
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

    [System.Serializable]
    public struct AttacksPool
    {
        public int maxPaws;
        public int maxJaws;
        public int maxTails;

        public AttacksPool(int maxPaws, int maxJaws, int maxTails)
        {
            this.maxPaws = maxPaws;
            this.maxJaws = maxJaws;
            this.maxTails = maxTails;
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
