using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Network.Client
{
    public class Client : NetworkCommunication
    {
        private readonly string serverIP;
        private readonly int port;

        public Client(string serverIP, int port) : base()
        {
            this.serverIP = serverIP;
            this.port = port;
            TcpClient = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            packetHandler.packetHandlers.Add(-1, ReceiveID);
        }

        public void ConnectToServer()
        {
            DebugOutput.DebugAction($"Connecting to server: {serverIP} on port: {port}.");
            TcpClient.BeginConnect(serverIP, port, ConnectCallback, TcpClient);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            try
            {
                TcpClient.EndConnect(_result);
            }catch(Exception e)
            {
                DebugOutput.DebugAction(e.Message);
            }

            if (!TcpClient.Connected)
            {
                DebugOutput.DebugAction("Could not connect!");
                return;
            }
            NetworkStream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        #region Send
        #endregion Send

        #region Receive
        private void ReceiveID(Packet packet)
        {
            packet.Skip();
            ID = packet.ReadInt();
            DebugOutput.DebugAction($"Receiving ID from server: {ID}...");
        }
        #endregion Receive
    }
}
