using DataBase.Persistance;
using Network;
using Network.Game;
using Network.Server;

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
            server.packetHandler.AddPacketHandler((int)PacketID.Login, ReceiveLogin);
        }

        #region Packet
        private void ReceiveRegister(Packet packet)
        {
            string jsonString = packet.ReadString();
            PlayerAccount playerAccount = PersistantObject.FromJson<PlayerAccount>(jsonString);
            string email = playerAccount.email;

            if (DatabaseQueries.PlayerExistsByEmail(email))
            {
                SendRegister(packet.GetSenderID(), playerAccount, emailTaken: true);
                return;
            }

            DatabaseQueries.Insert(playerAccount);
            SendRegister(packet.GetSenderID(), playerAccount);
        }

        public void SendRegister(int clientID, PlayerAccount playerAccount, bool emailTaken = false)
        {
            Packet packet = new(PacketID.Register);
            packet.Write(playerAccount.ToJson());
            packet.Write(emailTaken);
            server.SendTCP(clientID, packet);
        }

        private void ReceiveLogin(Packet packet)
        {
            Console.WriteLine("Login");
            string jsonString = packet.ReadString();
            PlayerAccount receivedAccount = PersistantObject.FromJson<PlayerAccount>(jsonString);
            string email = receivedAccount.email;

            PlayerAccount foundAccount = DatabaseQueries.Find<PlayerAccount>("email", receivedAccount.email, () => new());

            if (foundAccount.id == 0 || foundAccount.password != receivedAccount.password)
            {
                SendLogin(packet.GetSenderID(), receivedAccount, invalid: true);
                return;
            }

            SendLogin(packet.GetSenderID(), foundAccount);
        }

        public void SendLogin(int clientID, PlayerAccount playerAccount, bool invalid = false)
        {
            Console.WriteLine("Send Login");
            Packet packet = new(PacketID.Login);
            packet.Write(playerAccount.ToJson());
            packet.Write(invalid);
            server.SendTCP(clientID, packet);
        }
        #endregion Packet
    }
}
