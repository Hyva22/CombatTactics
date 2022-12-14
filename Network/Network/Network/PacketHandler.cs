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
        /// <param name="packetID">ID of the packet.</param>
        /// <param name="packetHandler">Method that handles the packet.</param>
        public void AddPacketHandler(int packetID, Action<Packet> packetHandler)
        {
            if(packetHandlers.ContainsKey(packetID))
            {
                DebugOutput.DebugAction($"Could not add packet with ID: {packetID}. ID already exists.");
                return;
            }
            packetHandlers.Add(packetID, packetHandler);
        }

        /// <summary>
        /// Adds a method that handles a packet with the given ID to the server.
        /// </summary>
        /// <param name="packetID">ID of the packet.</param>
        /// <param name="packetHandler">Method that handles the packet.</param>
        public void AddPacketHandler(PacketID packetID, Action<Packet> packetHandler)
        {
            AddPacketHandler((int)packetID, packetHandler);
        }

        /// <summary>
        /// Adds multiple actions to the packetHandler
        /// </summary>
        /// <param name="packetIDs"></param>
        /// <param name="packetHandlers"></param>
        public void AddPacketHandlers(List<int> packetIDs, List<Action<Packet>> packetHandlers)
        {
            for (int i = 0; i < packetIDs.Count && i < packetHandlers.Count; i++)
            {
                AddPacketHandler(packetIDs[i], packetHandlers[i]);
            }
        }

        /// <summary>
        /// Adds multiple actions to the packetHandler
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="packetHandlers"></param>
        public void AddPacketHandlers(List<PacketID> packetIDs, List<Action<Packet>> packetHandlers)
        {
            List<int> intPacketIDs = packetIDs.ConvertAll(x => (int)x);
            AddPacketHandlers(intPacketIDs, packetHandlers);
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
