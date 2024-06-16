using System;
using System.Collections.Generic;
using PJTC.Server;
using UnityEngine;

namespace PJTC.Server
{
    public static class RoomCreator
    {
        private const int ROOM_SIZE = 2;
        private static SynchronizedCollection<PlayerListener> playersQueue =
            new SynchronizedCollection<PlayerListener>();

        public static void OnPlayerConnect(PlayerListener listener)
        {
            playersQueue.Add(listener);
            TryBuildRoom();
        }

        public static void OnPlayerDisconnect(PlayerListener listener)
        {
            playersQueue.Remove(listener);
        }

        private static void TryBuildRoom()
        {
            if (playersQueue.Count >= ROOM_SIZE)
            {
                Guid roomId = Guid.NewGuid();
                List<PlayerListener> playerListeners = new List<PlayerListener>();
                for (int i = 0; i < ROOM_SIZE; i++)
                {
                    PlayerListener listener = playersQueue[0];
                    playerListeners.Add(listener);
                    listener.roomNumber = roomId;
                    listener.playerID = i;
                    playersQueue.RemoveAt(0);
                }
                PlayerDataSender playerDataSender = new PlayerDataSender(playerListeners);
                PlayerDataHandler playerDataHandler = new PlayerDataHandler();
                PlayersCommunicator playersCommunicator = new PlayersCommunicator(
                    playerDataSender,
                    playerDataHandler
                );

                if (RoomStorage.rooms.TryAdd(roomId, playersCommunicator))
                {
                    new ServerGameManager(playersCommunicator, playerListeners.Count, roomId);
                }
            }
        }

        public static void DestroyRoom(Guid roomID)
        {
            Debug.Log("DestroyRoom");
            RoomStorage.rooms.TryRemove(roomID, out _);
        }
    }
}
