using UnityEngine;
using WebSocketSharp.Server;

namespace PJTC.Server
{
    public class WebSocketServerManager : MonoBehaviour
    {
        [SerializeField]
        private string ip;

        private WebSocketServer wss;

        void OnEnable()
        {
            wss = new WebSocketServer($"ws://{ip}:8080");
            wss.AddWebSocketService<PlayerListener>("/checkers");
            wss.Start();

            Debug.Log($"WebSocket server started at ws://{ip}:8080");
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
