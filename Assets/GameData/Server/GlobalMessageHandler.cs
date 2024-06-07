using System;
using System.Collections.Concurrent;
using Unity.VisualScripting;
using UnityEngine;
using WebSocketSharp;

namespace PCTC.Server
{
    internal class GlobalMessageHandler
    {
        public static void OnMessage(MessageEventArgs e, Guid roomNumber)
        {
            RoomStorage.rooms[roomNumber].playerDataHandler.ProcessUserData(e.Data);
        }
    }
}
