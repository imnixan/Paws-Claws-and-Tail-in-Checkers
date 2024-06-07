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
            string message = JsonUtility.ToJson(catData);
            SendMessage(type, message);
        }

        public void SendPlayerFinishedMove()
        {
            RequestTypes.ClientRequests type = RequestTypes.ClientRequests.PlAYER_MOVE_FINISH;
            string message = "";
            SendMessage(type, message);
        }

        public void SendPlayerMove(MoveData moveData)
        {
            RequestTypes.ClientRequests type = RequestTypes.ClientRequests.PLAYER_MOVE;
            string message = JsonUtility.ToJson(moveData);
            SendMessage(type, message);
        }

        private void SendMessage(RequestTypes.ClientRequests type, string message)
        {
            ClientServerMessage csm = new ClientServerMessage((int)type, message);
            string data = JsonUtility.ToJson(csm);

            this.ws.Send(data);
        }
    }
}
