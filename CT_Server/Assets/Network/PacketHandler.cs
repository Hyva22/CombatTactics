using System;
using System.Collections.Generic;

namespace Network
{
    public class PacketHandler
    {
        internal readonly Dictionary<int, Action<Packet>> packetHandlers = new();

        /// <summary>
        /// Adds a method that handles a packet with the given ID to the server.
        /// </summary>
        /// <param name="id">ID of the packet. Must be greater or equal 0. negative values are reserved.</param>
        /// <param name="packetHandler">Method that handles the packet.</param>
        public void AddPacketHandler(int id, Action<Packet> packetHandler)
        {
            if (id < 0)
            {
                throw new Exception("can not add packet handler with negative id!");
            }
            packetHandlers.Add(id, packetHandler);
        }

        public void AddPacketHandlers(List<int> ids, List<Action<Packet>> packetHandlers)
        {
            for (int i = 0; i < ids.Count && i < packetHandlers.Count; i++)
            {
                AddPacketHandler(ids[i], packetHandlers[i]);
            }
        }

        public void HandleData(Packet packet)
        {
            packetHandlers[packet.GetPacketID()](packet);
        }
    }
}
