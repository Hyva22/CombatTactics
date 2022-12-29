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
            AddPacketHandler(PacketID.Register, ReceiveRegister);
            AddPacketHandler(PacketID.Login, ReceiveLogin);
        }

        private static void AddPacketHandler(PacketID id, Action<Packet> action)
        {
            client.packetHandler.AddPacketHandler(id, action);
        }

        #region Packet
        public static void SendRegister(PlayerAccount playerAccount)
        {
            Debug.Log("send register");
            Packet packet = new(PacketID.Register);
            string jsonString = playerAccount.ToJson();
            packet.Write(jsonString);
            client.tcp.SendData(packet);
        }

        public static void ReceiveRegister(Packet packet)
        {
            string playerString = packet.ReadString();
            bool emailTaken = packet.ReadBool();

            if(emailTaken)
            {
                Debug.Log("Email Taken");
                return;
            }

            PlayerAccount playerAccount = PersistantObject.FromJson<PlayerAccount>(playerString);
            Debug.Log($"Created: {playerAccount.accountName}");
        }

        public static void SendLogin(PlayerAccount playerAccount)
        {
            Debug.Log("send login");
            Packet packet = new(PacketID.Login);
            string jsonString = playerAccount.ToJson();
            packet.Write(jsonString);
            client.tcp.SendData(packet);
        }

        public static void ReceiveLogin(Packet packet)
        {
            Debug.Log("receive login");
            string playerString = packet.ReadString();
            bool valid = packet.ReadBool();

            if(!valid)
            {
                Debug.Log("incorrect credentials");
                return;
            }

            Debug.Log("A");

            PlayerAccount playerAccount = PersistantObject.FromJson<PlayerAccount>(playerString);
            Debug.Log("B");
            GameManager.Player.PlayerAccount = playerAccount;
            Debug.Log($"Login!");
        }
        #endregion Packet
    }
}