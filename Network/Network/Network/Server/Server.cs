using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Network.Server
{
    public class Server
    {
        public readonly int MaxConnections; //Max number of client allowed to connect to this server at the same time
        public readonly int Port; //Port of the TCP and UDP socket on which the server will be opened
        public readonly int ID; //ID of the server. Used for the clients to identify the server they communicate with.

        public readonly PacketHandler packetHandler = new(); //How the server handles incoming messages

        private readonly TcpListener tcpListener; //TCP listener for incoming connections
        private readonly UdpClient udpListener; //UDP client for incoming datagrams
        private readonly Dictionary<int, Client> clients; //List of connected clients to this server

        public Server(int maxConnections, int port, int id)
        {
            MaxConnections = maxConnections;
            Port = port;
            ID = id;
            clients = new();
            tcpListener = new TcpListener(IPAddress.Any, Port);
            udpListener = new UdpClient(port);
        }

        /// <summary>
        /// Boots up the server and opens the sockets for incoming connections and communication
        /// </summary>
        public void Start()
        {
            DebugOutput.DebugAction($"Starting server...");

            InitializeServerData();

            //Open TCP socket and begin accepting incoming connection requests
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            //Open UDP socket for incoming communication
            udpListener.BeginReceive(UDPReceiveCallback, null);

            DebugOutput.DebugAction($"Server started on port {Port}.");
        }


        /// <summary>
        /// shuts down the server and closes all sockets.
        /// </summary>
        public void Stop()
        {
            tcpListener.Stop();
            udpListener.Close();
        }

        /// <summary>
        /// Initialize list of clients ahead of time
        /// </summary>
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

                if (clientEndPoint == null)
                    return;

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

        /// <summary>
        /// Sends a packet to a client via TCP
        /// </summary>
        /// <param name="client">Client to send packet to</param>
        /// <param name="packet">packet to send</param>
        private void SendTCP(Client client, Packet packet)
        {
            client.tcp.SendData(packet);
        }

        /// <summary>
        /// Sends a packet to a client via TCP
        /// </summary>
        /// <param name="client">Id of client to send packet to</param>
        /// <param name="packet">packet to send</param>
        public void SendTCP(int clientID, Packet packet)
        {
            SendTCP(clients[clientID], packet);
        }

        /// <summary>
        /// Sends a packet to all clients via TCP
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet">packet to send</param>
        public void SendTCPToAll(Packet packet)
        {
            for(int i = 0; i < clients.Count; i++)
                SendTCP(clients[i], packet);
        }

        /// <summary>
        /// Sends a packet to all clients, except one via TCP
        /// </summary>
        /// <param name="client">Id of client which should NOT receive the packet</param>
        /// <param name="packet">packet to send</param>
        public void SendTCPToAllExcept(int clientID, Packet packet)
        {
            for(int i = 0; i < clients.Count; i++)
                if(i != clientID)
                    SendTCP(clients[i], packet);
        }

        /// <summary>
        /// Sends a packet to a client via UDP
        /// </summary>
        /// <param name="client">Client to send packet to</param>
        /// <param name="packet">packet to send</param>
        private void SendUDP(Client client, Packet packet)
        {
            client.udp.SendData(packet);
        }

        /// <summary>
        /// Sends a packet to a client via UDP
        /// </summary>
        /// <param name="client">Id of client to send packet to</param>
        /// <param name="packet">packet to send</param>
        public void SendUDP(int clientID, Packet packet)
        {
            SendUDP(clients[clientID], packet);
        }


        /// <summary>
        /// Sends a packet to all clients via UDP
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet">packet to send</param>
        public void SendUDPToAll(Packet packet)
        {
            for (int i = 0; i < clients.Count; i++)
                SendUDP(clients[i], packet);
        }

        /// <summary>
        /// Sends a packet to all clients, except one via UDP
        /// </summary>
        /// <param name="client">Id of client which should NOT receive the packet</param>
        /// <param name="packet">packet to send</param>
        public void SendUDPToAllExcept(int clientID, Packet packet)
        {
            for (int i = 0; i < clients.Count; i++)
                if (i != clientID)
                    SendUDP(clients[i], packet);
        }

        #region Send
        /// <summary>
        /// Sends the ID assigned tot he client on the server side to the client.
        /// Should be send after connection was established.
        /// </summary>
        /// <param name="clientID">ID of the client on the server</param>
        internal void SendClientID(int clientID)
        {
            Packet packet = new(PacketID.ClientID);
            packet.Write(clientID);
            SendTCP(clientID, packet);
        }

        /// <summary>
        /// Sends a single string to the client.
        /// The way this is interpreted is left to the client.
        /// </summary>
        /// <param name="clientID">ID of the recipient</param>
        /// <param name="message">string to send</param>
        public void SendMessage(int clientID, string message)
        {
            Packet packet = new(PacketID.SendMessage);
            packet.Write(message);
            SendTCP(clientID, packet);
        }

        /// <summary>
        /// Test message for UDP
        /// </summary>
        /// <param name="clientID">ID of the recipient</param>
        public void SendUDPTest(int clientID)
        {
            Packet packet = new(PacketID.UPDTest);
            packet.Write("UDP test");
            SendUDP(clientID, packet);
        }
        #endregion Send
    }
}