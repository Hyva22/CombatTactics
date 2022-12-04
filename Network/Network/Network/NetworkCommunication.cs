using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Network
{
    public abstract class NetworkCommunication
    {
        public int ID { get; protected set; }
        public TcpClient TcpClient { get; protected set; }
        protected const int dataBufferSize = 4096;
        protected byte[] receiveBuffer;
        protected List<byte> unreadData;
        public readonly PacketHandler packetHandler;

        protected NetworkStream NetworkStream => TcpClient.GetStream();

        protected NetworkCommunication(PacketHandler packetHandler = null)
        {
            receiveBuffer = new byte[dataBufferSize];
            unreadData = new List<byte>();
            this.packetHandler = packetHandler?? new PacketHandler();
        }

        protected void ReceiveCallback(IAsyncResult _result)
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

        protected void HandleData(byte[] data)
        {
            unreadData.AddRange(data);

            for (int packetLength = unreadData.Count >= 4 ? BitConverter.ToInt32(unreadData.ToArray()) : 0;
                packetLength > 0 && unreadData.Count >= packetLength;
                packetLength = unreadData.Count >= 4 ? BitConverter.ToInt32(unreadData.ToArray()) : 0)
            {
                Packet packet = new(unreadData.GetRange(0, packetLength).ToArray());
                packetHandler.HandleData(packet);
                unreadData.RemoveRange(0, packetLength);
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                NetworkStream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch (Exception)
            {

            }
        }
    }
}
