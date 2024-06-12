using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PCTC.Server;
using UnityEngine;

namespace PCTC.Server
{
    public static class RoomCreator
    {
        private const int ROOM_SIZE = 2;
        private static ConcurrentBag<PlayerListener> playersQueue =
            new ConcurrentBag<PlayerListener>();

        public static void OnPlayerConnect(PlayerListener listener)
        {
            playersQueue.Add(listener);
            TryBuildRoom();
        }

        private static void TryBuildRoom()
        {
            if (playersQueue.Count >= ROOM_SIZE)
            {
                Guid roomId = Guid.NewGuid();
                List<PlayerListener> playerListeners = new List<PlayerListener>();
                for (int i = 0; i < ROOM_SIZE; i++)
                {
                    PlayerListener listener;
                    playersQueue.TryTake(out listener);
                    if (listener != null)
                    {
                        playerListeners.Add(listener);
                        listener.roomNumber = roomId;
                        listener.playerID = i;
                    }
                    else
                    {
                        return;
                    }
                }
                PlayerDataSender playerDataSender = new PlayerDataSender(playerListeners);
                PlayerDataHandler playerDataHandler = new PlayerDataHandler();
                PlayersCommunicator playersCommunicator = new PlayersCommunicator(
                    playerDataSender,
                    playerDataHandler
                );

                if (RoomStorage.rooms.TryAdd(roomId, playersCommunicator))
                {
                    new ServerGameManager(playersCommunicator, playerListeners.Count);
                }
            }
        }
    }
}
