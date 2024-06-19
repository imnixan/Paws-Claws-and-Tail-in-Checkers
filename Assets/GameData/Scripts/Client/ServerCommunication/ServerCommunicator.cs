using System.Collections;
using System.Threading.Tasks;
using System.Timers;
using PJTC.Enums;
using PJTC.Managers;
using PJTC.Managers;
using PJTC.Structs;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

namespace PJTC.Scripts
{
    public class ServerCommunicator
    {
        public WebSocket ws { get; private set; }
        public ServerDataSender serverDataSender { get; private set; }
        public ServerDataHandler serverDataHandler { get; private set; }
        private const int PING_TIME = 1000;
        private const int MAX_CONNECT_TIME = 5000;
        private const int MAX_RETRIES = 5;
        private int retries;
        private string ip;
        private string port;
        private ClientGameManager gameManager;
        private int messageCount = 0;

        private Timer pingTimer;

        private Timer connectTimer;

        public struct userAttributes { }

        public struct appAttributes { }

        public ServerCommunicator(ClientGameManager gm)
        {
            this.ip = ip;
            this.port = port;
            this.gameManager = gm;
        }

        public async void TryToConnect()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("no internet");
                gameManager.OnError();
                return;
            }

            await InitializeRemoteConfigAsync();
            RemoteConfigService.Instance.FetchCompleted += ParseRemoteConfig;
            await RemoteConfigService.Instance.FetchConfigsAsync(
                new userAttributes(),
                new appAttributes()
            );
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            ClientServerMessage csm = JsonUtility.FromJson<ClientServerMessage>(e.Data);

            //SendAck(csm.messageID);

            UnityMainThreadDispatcher.Instance.Enqueue(() => HandleMessage(csm));
        }

        private void SendAck(int messageID)
        {
            CSMRequest.Type type = CSMRequest.Type.ACK;
            ClientServerMessage csm = new ClientServerMessage((int)type, "Ack");
            csm.messageID = messageID;
            string message = JsonUtility.ToJson(csm);

            Send(message);
        }

        public void Disconnect()
        {
            if (pingTimer != null)
                pingTimer.Stop();

            if (connectTimer != null)
                connectTimer.Stop();
            if (ws != null)
            {
                ws.Close();
            }
        }

        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck)
        {
            ClientServerMessage csm = BuildMessage(type, body);
            csm.messageID = messageCount;
            messageCount++;
            Debug.Log($"Client {gameManager.playerID} send message {csm.messageID}");
            string message = JsonUtility.ToJson(csm);
            Send(message);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Debug.Log("ws error");
            gameManager.OnError();
        }

        private void OnConnectError(object source, ElapsedEventArgs e)
        {
            Debug.Log("on connect error " + e.ToString());
            UnityMainThreadDispatcher.Instance.Enqueue(() => Disconnect());
            UnityMainThreadDispatcher.Instance.Enqueue(() => gameManager.OnConnectError());
        }

        private void OnConnected(object sender, System.EventArgs e)
        {
            Debug.Log("Connected");
            connectTimer.Stop();
            pingTimer = new Timer(1000);
            pingTimer.Elapsed += CheckAlive;
            pingTimer.AutoReset = true;

            pingTimer.Enabled = true;

            UnityMainThreadDispatcher.Instance.Enqueue(() => gameManager.OnConnect());
        }

        private void CheckAlive(object source, ElapsedEventArgs e)
        {
            if (ws.IsAlive)
            {
                retries = 0;
            }
            else
            {
                retries++;
            }
            if (retries >= MAX_RETRIES)
            {
                Disconnect();
            }
        }

        private void OnConnectionClosed(object sender, System.EventArgs e)
        {
            pingTimer.Stop();
            gameManager.OnServerEndConnection();
        }

        void OnDestroy()
        {
            Disconnect();
        }

        private void Send(string message)
        {
            ws.SendAsync(message, OnMessageSended);
        }

        private void OnMessageSended(bool succes) { }

        private void HandleMessage(ClientServerMessage csm)
        {
            serverDataHandler.ProcessServerData(csm);
        }

        private ClientServerMessage BuildMessage<T>(CSMRequest.Type type, T body)
        {
            string data = JsonUtility.ToJson(body);
            ClientServerMessage csm = new ClientServerMessage((int)type, data);

            return csm;
        }

        private async Task InitializeRemoteConfigAsync()
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private void ParseRemoteConfig(ConfigResponse configResponse)
        {
            this.ip = RemoteConfigService.Instance.appConfig.GetString("serverURL");
            this.port = RemoteConfigService.Instance.appConfig.GetString("serverPORT");

            Debug.Log($"connecting {ip}:{port}");
            ws = new WebSocket($"ws://{ip}:{port}/checkers");
            serverDataHandler = new ServerDataHandler(ws);
            serverDataSender = new ServerDataSender(ws, this);
            ws.OnMessage += OnMessage;
            ws.OnOpen += OnConnected;
            ws.OnError += OnError;
            ws.OnClose += OnConnectionClosed;
            ws.ConnectAsync();

            connectTimer = new Timer(MAX_CONNECT_TIME);
            connectTimer.Elapsed += OnConnectError;
            connectTimer.AutoReset = false;
            connectTimer.Enabled = true;
            connectTimer.Start();
        }
    }
}
