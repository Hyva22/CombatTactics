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
        private readonly Dictionary<int, Client> clients;

        public Server(int maxConnections, int port, int serverID)
        {
            MaxConnections = maxConnections;
            Port = port;
            ServerID = serverID;
            clients = new();
            tcpListener = new TcpListener(IPAddress.Any, Port);
        }

        public void Start()
        {
            DebugOutput.DebugAction($"Starting server...");

            InitializeServerData();
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

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
                clients.Add(i, new Client(i, this, packetHandler));
            }
        }

        public List<bool> GetConnectionStatus()
        {
            List<bool> status = new();
            foreach (Client client in clients.Values)
            {
                status.Add(client.TcpClient != null);
            }
            return status;
        }

        private void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient tcpClient = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            DebugOutput.DebugAction($"Incoming connection from {tcpClient.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxConnections; i++)
            {
                if (clients[i].TcpClient == null)
                {
                    clients[i].Connect(tcpClient);
                    return;
                }
            }
        }

        #region Send
        public void SendToClient(int clientID, Packet packet)
        {
            var packetName = (PacketID)packet.GetPacketID();
            DebugOutput.DebugAction($"Sending packet {packetName} to client {clientID}...");
            clients.TryGetValue(clientID, out var client);
            client?.SendData(packet);
        }

        internal void SendClientID(int clientID)
        {
            Packet packet = new(PacketID.ClientID);
            packet.Write(clientID);
            SendToClient(clientID, packet);
        }
        #endregion Send
    }
}