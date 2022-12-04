using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Network.Server
{
    class Client : NetworkCommunication
    {
        private readonly Server server;

        public Client(int ID, Server server, PacketHandler handler) : base(handler)
        {
            this.ID = ID;
            this.server = server;
        }

        public void Connect(TcpClient _socket)
        {
            TcpClient = _socket;
            TcpClient.ReceiveBufferSize = dataBufferSize;
            TcpClient.SendBufferSize = dataBufferSize;

            NetworkStream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            server.SendClientID(ID);
        }
    }
}