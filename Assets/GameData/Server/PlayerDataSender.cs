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

        public int RemoveListener(int playerID)
        {
            playerListeners.RemoveAt(playerID);
            return playerListeners.Count;
        }

        public void InitPlayer(int playerID, CatData[,] gameField)
        {
            RequestTypes.ServerRequests type = RequestTypes.ServerRequests.PLAYER_INIT;
            CatData[] field = ArrayTransformer.Flatten(gameField);
            PlayerInitData initData = new PlayerInitData(playerID, field);
            string message = BuildMessage(type, initData);
            SendPlayerMessage(playerID, message);
        }

        public void SendAllGameEnd(GameResult gameResult)
        {
            RequestTypes.ServerRequests type = RequestTypes.ServerRequests.GAME_RESULT;
            string message = BuildMessage(type, gameResult);
            SendAllPlayers(message);
        }

        private string BuildMessage<T>(RequestTypes.ServerRequests type, T body)
        {
            string data = JsonUtility.ToJson(body);
            ClientServerMessage csm = new ClientServerMessage((int)type, data);
            string message = JsonUtility.ToJson(csm);
            return message;
        }

        public void SendAllCurrentPlayerNotification(int currentPlayer)
        {
            for (int playerID = 0; playerID < playerListeners.Count; playerID++)
            {
                RequestTypes.ServerRequests type = RequestTypes.ServerRequests.SET_PLAYER_ORDER;
                bool playersTurn = currentPlayer == playerID;
                PlayerOrder plyerOrder = new PlayerOrder(playersTurn);
                string message = BuildMessage(type, plyerOrder);
                SendPlayerMessage(playerID, message);
            }
        }

        public void SendPlayerPossibleMoves(int playerID, Moves moves)
        {
            RequestTypes.ServerRequests type = RequestTypes.ServerRequests.POSSIBLE_MOVES;

            string message = BuildMessage(type, moves);
            SendPlayerMessage(playerID, message);
        }

        public void SendAllPlayerMove(MoveResult moveResult)
        {
            RequestTypes.ServerRequests type = RequestTypes.ServerRequests.MOVE_RESULT;
            string message = BuildMessage(type, moveResult);
            SendAllPlayers(message);
        }

        public void SendAllGameStart()
        {
            RequestTypes.ServerRequests type = RequestTypes.ServerRequests.START_GAME;
            string message = BuildMessage(type, "Game Started!");
            SendAllPlayers(message);
        }

        private void SendPlayerMessage(int playerID, string message)
        {
            playerListeners[playerID].SendPlayerMessage(message);
        }

        private void SendAllPlayers(string message)
        {
            for (int i = 0; i < playerListeners.Count; i++)
            {
                SendPlayerMessage(i, message);
            }
        }
    }
}
