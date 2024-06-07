using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PCTC.Server;
using UnityEngine;

namespace PCTC.Server
{
    public static class RoomCreator
    {
        private static int roomSize = 2;
        private static ConcurrentBag<PlayerListener> playersQueue =
            new ConcurrentBag<PlayerListener>();

        public static void OnPlayerConnect(PlayerListener listener)
        {
            playersQueue.Add(listener);
            TryBuildRoom();
        }

        private static void TryBuildRoom()
        {
            if (playersQueue.Count >= roomSize)
            {
                Guid roomId = Guid.NewGuid();
                PlayerListener[] playerListeners = new PlayerListener[roomSize];
                for (int i = 0; i < roomSize; i++)
                {
                    PlayerListener listener;
                    playersQueue.TryTake(out listener);
                    if (listener != null)
                    {
                        playerListeners[i] = listener;
                        listener.roomNumber = roomId;
                        listener.playerId = i;
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
                    new ServerGameManager(playersCommunicator);
                }
            }
        }

        public static void OnPlayerDisconnect(PlayerListener listener)
        {
            if (listener.roomNumber == null)
            {
                PlayerListener delListener;
                playersQueue.TryTake(out delListener);
            }
            RoomStorage.rooms.TryRemove(listener.roomNumber, out _);
        }
    }
}
