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
        private Dictionary<CSMRequest.Type, Action<ClientServerMessage>> requestHandlers;

        public ServerDataHandler(WebSocket ws, ClientGameManager gameManager)
        {
            this.ws = ws;
            this.gameManager = gameManager;
            InitHandler();
        }

        #region Init
        private void InitHandler()
        {
            requestHandlers = new Dictionary<CSMRequest.Type, Action<ClientServerMessage>>
            {
                { CSMRequest.Type.PLAYER_INIT, OnPlayerInit },
                { CSMRequest.Type.SET_PLAYER_ORDER, OnChangeOrder },
                { CSMRequest.Type.POSSIBLE_MOVES, OnPossibleMovesCatch },
                { CSMRequest.Type.PROCESS_MOVE, OnPlayerMoveResultCatch },
                { CSMRequest.Type.GAME_START, OnGameStart },
                { CSMRequest.Type.GAME_END, OnGameEnd }
            };
        }
        #endregion

        public void OnGameEnd(ClientServerMessage message)
        {
            Debug.Log("GAMEEND");
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
            CSMRequest.Type type = (CSMRequest.Type)message.type;
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
