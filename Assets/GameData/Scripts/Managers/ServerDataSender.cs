using System;
using PCTC.Enums;
using PCTC.Server;
using PCTC.Structs;
using UnityEngine;
using WebSocketSharp;

namespace PCTC.Managers
{
    public class ServerDataSender
    {
        private WebSocket ws;
        private ClientGameManager gameManager;

        public ServerDataSender(WebSocket ws, ClientGameManager gameManager)
        {
            this.ws = ws;
            this.gameManager = gameManager;
        }

        public void SendPlayerChooseCat(CatData catData)
        {
            Debug.Log("send that i choose cat");
            RequestTypes.ClientRequests type = RequestTypes.ClientRequests.PLAYER_CHOOSED_CAT;
            string message = BuildMessage(type, catData);
            SendMessage(message);
        }

        public void SendPlayerReady(string mapHash)
        {
            RequestTypes.ClientRequests type = RequestTypes.ClientRequests.PLAYER_READY;
            string message = BuildMessage(type, new MapHash(mapHash));
            SendMessage(message);
        }

        public void SendPlayerMove(MoveData moveData)
        {
            RequestTypes.ClientRequests type = RequestTypes.ClientRequests.PLAYER_MOVE;
            string message = BuildMessage(type, moveData);
            SendMessage(message);
        }

        private void SendMessage(string message)
        {
            this.ws.Send(message);
        }

        private string BuildMessage<T>(RequestTypes.ClientRequests type, T body)
        {
            string data = JsonUtility.ToJson(body);
            ClientServerMessage csm = new ClientServerMessage((int)type, data);
            string message = JsonUtility.ToJson(csm);
            return message;
        }
    }
}
