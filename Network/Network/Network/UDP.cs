using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Network.Network
{
    public class UDP
    {
        public int id;
        public UdpClient udpClient;
        public IPEndPoint ipEndPoint;
        private readonly PacketHandler packetHandler;

        public UDP(PacketHandler packetHandler, int id)
        {
            this.packetHandler = packetHandler;
            this.id = id;
        }

        /// <summary>
        /// Sets up UDP to send through the UdpClient of the server
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="udpClient"></param>
        public void Connect(IPEndPoint ipEndPoint, UdpClient udpClient)
        {
            DebugOutput.DebugAction($"Connect UDP");
            this.ipEndPoint = ipEndPoint;
            this.udpClient = udpClient;
        }

        /// <summary>
        /// Connects UDP to a remote server
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="serverPort"></param>
        /// <param name="localPort"></param>
        public void Connect(string serverIP, int serverPort, int localPort)
        {
            ipEndPoint = new(IPAddress.Parse(serverIP), serverPort);
            udpClient = new(localPort);
            udpClient.BeginReceive(ReceiveCallback, null);

            SendData(new Packet(0));
            DebugOutput.DebugAction("Sending empty UDP");
        }

        /// <summary>
        /// receive callback method for receiving data from a server
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                byte[] data = udpClient.EndReceive(result, ref ipEndPoint);
                udpClient.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4)
                {
                    //TODO: handle short data
                    return;
                }

                HandleData(data);
            }
            catch
            {
                //TODO: error handling
            }
        }

        /// <summary>
        /// Sends a datagram to the remote ipendpoint
        /// </summary>
        /// <param name="packet"></param>
        public void SendData(Packet packet)
        {
            try
            {
                packet.SetSenderID(id);
                packet.UpdateLength();
                udpClient.BeginSend(packet.ToArray(), packet.Length(), ipEndPoint, null, null);
            }
            catch (Exception exception)
            {
                DebugOutput.DebugAction($"Error sending data via UDP: {exception.Message}");
            }
        }

        public void HandleData(Packet packet)
        {
            packetHandler.HandleData(packet);
        }

        public void HandleData(byte[] data)
        {
            HandleData(new Packet(data));
        }
    }
}
