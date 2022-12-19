using Network.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Network.Server
{
    class Client
    {
        private readonly Server server;

        public TCP tcp;
        public UDP udp;

        public Client(int ID, Server server)
        {
            tcp = new(server.packetHandler, ID);
            udp = new(server.packetHandler, ID);
            this.server = server;
        }

        public void Connect(TcpClient _socket)
        {
            tcp.Connect(_socket);
            server.SendClientID(tcp.ID);
        }
    }
}