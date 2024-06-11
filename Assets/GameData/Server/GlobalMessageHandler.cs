using System;
using System.Collections.Concurrent;
using PCTC.Structs;
using Unity.VisualScripting;
using UnityEngine;
using WebSocketSharp;

namespace PCTC.Server
{
    internal class GlobalMessageHandler
    {
        public static void OnMessage(ClientServerMessage csm, Guid roomNumber, int playerID)
        {
            RoomStorage.rooms[roomNumber].playerDataHandler.ProcessUserData(csm, playerID);
        }

        public static void OnPlayerDisconnect(Guid roomNumber, int playerID)
        {
            RoomStorage.rooms[roomNumber].playerDataHandler.OnPlayerDisconnect(playerID);
        }
    }
}
