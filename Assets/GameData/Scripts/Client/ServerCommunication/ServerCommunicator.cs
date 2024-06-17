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
        private string ip;
        private string port;
        private ClientGameManager gameManager;
        private int messageCount = 0;

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
            Debug.Log("error" + e.Exception);
        }

        private void OnConnected(object sender, System.EventArgs e)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() => gameManager.OnConnect());
        }

        private void OnConnectionClosed(object sender, System.EventArgs e)
        {
            gameManager.OnServerEndConnection();
        }

        void OnDestroy()
        {
            Disconnect();
        }

        private void Send(string message)
        {
            ws.Send(message);
        }

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
