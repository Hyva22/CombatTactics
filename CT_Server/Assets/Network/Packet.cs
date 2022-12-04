using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class Packet
    {
        private List<byte> buffer;
        private int readPosition;

        public Packet(int packetID = -1, int senderID = -1)
        {
            readPosition = 0;
            buffer = new List<byte>();
            Write(sizeof(int));
            Write(packetID);
            Write(senderID);
        }

        public Packet(byte[] data)
        {
            readPosition = 0;
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

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public int ReadInt(bool moveReadPos = true)
        {
            if (buffer.Count < readPosition + 4)
            {
                DebugOutput.DebugAction($"Could not read value of type 'int' of length: 4, there are only {buffer.Count - readPosition} bytes left in the buffer!");
            }

            // If there are unread bytes
            int value = BitConverter.ToInt32(buffer.GetRange(readPosition, 4).ToArray()); // Convert the bytes to an int
            if (moveReadPos)
            {
                // If _moveReadPos is true
                readPosition += 4; // Increase readPos by 4
            }
            return value; // Return the int
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public string ReadString(bool moveReadPos = true)
        {
            if (buffer.Count < readPosition + 4)
            {
                DebugOutput.DebugAction($"Could not read value of type 'int' of length: 4, there are only {buffer.Count - readPosition} bytes left in the buffer!");
            }

            // If there are unread bytes
            int length = ReadInt(); // Read length of the string.

            if (buffer.Count < readPosition + length)
            {
                DebugOutput.DebugAction($"Could not read string of length: {length}, there are only {buffer.Count - readPosition} bytes left in the buffer!");
            }

            string value = Encoding.Unicode.GetString(buffer.GetRange(readPosition, length).ToArray()); // Convert the bytes to a string

            if (moveReadPos)
            {
                readPosition += length; // Increase readPos by the length of the string
            }
            else
            {
                readPosition -= 4; // Move readPos back by 4 to reset the read int.
            }
            return value; // Return the read string
        }
        #endregion Read
    }
}