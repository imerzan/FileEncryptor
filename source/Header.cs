namespace FileEncryptor
{
    internal static class Header
    {
        public static byte[] GetBytes()
        {
            return new byte[] { 0xFF, (byte)Version.CurrentVersion }; // 2 Bytes
        }
    }

    public enum Version : byte
    {
        CurrentVersion = 0x03, // V3
        V2 = 0x02,
        V1 = 0x01
    }
}
