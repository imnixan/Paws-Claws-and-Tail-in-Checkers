using System;
using System.Collections.Concurrent;

namespace PJTC.Server
{
    public static class RoomStorage
    {
        public static ConcurrentDictionary<Guid, PlayersCommunicator> rooms =
            new ConcurrentDictionary<Guid, PlayersCommunicator>();
    }
}
