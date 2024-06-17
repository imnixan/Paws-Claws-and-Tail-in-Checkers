using GameData.Scripts;
using PJTC.Enums;
using PJTC.Structs;
using UnityEngine;
using WebSocketSharp;

namespace PJTC.Managers
{
    public class ServerDataSender
    {
        private WebSocket ws;
        private ServerCommunicator serverCommunicator;

        public ServerDataSender(WebSocket ws, ServerCommunicator serverCommunicator)
        {
            this.ws = ws;
            this.serverCommunicator = serverCommunicator;
        }

        public void SendPlayerChooseCat(CatData catData)
        {
            CSMRequest.Type type = CSMRequest.Type.POSSIBLE_MOVES;

            SendMessage(type, catData);
        }

        public void SendPlayerReady(string mapHash)
        {
            CSMRequest.Type type = CSMRequest.Type.PLAYER_READY;

            SendMessage(type, new MapHash(mapHash), true);
        }

        public void SendPlayerMove(MoveData moveData)
        {
            CSMRequest.Type type = CSMRequest.Type.MAKE_MOVE;

            SendMessage(type, moveData);
        }

        public void SendPlayerAttack(PlayerAttackTypesData playerAttackTypesData)
        {
            CSMRequest.Type type = CSMRequest.Type.SET_ATTACK;

            SendMessage(type, playerAttackTypesData, true);
        }

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck = false)
        {
            serverCommunicator.SendMessage(type, body, needAck);
        }
    }
}
