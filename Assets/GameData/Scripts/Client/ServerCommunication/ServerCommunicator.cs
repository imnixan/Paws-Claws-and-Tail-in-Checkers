using System.Collections;
using System.Timers;
using GameData.Managers;
using PJTC.Enums;
using PJTC.Managers;
using PJTC.Structs;
using UnityEngine;
using WebSocketSharp;

namespace GameData.Scripts
{
    public class ServerCommunicator
    {
        public WebSocket ws { get; private set; }
        public ServerDataSender serverDataSender { get; private set; }
        public ServerDataHandler serverDataHandler { get; private set; }
        private const int PING_TIME = 1000;
        private string ip;
        private string port;
        private ClientGameManager gameManager;
        private int messageCount = 0;

        private Timer timer;

        public ServerCommunicator(
            ClientGameManager gm,
            string ip = "localhost",
            string port = "8080"
        )
        {
            this.ip = ip;
            this.port = port;
            this.gameManager = gm;
        }

        public void ConnectToServer()
        {
            ws = new WebSocket($"ws://{ip}:{port}/checkers");
            serverDataHandler = new ServerDataHandler(ws);
            serverDataSender = new ServerDataSender(ws, this);
            ws.OnMessage += OnMessage;
            ws.OnOpen += OnConnected;
            ws.OnError += OnError;
            ws.OnClose += OnConnectionClosed;
            ws.ConnectAsync();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            ClientServerMessage csm = JsonUtility.FromJson<ClientServerMessage>(e.Data);

            SendAck(csm.messageID);

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
            if (ws != null)
            {
                ws.Close();
            }
            else
            {
                gameManager.RestartScene();
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
            gameManager.OnError();
        }

        private void OnConnected(object sender, System.EventArgs e)
        {
            timer = new Timer(1000);

            timer.Elapsed += OnTimerEvent;

            timer.AutoReset = true;

            timer.Enabled = true;

            UnityMainThreadDispatcher.Instance.Enqueue(() => gameManager.OnConnect());
        }

        private void OnTimerEvent(object source, ElapsedEventArgs e)
        {
            ws.Ping();
        }

        private void OnConnectionClosed(object sender, System.EventArgs e)
        {
            timer.Stop();
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
    }
}
