using System;
using System.Linq;
using System.Text;

namespace SteamQueryNet48.Utils
{
    internal class Parser
    {
        // Token: 0x060001E1 RID: 481 RVA: 0x00002CCC File Offset: 0x00000ECC
        internal Parser(byte[] data)
        {
            this.Data = data;
            this.CurrentPosition = -1;
            this.LastPosition = this.Data.Length - 1;
        }

        // Token: 0x1700008C RID: 140
        // (get) Token: 0x060001E2 RID: 482 RVA: 0x00002CF9 File Offset: 0x00000EF9
        internal bool HasMore
        {
            get
            {
                return this.CurrentPosition + 1 <= this.LastPosition;
            }
        }

        // Token: 0x060001E3 RID: 483 RVA: 0x00002D0E File Offset: 0x00000F0E
        internal byte ReadByte()
        {
            this.CurrentPosition++;
            if (this.CurrentPosition > this.LastPosition)
            {
                throw new Exception("Index was outside the bounds of the byte array.");
            }
            return this.Data[this.CurrentPosition];
        }

        // Token: 0x060001E4 RID: 484 RVA: 0x00006168 File Offset: 0x00004368
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

        // Token: 0x060001E5 RID: 485 RVA: 0x000061D8 File Offset: 0x000043D8
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

        // Token: 0x060001E6 RID: 486 RVA: 0x00006248 File Offset: 0x00004448
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

        // Token: 0x060001E7 RID: 487 RVA: 0x000062B8 File Offset: 0x000044B8
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

        // Token: 0x060001E8 RID: 488 RVA: 0x00006328 File Offset: 0x00004528
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

        // Token: 0x060001E9 RID: 489 RVA: 0x00002D44 File Offset: 0x00000F44
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

        // Token: 0x04000122 RID: 290
        private byte[] Data;

        // Token: 0x04000123 RID: 291
        private int CurrentPosition = -1;

        // Token: 0x04000124 RID: 292
        private int LastPosition;
    }
}
