using System;
using System.Collections.Concurrent;
using PJTC.Server;
using UnityEngine;

namespace PJTC.Server
{
    public static class RoomStorage
    {
        public static ConcurrentDictionary<Guid, PlayersCommunicator> rooms =
            new ConcurrentDictionary<Guid, PlayersCommunicator>();
    }
}
