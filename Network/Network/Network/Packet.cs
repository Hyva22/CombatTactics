using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Network
{
    public enum PacketID
    {
        ClientID,
        Register,
        EmailTaken,
        GetAccountForMail,
        GetAccountForID,
        Login,
    }

    public class Packet
    {
        private List<byte> buffer;
        private int readPosition;

        public Packet(PacketID packetID)
        {
            int baseLength = sizeof(int) * 3;
            buffer = new List<byte>();
            Write(baseLength);
            Write((int)packetID);
            Write(-1); //Write palceholder sender ID;
            readPosition = baseLength;
        }

        public Packet(byte[] data)
        {
            readPosition = sizeof(int) * 3;
            buffer = new List<byte>(data);
        }

        /// <summary>
        /// Calculates the packet size and writes in the beginning of the buffer. Should be called every time after adding or removing data from the buffer.
        /// </summary>
        public void UpdateLength()
        {
            int length = buffer.Count; //Save size of the packet
            buffer.RemoveRange(0, sizeof(int)); //remove old size of the packet from the beginning of the buffer
            buffer.InsertRange(0, BitConverter.GetBytes(length)); //Insert new size of the packet in the beginning of the buffer
        }

        /// <summary>Resets the packet instance to allow it to be reused.</summary>
        /// <param name="clearData">Whether or not to delet the content of the package.</param>
        public void Reset(bool clearData = false)
        {
            if (clearData)
                buffer.Clear(); // Clear buffer
            readPosition = 0;
        }

        /// <summary>Gets the size of the packet's content.</summary>
        public int Length()
        {
            return buffer.Count; // Return the size of buffer
        }

        /// <summary>Gets the size of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - readPosition; // Return the remaining size (unread)
        }

        /// <summary>
        /// Returns the ID of the packet.
        /// </summary>
        /// <returns></returns>
        public int GetPacketID()
        {
            return BitConverter.ToInt32(buffer.GetRange(4, 4).ToArray());
        }

        /// <summary>
        /// Returns the ID of the sender.
        /// </summary>
        /// <returns></returns>
        public int GetSenderID()
        {
            return BitConverter.ToInt32(buffer.GetRange(8, 4).ToArray());
        }

        /// <summary>
        /// Sets the sender ID, should be called right before sending Data
        /// </summary>
        internal void SetSenderID(int id)
        {
            const int size = sizeof(int); //Length to be removed and written 
            const int senderIDPosition = size * 2; //startposition of senderID
            buffer.RemoveRange(senderIDPosition, size); //remove old id from the packet.
            buffer.InsertRange(senderIDPosition, BitConverter.GetBytes(id)); //Insert new id into the packet.
        }

        /// <summary>Sets the packet's content and prepares it to be read.</summary>
        /// <param name="_data">The bytes to add to the packet.</param>
        public void SetBytes(byte[] data)
        {
            buffer = new List<byte>(data);
        }

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[] ToArray()
        {
            return buffer.ToArray();
        }

        #region Write
        /// <summary>Adds an array of bytes to the packet.</summary>
        /// <param name="_value">The byte array to add.</param>
        public void Write(byte[] _value)
        {
            buffer.AddRange(_value);
        }

        public void Write(int value)
        {
            buffer.AddRange(BitConverter.GetBytes(value)); // Add the int itself
        }

        public void Write(bool value)
        {
            buffer.AddRange(BitConverter.GetBytes(value)); // Add the int itself
        }

        /// <summary>Adds a string to the packet.</summary>
        /// <param name="value">The string to add.</param>
        public void Write(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value); //Convert to byte array first to prevent errors with different bytes per character from different encoding.
            Write(bytes.Length); // Add the size of the string to the packet
            buffer.AddRange(bytes); // Add the string itself
            UpdateLength(); //Update the size of the packet
        }
        #endregion Write

        #region Read
        /// <summary>
        /// Skips past Packet size, packet ID, Sernder ID
        /// does nothing if readposition si already advanced further
        /// </summary>
        [Obsolete]
        public void Skip()
        {
            int start = sizeof(int) * 3;
            if (readPosition < start)
                readPosition = start;
        }

        private byte[] CheckSize(int size, bool moveReadPos = true)
        {
            //Check if there are enough bytes in the array
            if (buffer.Count < readPosition + size)
            {
                DebugOutput.DebugAction($"Could not read value of length: {size}, there are only {buffer.Count - readPosition} bytes left in the buffer!");
                //TODO: Handle error
            }
            
            byte[] bytes = buffer.GetRange(readPosition, size).ToArray(); //Get bytes of datatype

            if (moveReadPos)
                readPosition += size;

            return bytes;
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public int ReadInt(bool moveReadPos = true)
        {
            const int size = sizeof(int); //Get size of datatype
            byte[] bytes = CheckSize(size, moveReadPos); //Perform size checks and get byte array
            int value = BitConverter.ToInt32(bytes); // Convert the bytes to an int
            return value; // Return the int
        }

        /// <summary>Reads a bool from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public bool ReadBool(bool moveReadPos = true)
        {
            const int size = sizeof(bool); //Get size of datatype
            byte[] bytes = CheckSize(size, moveReadPos); //Perform size checks and get byte array
            bool value = BitConverter.ToBoolean(bytes); // Convert the bytes to an int
            return value; // Return the int
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public string ReadString(bool moveReadPos = true)
        {
            int size = ReadInt(); // Read size of the string.
            byte[] bytes = CheckSize(size, moveReadPos); //Perform size checks and get byte array
            string value = Encoding.Unicode.GetString(bytes); // Convert the bytes to an int
            return value; // Return the int
        }
        #endregion Read
    }
}