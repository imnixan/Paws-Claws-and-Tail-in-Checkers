namespace PJTC.Server
{
    public class PlayersCommunicator
    {
        public readonly PlayerDataSender playerDataSender;
        public readonly PlayerDataHandler playerDataHandler;

        public PlayersCommunicator(
            PlayerDataSender playerDataSender,
            PlayerDataHandler playerDataHandler
        )
        {
            this.playerDataSender = playerDataSender;
            this.playerDataHandler = playerDataHandler;
        }

        public void Init(ServerGameManager gameManager)
        {
            this.playerDataHandler.SetGameManager(gameManager);
        }

        public int RemovePlayer(int playerID)
        {
            return playerDataSender.GetActiveListenersCount(playerID);
        }
    }
}
