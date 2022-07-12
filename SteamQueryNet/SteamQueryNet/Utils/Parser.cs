using System;
using System.Linq;
using System.Text;

namespace SteamQueryNet.Utils
{
    internal class Parser
    {
        internal Parser(byte[] data)
        {
            this.Data = data;
            this.CurrentPosition = -1;
            this.LastPosition = this.Data.Length - 1;
        }

        internal bool HasMore
        {
            get
            {
                return this.CurrentPosition + 1 <= this.LastPosition;
            }
        }


        internal byte ReadByte()
        {
            this.CurrentPosition++;
            if (this.CurrentPosition > this.LastPosition)
            {
                throw new Exception("Index was outside the bounds of the byte array.");
            }
            return this.Data[this.CurrentPosition];
        }


        internal ushort ReadShort()
        {
            this.CurrentPosition++;
            if (this.CurrentPosition + 1 > this.LastPosition)
            {
                throw new Exception("Unable to parse bytes to short.");
            }
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(this.Data, this.CurrentPosition, 2);
            }
            ushort num = BitConverter.ToUInt16(this.Data, this.CurrentPosition);
            this.CurrentPosition++;
            return num;
        }


        internal int ReadInt()
        {
            this.CurrentPosition++;
            if (this.CurrentPosition + 3 > this.LastPosition)
            {
                throw new Exception("Unable to parse bytes to int.");
            }
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(this.Data, this.CurrentPosition, 4);
            }
            int num = BitConverter.ToInt32(this.Data, this.CurrentPosition);
            this.CurrentPosition += 3;
            return num;
        }


        internal long ReadLong()
        {
            this.CurrentPosition++;
            if (this.CurrentPosition + 7 > this.LastPosition)
            {
                throw new Exception("Unable to parse bytes to long.");
            }
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(this.Data, this.CurrentPosition, 8);
            }
            long num = BitConverter.ToInt64(this.Data, this.CurrentPosition);
            this.CurrentPosition += 7;
            return num;
        }


        internal float ReadFloat()
        {
            this.CurrentPosition++;
            if (this.CurrentPosition + 3 > this.LastPosition)
            {
                throw new Exception("Unable to parse bytes to float.");
            }
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(this.Data, this.CurrentPosition, 4);
            }
            float num = BitConverter.ToSingle(this.Data, this.CurrentPosition);
            this.CurrentPosition += 3;
            return num;
        }


        internal string ReadString()
        {
            this.CurrentPosition++;
            int currentPosition = this.CurrentPosition;
            while (this.Data[this.CurrentPosition] != 0)
            {
                this.CurrentPosition++;
                if (this.CurrentPosition > this.LastPosition)
                {
                    throw new Exception("Unable to parse bytes to string.");
                }
            }
            return Encoding.UTF8.GetString(this.Data, currentPosition, this.CurrentPosition - currentPosition);
        }


        internal void Skip(byte count)
        {
            this.CurrentPosition += (int)count;
            if (this.CurrentPosition > this.LastPosition)
            {
                throw new Exception("skip count was outside the bounds of the byte array.");
            }
        }

        // Token: 0x060001EA RID: 490 RVA: 0x00002D6D File Offset: 0x00000F6D
        internal byte[] GetUnParsedData()
        {
            return this.Data.Skip(this.CurrentPosition + 1).ToArray<byte>();
        }


        private byte[] Data;


        private int CurrentPosition = -1;


        private int LastPosition;
    }
}
