using Network;
using Network.Client;
using Network.Game;
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
            client.ConnectToServer();
        }

        #region send
        public static void Register(PlayerAccount playerAccount)
        {
            Packet packet = new((int)PacketID.Register);
            string jsonString = playerAccount.ToJson();
            packet.Write(jsonString);
            client.SendData(packet);
        }
        #endregion send

        #region receive
        #endregion receive
    }
}