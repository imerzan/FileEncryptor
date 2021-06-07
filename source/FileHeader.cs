using System;

namespace FileEncryptor
{
    public class FileHeader
    {
        public byte[] Bytes { get; private set; }

        public FileHeader()
        {
            this.GetDefaultBytes();
            this.DecodeBytes();
        }

        public FileHeader(byte[] inBytes)
        {
            if (inBytes.Length != 2) throw new Exception("Invalid header length!");
            this.Bytes = inBytes;
            this.DecodeBytes();
        }

        private void DecodeBytes() // Set parameters for values that may have adjusted between different program versions
        {
            switch (this.Bytes[0])
            {
                case (byte)Version.Current:
                    break;
                default:
                    throw new Exception("Invalid file header!\n" +
                        "Possible Causes:\n" +
                        "1. File was encoded using a newer version of FileEncryptor than the one you are currently using.\n" +
                        "2. File was modified/corrupted.");
            }
        }

        private void GetDefaultBytes() // Get current version bytes
        {
            this.Bytes = new byte[2];
            this.Bytes[0] = (byte)Version.Current;
            this.Bytes[1] = 0x00; // Reserved for future use
        }
    }

    public enum Version : byte
    {
        Current = 0x01 // V1
    }
}
