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
        private PlayerListener[] playerListeners;

        public PlayerDataSender(PlayerListener[] listeners)
        {
            this.playerListeners = listeners;
        }

        public void InitUsers(CatData[,] gameField)
        {
            RequestTypes.ServerRequests type = RequestTypes.ServerRequests.PLAYER_INIT;
            CatData[] field = ArrayTransformer.Flatten(gameField);
            for (int i = 0; i < 2; i++)
            {
                PlayerInitData initData = new PlayerInitData(i, field);
                string data = JsonUtility.ToJson(initData);
                ClientServerMessage clientServerMessage = new ClientServerMessage((int)type, data);
                string message = JsonUtility.ToJson(clientServerMessage);
                SendPlayerMessage(i, message);
            }
        }

        public void ChangePlayerMoveOrder(bool firstPlayerMove)
        {
            for (int i = 0; i < 2; i++)
            {
                RequestTypes.ServerRequests type = RequestTypes.ServerRequests.SET_PLAYER_ORDER;
                string data = JsonUtility.ToJson(new PlayerOrder(firstPlayerMove));
                ClientServerMessage csm = new ClientServerMessage((int)type, data);
                string message = JsonUtility.ToJson(csm);
                SendPlayerMessage(i, message);
                firstPlayerMove = !firstPlayerMove;
            }
        }

        public void SendPlayerPossibleMoves(int playerId, Moves moves)
        {
            RequestTypes.ServerRequests type = RequestTypes.ServerRequests.POSSIBLE_MOVES;
            string data = JsonUtility.ToJson(moves);
            ClientServerMessage csm = new ClientServerMessage((int)type, data);
            string message = JsonUtility.ToJson(csm);
            SendPlayerMessage(playerId, message);
        }

        public void SendPlayerMove(MoveResult moveResult)
        {
            RequestTypes.ServerRequests type = RequestTypes.ServerRequests.MOVE_RESULT;
            string data = JsonUtility.ToJson(moveResult);
            ClientServerMessage csm = new ClientServerMessage((int)type, data);
            string message = JsonUtility.ToJson(csm);
            for (int i = 0; i < playerListeners.Length; i++)
            {
                SendPlayerMessage(i, message);
            }
        }

        private void SendPlayerMessage(int playerId, string message)
        {
            playerListeners[playerId].SendPlayerMessage(message);
        }
    }
}
