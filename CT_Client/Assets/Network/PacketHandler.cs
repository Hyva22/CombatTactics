using System;
using System.Collections.Generic;

namespace Network
{
    public class PacketHandler
    {
        private readonly Dictionary<int, Action<Packet>> packetHandlers = new();

        /// <summary>
        /// Adds a method that handles a packet with the given ID to the server.
        /// </summary>
        /// <param name="id">ID of the packet.</param>
        /// <param name="packetHandler">Method that handles the packet.</param>
        public void AddPacketHandler(int id, Action<Packet> packetHandler)
        {
            if(packetHandlers.ContainsKey(id))
            {
                DebugOutput.DebugAction($"Could not add packet with ID: {id}. ID already exists.");
                return;
            }
            packetHandlers.Add(id, packetHandler);
        }

        /// <summary>
        /// Adds multiple actions to the packetHandler
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="packetHandlers"></param>
        public void AddPacketHandlers(List<int> ids, List<Action<Packet>> packetHandlers)
        {
            for (int i = 0; i < ids.Count && i < packetHandlers.Count; i++)
            {
                AddPacketHandler(ids[i], packetHandlers[i]);
            }
        }

        /// <summary>
        /// Calls corresponding action for the given packet.
        /// </summary>
        /// <param name="packet"></param>
        public void HandleData(Packet packet)
        {
            packetHandlers[packet.GetPacketID()](packet);
        }
    }
}
