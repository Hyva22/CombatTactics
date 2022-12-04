using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Network
{
    public enum PacketID
    {
        Register,
        IsEmailTaken,
        GetAccountForMail,
        GetAccountForID,
        Login,
    }

    public class Packet
    {
        private List<byte> buffer;
        private int readPosition;

        public Packet(int packetID = -1, int senderID = -1)
        {
            readPosition = sizeof(int) * 3;
            buffer = new List<byte>();
            Write(sizeof(int));
            Write(packetID);
            Write(senderID);
        }

        public Packet(byte[] data)
        {
            readPosition = sizeof(int) * 3;
            buffer = new List<byte>(data);
        }

        /// <summary>
        /// Calculates the packet size and writes in the beginning of the buffer. Should be called every time after adding or removing data from the buffer.
        /// </summary>
        private void UpdateLength()
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
            {
                buffer.Clear(); // Clear buffer
            }
            readPosition = 0;
        }

        /// <summary>Gets the length of the packet's content.</summary>
        public int Length()
        {
            return buffer.Count; // Return the length of buffer
        }

        /// <summary>Gets the length of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - readPosition; // Return the remaining length (unread)
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
            UpdateLength(); //Update the length of the packet
        }

        public void Write(int value)
        {
            buffer.AddRange(BitConverter.GetBytes(value)); // Add the int itself
            UpdateLength(); //Update the length of the packet
        }

        public void Write(bool value)
        {
            buffer.AddRange(BitConverter.GetBytes(value)); // Add the int itself
            UpdateLength(); //Update the length of the packet
        }

        /// <summary>Adds a string to the packet.</summary>
        /// <param name="value">The string to add.</param>
        public void Write(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value); //Convert to byte array first to prevent errors with different bytes per character from different encoding.
            Write(bytes.Length); // Add the length of the string to the packet
            buffer.AddRange(bytes); // Add the string itself
            UpdateLength(); //Update the length of the packet
        }
        #endregion Write

        #region Read
        /// <summary>
        /// Skips past Packet length, packet ID, Sernder ID
        /// </summary>
        public void Skip()
        {
            ReadInt();
            ReadInt();
            ReadInt();
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
            int size = ReadInt(); // Read length of the string.
            byte[] bytes = CheckSize(size, moveReadPos); //Perform size checks and get byte array
            string value = Encoding.Unicode.GetString(bytes); // Convert the bytes to an int
            return value; // Return the int
        }
        #endregion Read
    }
}