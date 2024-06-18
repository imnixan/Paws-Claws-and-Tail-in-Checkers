using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            Debug.Log("add player in queue ");
            playersQueue.Add(listener);
            Debug.Log("Queue lenght " + playersQueue.Count);
            TryBuildRoom();
        }

        public static void OnPlayerDisconnect(PlayerListener listener)
        {
            Debug.Log("remove player from queue coz disconnect");
            playersQueue.Remove(listener);
            Debug.Log("Queue lenght " + playersQueue.Count);
        }

        public static void DestroyRoom(Guid roomID)
        {
            Debug.Log("DestroyRoom");
            RoomStorage.rooms.TryRemove(roomID, out _);
            Debug.Log($"TOTAL ROOMS {RoomStorage.rooms.Count}");
        }

        private static void TryBuildRoom()
        {
            Debug.Log("Trying build room");

            if (playersQueue.Count >= ROOM_SIZE)
            {
                Guid roomId = Guid.NewGuid();
                List<PlayerListener> playerListeners = new List<PlayerListener>();
                for (int i = 0; i < ROOM_SIZE; i++)
                {
                    if (playersQueue.Count == 0)
                    {
                        return;
                    }
                    PlayerListener listener = playersQueue[0];
                    if (listener.isConnected)
                    {
                        playerListeners.Add(listener);
                        listener.roomNumber = roomId;
                        listener.playerID = i;
                        playersQueue.RemoveAt(0);
                    }
                    else
                    {
                        Debug.Log("remove player from queue coz of not connected");
                        listener.CloseListener();
                        playersQueue.RemoveAt(0);
                        i--;
                    }
                }

                if (playerListeners.Count == ROOM_SIZE)
                {
                    PlayerDataSender playerDataSender = new PlayerDataSender(playerListeners);
                    PlayerDataHandler playerDataHandler = new PlayerDataHandler();
                    PlayersCommunicator playersCommunicator = new PlayersCommunicator(
                        playerDataSender,
                        playerDataHandler
                    );

                    if (RoomStorage.rooms.TryAdd(roomId, playersCommunicator))
                    {
                        Debug.Log($"TOTAL ROOMS {RoomStorage.rooms.Count}");
                        new ServerGameManager(playersCommunicator, playerListeners.Count, roomId);
                    }
                }
                else
                {
                    // Если не удалось собрать полную комнату, возвращаем оставшихся игроков в очередь
                    playersQueue.AddRange(playerListeners);
                    Debug.Log("Not enough connected players to build a full room");
                }
            }
            else
            {
                Debug.Log("Not enough players for room");
            }
        }
    }
}
