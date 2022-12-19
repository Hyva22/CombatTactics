using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Network.Network
{
    public class TCP
    {
        public int ID;
        public TcpClient TcpClient { get; private set; }
        private const int dataBufferSize = 4096;
        private readonly byte[] receiveBuffer;
        private readonly List<byte> unreadData;
        private readonly PacketHandler packetHandler;

        private NetworkStream NetworkStream => TcpClient.GetStream();

        public TCP(PacketHandler packetHandler, int id)
        {
            this.packetHandler = packetHandler;
            this.ID = id;
            unreadData = new();
            receiveBuffer = new byte[dataBufferSize];
        }

        public void Connect(TcpClient _tcpClient)
        {
            TcpClient = _tcpClient;
            TcpClient.ReceiveBufferSize = dataBufferSize;
            TcpClient.SendBufferSize = dataBufferSize;

            NetworkStream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void Connect(string ip, int port)
        {
            TcpClient = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            TcpClient.BeginConnect(ip, port, ConnectToServerCallback, TcpClient);
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.SetSenderID(ID);
                packet.UpdateLength();
                NetworkStream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch (Exception exception)
            {
                DebugOutput.DebugAction(exception.Message);
            }
        }

        private void ConnectToServerCallback(IAsyncResult result)
        {
            TcpClient.EndConnect(result);

            if (!TcpClient.Connected)
            {
                return;
            }

            NetworkStream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = NetworkStream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    // TODO: disconnect
                    return;
                }

                byte[] data = new byte[_byteLength];
                Array.Copy(receiveBuffer, data, _byteLength);

                HandleData(data);

                NetworkStream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                // TODO: disconnect
            }
        }

        private void HandleData(byte[] data)
        {
            unreadData.AddRange(data);

            for (int packetLength = unreadData.Count >= 4 ? BitConverter.ToInt32(unreadData.ToArray()) : 0;
                packetLength > 0 && unreadData.Count >= packetLength;
                packetLength = unreadData.Count >= 4 ? BitConverter.ToInt32(unreadData.ToArray()) : 0)
            {
                Packet packet = new(unreadData.GetRange(0, packetLength).ToArray());
                packet.SetSenderID(ID);
                packetHandler.HandleData(packet);
                unreadData.RemoveRange(0, packetLength);
            }
        }
    }
}
