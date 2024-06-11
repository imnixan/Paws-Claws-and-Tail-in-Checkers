using System;
using PCTC.Enums;
using PCTC.Server;
using PCTC.Structs;
using PTCP.Scripts;
using UnityEngine;
using WebSocketSharp;

namespace PCTC.Managers
{
    public class ServerDataSender : MessageSender
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
            CSMRequest.Type type = CSMRequest.Type.POSSIBLE_MOVES;
            SendMessage(type, catData);
        }

        public void SendPlayerReady(string mapHash)
        {
            CSMRequest.Type type = CSMRequest.Type.PLAYER_READY;
            SendMessage(type, new MapHash(mapHash));
        }

        public void SendPlayerMove(MoveData moveData)
        {
            CSMRequest.Type type = CSMRequest.Type.MAKE_MOVE;
            SendMessage(type, moveData);
        }

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck = false)
        {
            gameManager.serverCommunicator.SendMessage(type, body, needAck);
        }
    }
}
