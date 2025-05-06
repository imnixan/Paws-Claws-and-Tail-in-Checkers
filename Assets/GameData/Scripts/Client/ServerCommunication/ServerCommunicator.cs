using System.Collections;
using System.Text;
using System.Threading.Tasks;
using PJTC.Enums;
using PJTC.Managers;
using PJTC.Structs;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.Networking;
using HybridWebSocket;

namespace PJTC.Scripts
{
    public class ServerCommunicator
    {
        public WebSocket ws { get; private set; }
        public ServerDataSender serverDataSender { get; private set; }
        public ServerDataHandler serverDataHandler { get; private set; }

        private const float PING_INTERVAL = 1.0f;
        private const float MAX_CONNECT_TIME = 5.0f;
        private const int MAX_RETRIES = 5;

        private int retries = 0;
        private string ip;
        private string port;
        private ClientGameManager gameManager;
        private int messageCount = 0;

        private MonoBehaviour coroutineHost;

        private Coroutine pingCoroutine;
        private Coroutine connectTimeoutCoroutine;

        public struct userAttributes { }
        public struct appAttributes { }

        private bool IsAlive => ws != null && ws.GetState() == WebSocketState.Open;

        public ServerCommunicator(ClientGameManager gm, MonoBehaviour coroutineHost)
        {
            this.gameManager = gm;
            this.coroutineHost = coroutineHost;
        }

        public async void TryToConnect()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("No internet");
                gameManager.OnError();
                return;
            }

            await InitializeRemoteConfigAsync();
            RemoteConfigService.Instance.FetchCompleted += ParseRemoteConfig;
            await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
        }

        private void ParseRemoteConfig(ConfigResponse configResponse)
        {
            RemoteConfigService.Instance.FetchCompleted -= ParseRemoteConfig;

            ip = RemoteConfigService.Instance.appConfig.GetString("serverURL");
            port = RemoteConfigService.Instance.appConfig.GetString("serverPORT");

            string finalAddress = $"wss://{ip}:{port}/checkers";
            Debug.Log($"Connecting to {finalAddress}");
            ws = WebSocketFactory.CreateInstance(finalAddress);
            serverDataHandler = new ServerDataHandler(ws);
            serverDataSender = new ServerDataSender(ws, this);

            ws.OnMessage += OnMessage;

            // Все обработчики заворачиваем в try/catch
            ws.OnOpen += () =>
            {
                try
                {
                    OnConnected();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Exception in OnOpen: " + e.Message);
                    OnError(e.Message);
                }
            };

            ws.OnError += OnError;
            ws.OnClose += OnConnectionClosed;

            ConnectWebSocket();
        }

        private void ConnectWebSocket()
        {
            connectTimeoutCoroutine = coroutineHost.StartCoroutine(ConnectTimeout());
            ws.Connect();
        }

        private IEnumerator ConnectTimeout()
        {
            yield return new WaitForSeconds(MAX_CONNECT_TIME);
            if (!IsAlive)
            {
                Debug.Log("Connection timeout");
                Disconnect();
                gameManager.OnConnectError();
            }
        }

        private void OnConnected()
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                Debug.Log("Connected");

                if (connectTimeoutCoroutine != null)
                    coroutineHost.StopCoroutine(connectTimeoutCoroutine);

                pingCoroutine = coroutineHost.StartCoroutine(PingRoutine());
                gameManager.OnConnect();
            });
        }

        private IEnumerator PingRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(PING_INTERVAL);

                if (IsAlive)
                {
                    retries = 0;
                }
                else
                {
                    retries++;
                    Debug.LogWarning($"Ping failed ({retries})");
                }

                if (retries >= MAX_RETRIES)
                {
                    Debug.LogError("Ping failed too many times. Disconnecting.");
                    Disconnect();
                    break;
                }
            }
        }

        private void OnMessage(byte[] msg)
        {
            // ws.DispatchMessageQueue();
#if UNITY_WEBGL && !UNITY_EDITOR
#endif
            try
            {
                Debug.Log($"Message bytes length '{msg.Length}'" );
                string data = Encoding.UTF8.GetString(msg);
                Debug.Log($"MessageText  '{data}'" );
                ClientServerMessage csm = JsonUtility.FromJson<ClientServerMessage>(data);
                Debug.Log($"Message csm data  '{csm.data}'" );
                UnityMainThreadDispatcher.Instance.Enqueue(() => HandleMessage(csm));
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error processing message: " + e.Message);
            }
        }

        private void HandleMessage(ClientServerMessage csm)
        {
            serverDataHandler.ProcessServerData(csm);
        }

        private void OnError(string errMsg)
        {
            Debug.LogError("WebSocket error: " + errMsg);
            Disconnect();
            gameManager.OnError();
        }

        private void OnConnectionClosed(WebSocketCloseCode code)
        {
            Debug.Log($"Connection closed with code: {code}");

            if (pingCoroutine != null)
                coroutineHost.StopCoroutine(pingCoroutine);

            gameManager.OnServerEndConnection();
        }

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck)
        {
            ClientServerMessage csm = BuildMessage(type, body);
            csm.messageID = messageCount++;
            Debug.Log($"Client {gameManager.playerID} sending message {csm.messageID}");
            string message = JsonUtility.ToJson(csm);
            Send(message);
        }

        private void Send(string message)
        {
            if (IsAlive)
            {
                ws.Send(Encoding.UTF8.GetBytes(message));
            }
        }

        public void Disconnect()
        {
            if (pingCoroutine != null)
                coroutineHost.StopCoroutine(pingCoroutine);

            if (connectTimeoutCoroutine != null)
                coroutineHost.StopCoroutine(connectTimeoutCoroutine);

            if (ws != null)
            {
                ws.Close();
            }
        }

        private ClientServerMessage BuildMessage<T>(CSMRequest.Type type, T body)
        {
            string data = JsonUtility.ToJson(body);
            return new ClientServerMessage((int)type, data);
        }

        private async Task InitializeRemoteConfigAsync()
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
    }
}
