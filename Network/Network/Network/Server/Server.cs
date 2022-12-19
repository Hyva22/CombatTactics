using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Network.Server
{
    public class Server
    {
        public readonly int MaxConnections;
        public readonly int Port;
        public readonly int ServerID;

        public readonly PacketHandler packetHandler = new();

        private readonly TcpListener tcpListener;
        private readonly UdpClient udpListener;
        private readonly Dictionary<int, Client> clients;

        public Server(int maxConnections, int port, int serverID)
        {
            MaxConnections = maxConnections;
            Port = port;
            ServerID = serverID;
            clients = new();
            tcpListener = new TcpListener(IPAddress.Any, Port);
            udpListener = new UdpClient(port);
        }

        public void Start()
        {
            DebugOutput.DebugAction($"Starting server...");

            InitializeServerData();
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpListener.BeginReceive(UDPReceiveCallback, null);

            DebugOutput.DebugAction($"Server started on port {Port}.");
        }

        public void Stop()
        {
            tcpListener.Stop();
        }

        private void InitializeServerData()
        {
            for (int i = 1; i <= MaxConnections; i++)
            {
                clients.Add(i, new Client(i, this));
            }
        }

        private void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient tcpClient = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            DebugOutput.DebugAction($"Incoming connection from {tcpClient.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxConnections; i++)
            {
                if (clients[i].tcp.TcpClient == null)
                {
                    clients[i].Connect(tcpClient);
                    return;
                }
            }
        }

        private void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint clientEndPoint = new(IPAddress.Any, 0); //Endpoint received from UDP protocoll
                byte[] receivedData = udpListener.EndReceive(result, ref clientEndPoint);
                Packet receivedPacket = new(receivedData);
                udpListener.BeginReceive(UDPReceiveCallback, null); //Continue listening.

                if (!receivedPacket.Validate())
                    return;

                //Return if sender ID is out of bounds
                int clientID = receivedPacket.GetSenderID();
                if (0 > clientID || clientID > clients.Count)
                    return;

                //If IPEndpoint is null, it is a new connection and we connect it.
                var clientUDP = clients[clientID].udp;//endpoint fetched from local udpClient
                if (clientUDP.ipEndPoint == null)
                {
                    clientUDP.Connect(clientEndPoint, udpListener);
                    return;
                }

                //Only handle data if the ipendpoints match, to prevent impersonation.
                if (clientUDP.ipEndPoint.ToString() == clientEndPoint.ToString())
                    clientUDP.HandleData(receivedPacket);
            }
            catch(Exception excption)
            {
                DebugOutput.DebugAction($"Error receiving UDP data: {excption.Message}");
            }
        }

        #region Send
        public void SendTCPData(int clientID, Packet packet)
        {
            var packetName = (PacketID)packet.GetPacketID();
            DebugOutput.DebugAction($"Sending TCP packet {packetName} to client {clientID}...");
            clients.TryGetValue(clientID, out var client);
            client.tcp.SendData(packet);
        }

        public void SendUDPData(int clientID, Packet packet)
        {
            var packetName = (PacketID)packet.GetPacketID();
            DebugOutput.DebugAction($"Sending UDP packet {packetName} to client {clientID}...");
            clients.TryGetValue(clientID, out var client);
            client.udp.SendData(packet);
        }

        internal void SendClientID(int clientID)
        {
            Packet packet = new(PacketID.ClientID);
            packet.Write(clientID);
            SendTCPData(clientID, packet);
        }

        public void SendUDPTest(int clientID)
        {
            Packet packet = new(PacketID.UPDTest);
            packet.Write("UDP test");
            SendUDPData(clientID, packet);
        }
        #endregion Send
    }
}