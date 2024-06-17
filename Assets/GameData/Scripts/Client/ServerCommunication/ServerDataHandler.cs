using System;
using System.Collections.Generic;
using PJTC.Enums;
using PJTC.Structs;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

namespace GameData.Managers
{
    public class ServerDataHandler
    {
        public static event UnityAction<GameResult> GameEnd;
        public static event UnityAction GameStart;
        public static event UnityAction<PlayerInitData> PlayerInit;
        public static event UnityAction<bool> MoverOrderChanging;
        public static event UnityAction<Moves> GotPossibleMoves;
        public static event UnityAction<MoveResult> Move;
        public static event UnityAction<PlayerInitData> PlayerSync;
        public static event UnityAction DrawAlarm;

        private WebSocket ws;
        private Dictionary<CSMRequest.Type, Action<ClientServerMessage>> requestHandlers;

        public ServerDataHandler(WebSocket ws)
        {
            this.ws = ws;

            InitHandler();
        }

        #region Init
        private void InitHandler()
        {
            requestHandlers = new Dictionary<CSMRequest.Type, Action<ClientServerMessage>>
            {
                { CSMRequest.Type.GAME_START, OnGameStart },
                { CSMRequest.Type.PLAYER_INIT, OnPlayerInit },
                { CSMRequest.Type.SET_PLAYER_ORDER, OnChangeOrder },
                { CSMRequest.Type.POSSIBLE_MOVES, OnPossibleMovesCatch },
                { CSMRequest.Type.MAKE_MOVE, OnPlayerMoveResultCatch },
                { CSMRequest.Type.PLAYER_SYNC, OnPlayerSync },
                { CSMRequest.Type.DRAW_ALARM, OnDrawAlarm },
                { CSMRequest.Type.GAME_END, OnGameEnd }
            };
        }
        #endregion



        public void ProcessServerData(ClientServerMessage serverMessage)
        {
            InvokeHandler(serverMessage);
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
            GameStart?.Invoke();
        }

        private void OnPlayerInit(ClientServerMessage message)
        {
            PlayerInitData initData = JsonUtility.FromJson<PlayerInitData>(message.data);

            PlayerInit?.Invoke(initData);
        }

        private void OnChangeOrder(ClientServerMessage message)
        {
            PlayerOrder playerOrder = JsonUtility.FromJson<PlayerOrder>(message.data);

            MoverOrderChanging?.Invoke(playerOrder.playerOrder);
        }

        private void OnPossibleMovesCatch(ClientServerMessage message)
        {
            Moves moves = JsonUtility.FromJson<Moves>(message.data);

            GotPossibleMoves?.Invoke(moves);
        }

        private void OnPlayerMoveResultCatch(ClientServerMessage message)
        {
            MoveResult moveResult = JsonUtility.FromJson<MoveResult>(message.data);

            Move?.Invoke(moveResult);
        }

        private void OnPlayerSync(ClientServerMessage message)
        {
            PlayerInitData initData = JsonUtility.FromJson<PlayerInitData>(message.data);

            PlayerSync?.Invoke(initData);
        }

        private void OnDrawAlarm(ClientServerMessage message)
        {
            DrawAlarm?.Invoke();
        }

        private void OnGameEnd(ClientServerMessage message)
        {
            Debug.Log("GAME END");
            GameResult result = JsonUtility.FromJson<GameResult>(message.data);

            GameEnd?.Invoke(result);
        }
    }
}
