using Unity.VisualScripting;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PJTC.Server
{
    public class WebSocketServerManager : MonoBehaviour
    {
        private WebSocketServer wss;

#if UNITY_EDITOR
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
#endif
    }
}
