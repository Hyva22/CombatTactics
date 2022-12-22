using Network;
using Network.Client;
using Network.Game;
using System;
using UnityEngine;

namespace GameClient
{
    public static class GameClient
    {
        public static Client client;

        public static void ConnectToServer(string serverIP, int serverPort)
        {
            DebugOutput.DebugAction = Debug.Log;
            client = new(serverIP, serverPort);
            InitializePacketHandlers();
            client.ConnectToServer();
        }

        private static void InitializePacketHandlers()
        {
            AddPacketHandler(PacketID.EmailTaken, ReceiveEmailTaken);
        }

        private static void AddPacketHandler(PacketID id, Action<Packet> action)
        {
            client.packetHandler.AddPacketHandler(id, action);
        }

        #region send
        public static void Register(PlayerAccount playerAccount)
        {
            Debug.Log("send register");
            Packet packet = new(PacketID.Register);
            string jsonString = playerAccount.ToJson();
            packet.Write(jsonString);
            client.tcp.SendData(packet);
        }
        #endregion send

        #region receive
        public static void ReceiveEmailTaken(Packet packet)
        {
            string email = packet.ReadString();
            Debug.Log($"The email {email} is taken");
        }
        #endregion receive
    }
}