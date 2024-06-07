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
        private Dictionary<RequestTypes.ClientRequests, Action<DataFromPlayer>> requestHandlers;
        private HashSet<RequestTypes.ClientRequests> requestsRequireActivePlayer;

        public PlayerDataHandler()
        {
            InitializeHandlers();
        }

        #region Initialization
        private void InitializeHandlers()
        {
            #region Handlers
            requestHandlers = new Dictionary<RequestTypes.ClientRequests, Action<DataFromPlayer>>
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

        public void ProcessUserData(string data, int playerId)
        {
            ClientServerMessage userMessage = JsonUtility.FromJson<ClientServerMessage>(data);
            DataFromPlayer dataFromPlayer = new DataFromPlayer(userMessage, playerId);

            InvokeHandler(dataFromPlayer);
        }

        private void InvokeHandler(DataFromPlayer dataFromPlayer)
        {
            RequestTypes.ClientRequests type = (RequestTypes.ClientRequests)
                dataFromPlayer.message.type;
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

        private void HandlePlayerMoveFinish(DataFromPlayer dft)
        {
            gameManager.OnPlayerMoveFinish(dft.playerID);
        }
    }
}
