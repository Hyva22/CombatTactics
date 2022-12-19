using Network.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Network.Client
{
    public class Client
    {
        private int id;
        private readonly string serverIP;
        private readonly int serverPort;
        private readonly PacketHandler packetHandler;

        public TCP tcp;
        public UDP udp;

        public Client(string serverIP, int serverPort)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            packetHandler = new();
            packetHandler.AddPacketHandler(PacketID.ClientID, ReceiveID);
            packetHandler.AddPacketHandler(PacketID.UPDTest, ReceivUDPTest);

            tcp = new(packetHandler, id);
            udp = new(packetHandler, id);
        }

        public void ConnectToServer()
        {
            DebugOutput.DebugAction($"Connecting to server: {serverIP} on port: {serverPort}.");
            tcp.Connect(serverIP, serverPort);
        }

        #region Receive
        private void ReceiveID(Packet packet)
        {
            tcp.ID = packet.ReadInt();
            DebugOutput.DebugAction($"Receiving ID from server: {tcp.ID}...");
            udp.id = tcp.ID;
            udp.Connect(serverIP, serverPort, ((IPEndPoint)tcp.TcpClient.Client.LocalEndPoint).Port);
        }

        private void ReceivUDPTest(Packet packet)
        {
            DebugOutput.DebugAction(packet.ReadString());
        }
        #endregion Receive
    }
}
