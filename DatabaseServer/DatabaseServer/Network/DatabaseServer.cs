using DataBase.Persistance;
using Network;
using Network.Game;
using Network.Server;
using System.Text.Json;

namespace DatabaseServer.Network
{
    internal class DatabaseServer
    {
        private readonly Server server;

        private const int maxConnections = 10;
        private const int port = 22220;
        private const int id = 1;

        public DatabaseServer()
        {
            server = new Server(maxConnections, port, id);
            server.Start();
            InitializePacketHandlers();
        }

        private void InitializePacketHandlers()
        {
            server.packetHandler.AddPacketHandler((int)PacketID.Register, ReceiveRegister);
        }

        #region send
        public void SendEmailTaken(int clientID, string email)
        {
            Packet packet = new(PacketID.EmailTaken);
            packet.Write(email);
            server.SendTCP(clientID, packet);
        }
        #endregion send

        #region receive
        private void ReceiveRegister(Packet packet)
        {
            string jsonString = packet.ReadString();
            PlayerAccount playerAccount = PersistantObject.FromJson<PlayerAccount>(jsonString);
            string email = playerAccount.email;

            if (DatabaseQueries.PlayerExistsByEmail(email))
            {
                SendEmailTaken(packet.GetSenderID(), email);
                return;
            }

            DatabaseQueries.AddPlayerAccount(playerAccount);
        }
        #endregion receive
    }
}
