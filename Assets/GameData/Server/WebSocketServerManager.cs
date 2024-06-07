using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PCTC.Server
{
    public class WebSocketServerManager : MonoBehaviour
    {
        private WebSocketServer wss;

        void OnEnable()
        {
            wss = new WebSocketServer("ws://localhost:8080");
            wss.AddWebSocketService<PlayerListener>("/checkers");
            wss.Start();

            Debug.Log("WebSocket server started at ws://localhost:8080");
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
