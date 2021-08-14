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
        CurrentVersion = 0x02, // V2
        V1 = 0x01
    }
}
