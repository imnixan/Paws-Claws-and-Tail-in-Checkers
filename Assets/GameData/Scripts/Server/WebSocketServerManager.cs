using System;
using UnityEngine;
using WebSocketSharp.Server;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace PJTC.Server
{
    public class WebSocketServerManager : MonoBehaviour
    {
        [SerializeField]
        private bool production;

        private WebSocketServer wss;

       
        void OnEnable()
        {
            wss = new WebSocketServer(System.Net.IPAddress.Any, 8080, true);
            try
            {
                wss.SslConfiguration.ServerCertificate = new X509Certificate2(
                    "/etc/letsencrypt/live/pctc.wowandy.dev/certificate.pfx",
                    "1337",
                    X509KeyStorageFlags.DefaultKeySet
                );
                Debug.Log("Certificate loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load certificate: {ex.Message}\n{ex.StackTrace}");
            }
            wss.AddWebSocketService<PlayerListener>("/checkers"); // Подключение по wss://pctc.wowandy.dev:8081/ws
            wss.Start();
            Debug.Log($"{wss.Address} is listening on port {wss.Port}");
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
