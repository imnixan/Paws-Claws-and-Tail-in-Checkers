using System;
using PJTC.Enums;
using PJTC.Scripts;
using PJTC.Server;
using PJTC.Structs;
using UnityEngine;
using WebSocketSharp;

namespace PJTC.Managers
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
            Debug.Log($"chose cell {moveData.moveEnd}");
            SendMessage(type, moveData);
        }

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck = false)
        {
            gameManager.serverCommunicator.SendMessage(type, body, needAck);
        }
    }
}
