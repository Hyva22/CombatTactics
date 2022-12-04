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
            server.packetHandler.AddPacketHandler((int)PacketID.IsEmailTaken, ReceiveIsEmailTaken);
            server.packetHandler.AddPacketHandler((int)PacketID.Register, ReceiveRegister);
        }

        #region send
        public void SendIsEmailTaken(int clientID, bool value)
        {
            Packet packet = new((int)PacketID.IsEmailTaken, id);
            packet.Write(value);
            server.SendToClient(clientID, packet);
        }
        #endregion send

        #region receive
        private void ReceiveRegister(Packet packet)
        {
            packet.Skip();

            string jsonString = packet.ReadString();
            Console.WriteLine(jsonString);
            PlayerAccount playerAccount = PersistantObject.FromJson<PlayerAccount>(jsonString);

            Console.WriteLine(playerAccount.email);
            DatabaseQueries.AddPlayerAccount(playerAccount);
        }

        private void ReceiveIsEmailTaken(Packet packet)
        {
            packet.Skip();
            string email = packet.ReadString();
            SendIsEmailTaken(packet.GetSenderID(), DatabaseQueries.PlayerExistsByEmail(email));
        }
        #endregion receive
    }
}
