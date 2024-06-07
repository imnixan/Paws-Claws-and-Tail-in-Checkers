using System;
using System.Collections.Generic;
using PCTC.Enums;
using PCTC.Scripts;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Server
{
    public class PlayerDataHandler
    {
        private ServerGameManager gameManager;
        private Dictionary<
            RequestTypes.ClientRequests,
            Action<ClientServerMessage>
        > requestHandlers;
        private HashSet<RequestTypes.ClientRequests> requestsRequireActivePlayer;

        public PlayerDataHandler()
        {
            InitializeHandlers();
        }

        #region Initialization
        private void InitializeHandlers()
        {
            #region Handlers
            requestHandlers = new Dictionary<
                RequestTypes.ClientRequests,
                Action<ClientServerMessage>
            >
            {
                { RequestTypes.ClientRequests.PLAYER_CHOOSED_CAT, HandlePlayerChoosedCat },
                { RequestTypes.ClientRequests.PLAYER_MOVE, HandlePlayerMove },
                { RequestTypes.ClientRequests.PlAYER_MOVE_FINISH, HandlePlayerMoveFinish }
            };
            #endregion
            #region ActivePlayerRequests
            requestsRequireActivePlayer = new HashSet<RequestTypes.ClientRequests>
            {
                RequestTypes.ClientRequests.PLAYER_CHOOSED_CAT,
                RequestTypes.ClientRequests.PLAYER_MOVE,
            };
            #endregion
        }
        #endregion

        public void SetGameManager(ServerGameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void ProcessUserData(string data)
        {
            ClientServerMessage userMessage = JsonUtility.FromJson<ClientServerMessage>(data);

            InvokeHandler(userMessage);
        }

        private void InvokeHandler(ClientServerMessage message)
        {
            RequestTypes.ClientRequests type = (RequestTypes.ClientRequests)message.type;
            int playerId = message.playerId;

            if (requestHandlers.TryGetValue(type, out Action<ClientServerMessage> handler))
            {
                if (
                    requestsRequireActivePlayer.Contains(type)
                    && !gameManager.IsCurrentPlayer(playerId)
                )
                {
                    Console.WriteLine("Player is not active");
                    return;
                }

                handler(message);
            }
            else
            {
                // Обработка неизвестного типа запроса, если требуется
                Console.WriteLine("Unknown request type");
            }
        }

        private void HandlePlayerChoosedCat(ClientServerMessage message)
        {
            CatData catData = JsonUtility.FromJson<CatData>(message.data);
            gameManager.OnPlayerCatSelect(catData);
        }

        private void HandlePlayerMove(ClientServerMessage message)
        {
            MoveData moveData = JsonUtility.FromJson<MoveData>(message.data);
            gameManager.OnPlayerMove(moveData);
        }

        private void HandlePlayerMoveFinish(ClientServerMessage message)
        {
            gameManager.OnPlayerMoveFinish(message.playerId);
        }
    }
}
