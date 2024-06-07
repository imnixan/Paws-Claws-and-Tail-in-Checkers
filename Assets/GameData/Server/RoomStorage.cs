using System;
using System.Collections.Concurrent;
using PCTC.Server;
using UnityEngine;

namespace PCTC.Server
{
    public static class RoomStorage
    {
        public static ConcurrentDictionary<Guid, PlayersCommunicator> rooms =
            new ConcurrentDictionary<Guid, PlayersCommunicator>();
    }
}
