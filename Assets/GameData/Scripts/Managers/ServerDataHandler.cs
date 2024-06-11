using System;
using System.Collections.Generic;
using PCTC.Enums;
using PCTC.Managers;
using PCTC.Structs;
using UnityEngine;
using WebSocketSharp;

namespace GameData.Managers
{
    public class ServerDataHandler
    {
        private WebSocket ws;
        private ClientGameManager gameManager;

        private Dictionary<
            RequestTypes.ServerRequests,
            Action<ClientServerMessage>
        > requestHandlers;

        public ServerDataHandler(WebSocket ws, ClientGameManager gameManager)
        {
            this.ws = ws;
            this.gameManager = gameManager;
            InitHandler();
        }

        #region Init
        private void InitHandler()
        {
            requestHandlers = new Dictionary<
                RequestTypes.ServerRequests,
                Action<ClientServerMessage>
            >
            {
                { RequestTypes.ServerRequests.PLAYER_INIT, OnPlayerInit },
                { RequestTypes.ServerRequests.SET_PLAYER_ORDER, OnChangeOrder },
                { RequestTypes.ServerRequests.POSSIBLE_MOVES, OnPossibleMovesCatch },
                { RequestTypes.ServerRequests.MOVE_RESULT, OnPlayerMoveResultCatch },
                { RequestTypes.ServerRequests.START_GAME, OnGameStart },
                { RequestTypes.ServerRequests.GAME_RESULT, OnGameEnd }
            };
        }
        #endregion

        public void OnGameEnd(ClientServerMessage message)
        {
            GameResult result = JsonUtility.FromJson<GameResult>(message.data);
            gameManager.OnGameEnd(result);
        }

        public void ProcessServerData(object sender, MessageEventArgs e)
        {
            ClientServerMessage serverMessage = JsonUtility.FromJson<ClientServerMessage>(e.Data);
            UnityMainThreadDispatcher.Instance.Enqueue(() => InvokeHandler(serverMessage));
        }

        private void InvokeHandler(ClientServerMessage message)
        {
            RequestTypes.ServerRequests type = (RequestTypes.ServerRequests)message.type;
            if (requestHandlers.TryGetValue(type, out Action<ClientServerMessage> handler))
            {
                handler(message);
            }
            else
            {
                // Обработка неизвестного типа запроса, если требуется
                Console.WriteLine("Unknown request type");
            }
        }

        private void OnGameStart(ClientServerMessage message)
        {
            gameManager.OnGameStart();
        }

        private void OnPlayerInit(ClientServerMessage message)
        {
            PlayerInitData initData = JsonUtility.FromJson<PlayerInitData>(message.data);

            gameManager.InitPlayer(initData);
        }

        private void OnChangeOrder(ClientServerMessage message)
        {
            PlayerOrder playerOrder = JsonUtility.FromJson<PlayerOrder>(message.data);

            gameManager.ChangePlayerOrder(playerOrder.playerOrder);
        }

        private void OnPossibleMovesCatch(ClientServerMessage message)
        {
            Moves moves = JsonUtility.FromJson<Moves>(message.data);

            gameManager.ProcessPossibleMoves(moves);
        }

        private void OnPlayerMoveResultCatch(ClientServerMessage message)
        {
            MoveResult moveResult = JsonUtility.FromJson<MoveResult>(message.data);

            gameManager.ProcessMoveResult(moveResult);
        }
    }
}
