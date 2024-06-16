using System;
using System.Collections.Generic;
using System.Drawing.Text;
using PJTC.Enums;
using PJTC.Managers;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Server
{
    public class PlayerDataHandler
    {
        private ServerGameManager gameManager;
        private Dictionary<CSMRequest.Type, Action<DataFromPlayer>> requestHandlers;
        private HashSet<CSMRequest.Type> requestsRequireActivePlayer;

        public PlayerDataHandler()
        {
            InitializeHandlers();
        }

        public void OnPlayerDisconnect(int playerID)
        {
            gameManager.OnPlayerDisconnect(playerID);
        }

        #region Initialization
        private void InitializeHandlers()
        {
            #region Handlers
            requestHandlers = new Dictionary<CSMRequest.Type, Action<DataFromPlayer>>
            {
                { CSMRequest.Type.SET_ATTACK, HandlePlayerAttack },
                { CSMRequest.Type.POSSIBLE_MOVES, HandlePlayerChoosedCat },
                { CSMRequest.Type.MAKE_MOVE, HandlePlayerMove },
                { CSMRequest.Type.PLAYER_READY, HandlePlayerReady }
            };
            #endregion
            #region ActivePlayerRequests
            requestsRequireActivePlayer = new HashSet<CSMRequest.Type>
            {
                CSMRequest.Type.POSSIBLE_MOVES,
                CSMRequest.Type.MAKE_MOVE,
            };
            #endregion
        }
        #endregion

        public void SetGameManager(ServerGameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void ProcessUserData(ClientServerMessage scm, int playerID)
        {
            DataFromPlayer dataFromPlayer = new DataFromPlayer(scm, playerID);

            InvokeHandler(dataFromPlayer);
        }

        private void InvokeHandler(DataFromPlayer dataFromPlayer)
        {
            CSMRequest.Type type = (CSMRequest.Type)dataFromPlayer.message.type;
            if (requestHandlers.TryGetValue(type, out Action<DataFromPlayer> handler))
            {
                if (
                    requestsRequireActivePlayer.Contains(type)
                    && !gameManager.IsCurrentPlayer(dataFromPlayer.playerID)
                )
                {
                    Console.WriteLine("Player is not active");
                    return;
                }

                handler(dataFromPlayer);
            }
            else
            {
                // Обработка неизвестного типа запроса, если требуется
                Console.WriteLine("Unknown request type");
            }
        }

        private void HandlePlayerChoosedCat(DataFromPlayer dft)
        {
            CatData catData = JsonUtility.FromJson<CatData>(dft.message.data);
            gameManager.OnPlayerCatSelect(catData);
        }

        private void HandlePlayerMove(DataFromPlayer dft)
        {
            MoveData moveData = JsonUtility.FromJson<MoveData>(dft.message.data);
            gameManager.OnPlayerMove(moveData);
        }

        private void HandlePlayerReady(DataFromPlayer dft)
        {
            MapHash playerMap = JsonUtility.FromJson<MapHash>(dft.message.data);
            gameManager.OnPlayerReady(playerMap, dft.playerID);
        }

        private void HandlePlayerAttack(DataFromPlayer dft)
        {
            PlayerAttackTypesData playerAttacks = JsonUtility.FromJson<PlayerAttackTypesData>(
                dft.message.data
            );
            gameManager.OnPlayerSetAttack(playerAttacks, dft.playerID);
        }
    }
}
