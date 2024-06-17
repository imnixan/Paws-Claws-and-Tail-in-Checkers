using UnityEngine;
using WebSocketSharp.Server;
using System.Net;

namespace PJTC.Server
{
    public class WebSocketServerManager : MonoBehaviour
    {
        [SerializeField]
        private bool production;

        private WebSocketServer wss;

        void OnEnable()
        {
            wss = production
                ? new WebSocketServer(IPAddress.IPv6Any, 8080)
                : new WebSocketServer($"ws://0.0.0.0:8080");

            wss.AddWebSocketService<PlayerListener>("/checkers");
            wss.Start();

            // Возможно
            Debug.Log($"WebSocket server started at ws://IPAddress.IPv6Any:8080");
        }

        void OnDestroy()
        {
            if (wss != null)
            {
                wss.Stop();
            }
        }
    }
}
