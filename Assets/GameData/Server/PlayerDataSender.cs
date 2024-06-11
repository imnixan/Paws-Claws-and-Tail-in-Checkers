using System;
using System.Collections.Generic;
using PCTC.Enums;
using PCTC.Scripts;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Server
{
    public class PlayerDataSender
    {
        private List<PlayerListener> playerListeners;

        public PlayerDataSender(List<PlayerListener> listeners)
        {
            this.playerListeners = listeners;
        }

        public int GetActiveListenersCount(int playerID)
        {
            int activeListenersCount = 0;
            foreach (PlayerListener listener in playerListeners)
            {
                if (listener.active)
                {
                    activeListenersCount++;
                }
            }
            return playerListeners.Count;
        }

        public void InitPlayer(int playerID, CatData[,] gameField)
        {
            CSMRequest.Type type = CSMRequest.Type.PLAYER_INIT;
            CatData[] field = ArrayTransformer.Flatten(gameField);
            PlayerInitData initData = new PlayerInitData(playerID, field);

            SendMessage(playerID, type, initData);
        }

        public void SendAllGameEnd(GameResult gameResult)
        {
            CSMRequest.Type type = CSMRequest.Type.GAME_END;
            SendAllPlayers(type, gameResult);
        }

        public void SendAllCurrentPlayerNotification(int currentPlayer)
        {
            for (int playerID = 0; playerID < playerListeners.Count; playerID++)
            {
                CSMRequest.Type type = CSMRequest.Type.SET_PLAYER_ORDER;
                bool playersTurn = currentPlayer == playerID;
                PlayerOrder plyerOrder = new PlayerOrder(playersTurn);

                SendMessage(playerID, type, plyerOrder);
            }
        }

        public void SendPlayerPossibleMoves(int playerID, Moves moves)
        {
            CSMRequest.Type type = CSMRequest.Type.POSSIBLE_MOVES;

            SendMessage(playerID, type, moves);
        }

        public void SendAllPlayerMove(MoveResult moveResult)
        {
            CSMRequest.Type type = CSMRequest.Type.PROCESS_MOVE;
            SendAllPlayers(type, moveResult);
        }

        public void SendAllGameStart()
        {
            CSMRequest.Type type = CSMRequest.Type.GAME_START;

            SendAllPlayers(type, "Game Started!");
        }

        public void SendMessage<T>(int playerID, CSMRequest.Type type, T body, bool needAck = true)
        {
            playerListeners[playerID].SendMessage(type, body, needAck);
        }

        private void SendAllPlayers<T>(CSMRequest.Type type, T body, bool needAck = true)
        {
            for (int i = 0; i < playerListeners.Count; i++)
            {
                SendMessage(i, type, body, needAck);
            }
        }
    }
}
