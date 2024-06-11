using System;
using System.Collections.Concurrent;
using Unity.VisualScripting;
using UnityEngine;
using WebSocketSharp;

namespace PCTC.Server
{
    internal class GlobalMessageHandler
    {
        public static void OnMessage(MessageEventArgs e, Guid roomNumber, int playerID)
        {
            RoomStorage.rooms[roomNumber].playerDataHandler.ProcessUserData(e.Data, playerID);
        }

        public static void OnPlayerDisconnect(Guid roomNumber, int playerID)
        {
            RoomStorage.rooms[roomNumber].playerDataHandler.OnPlayerDisconnect(playerID);
        }
    }
}
