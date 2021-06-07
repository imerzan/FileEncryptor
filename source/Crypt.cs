using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FileEncryptor
{
    internal static class Crypt
    {

        public static void EncryptFile(string _password, string _sourceFile, string _destFile = null)
        {
            if (_destFile is null | _destFile == String.Empty) _destFile = _sourceFile + ".encrypted"; // Set default output destination
            if (!File.Exists(_sourceFile)) throw new FileNotFoundException("Source filepath not found!");
            var header = new FileHeader();
            using (var psk = new Rfc2898DeriveBytes(_password, 8, 100000, HashAlgorithmName.SHA512)) // 100,000 Iterations of Rfc2898 using 8 byte Salt
            using (var aes = new AesManaged() { Mode = CipherMode.CBC, KeySize = 256 }) // AES-CBC 256 Bits
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.BlockSize = 128; // 128 Bits Block/IV
                byte[] _key = psk.GetBytes(96); // Derive key from Rfc2898
                aes.Key = _key.Take(32).ToArray(); // Use 32 Bytes (256 Bits) for Encryption Key
                aes.GenerateIV(); // Generate Crypto Random IV
                using (var writer = new FileStream(_destFile, FileMode.Create, FileAccess.Write)) // Open output file for writing
                {
                    writer.Write(header.Bytes, 0, header.Bytes.Length); // Write header (2 bytes)
                    writer.Position = 66; // Leave 64 bytes at the start of the file (after 2 byte header) for the hash to be written later.
                    writer.Write(psk.Salt, 0, psk.Salt.Length); // Write salt as plaintext (8 bytes)
                    writer.Write(aes.IV, 0, aes.IV.Length); // Write IV as plaintext (16 bytes)
                    using (var reader = new FileStream(_sourceFile, FileMode.Open, FileAccess.Read)) // Open source file for reading
                    using (var cs = new CryptoStream(writer, aes.CreateEncryptor(), CryptoStreamMode.Write)) // Open CryptoStream for writing encrypted to output file
                    {
                        int bytesRead;
                        Span<byte> _buffer = stackalloc byte[256000]; // 256kb Stack allocated buffer
                        while ((bytesRead = reader.Read(_buffer)) > 0) // Keep reading until 0 bytes read
                        {
                            cs.Write(_buffer.Slice(0, bytesRead)); // Write (encrypted) to output file
                        }
                    } // Close source file, CryptoStream
                } // Close output file
                using (var hashWriter = new FileStream(_destFile, FileMode.Open, FileAccess.ReadWrite)) // Open output file for writing hash
                using (var hmac = new HMACSHA512(_key.Skip(32).Take(64).ToArray())) // Take next 64 bytes (512 Bits) for Hash Key
                {
                    hashWriter.Position = 66; // Calculate hash of SALT/IV/PAYLOAD
                    byte[] computedHash = new byte[64]; // Allocate 64 byte array for computed hash
                    computedHash = hmac.ComputeHash(hashWriter); // Compute hash from output file
                    hashWriter.Position = 2; // Write to blank space at beginning of file (after 2 byte header)
                    hashWriter.Write(computedHash, 0, computedHash.Length); // Write hash value (64 bytes)
                }
            }
        }

        public static void DecryptFile(string _password, string _sourceFile, string _destFile = null)
        {
            if (_destFile is null | _destFile == String.Empty) _destFile = _sourceFile + ".plaintext"; // Set default output destination
            if (!File.Exists(_sourceFile)) throw new FileNotFoundException("Source filepath not found!");
            using (var reader = new FileStream(_sourceFile, FileMode.Open, FileAccess.Read)) // Open source file for reading
            {
                byte[] _header = new byte[2];
                reader.Read(_header, 0, _header.Length); // Read 2 bytes for Header
                var header = new FileHeader(_header);
                byte[] _hash = new byte[64];
                byte[] _salt = new byte[8];
                byte[] _iv = new byte[16];
                reader.Read(_hash, 0, _hash.Length); // Read 64 bytes for Hash
                reader.Read(_salt, 0, _salt.Length); // Read 8 bytes for Salt
                reader.Read(_iv, 0, _iv.Length); // Read 16 bytes for IV
                using (var psk = new Rfc2898DeriveBytes(_password, _salt, 100000, HashAlgorithmName.SHA512)) // 100,000 Iterations of Rfc2898 using provided 8 byte Salt
                using (var aes = new AesManaged() { Mode = CipherMode.CBC, KeySize = 256 }) // AES-CBC 256 Bits
                {
                    aes.Padding = PaddingMode.PKCS7;
                    aes.BlockSize = 128; // 128 Bits Block/IV
                    byte[] _key = psk.GetBytes(96); // Derive key from RfcC2898
                    aes.Key = _key.Take(32).ToArray(); // Use 32 Bytes (256 Bits) for Encryption Key
                    aes.IV = _iv; // Use provided 16 byte IV
                    using (var hmac = new HMACSHA512(_key.Skip(32).Take(64).ToArray())) // Take next 64 bytes (512 Bits) for Hash Key
                    {
                        reader.Position = 66; // Read Salt/IV/Payload (skip header/hash portion)
                        byte[] computedHash = new byte[64]; // Allocate 64 byte array for computed hash
                        computedHash = hmac.ComputeHash(reader); // Compute hash from *source* file
                        reader.Position = 90; // Move back to previous reader position (after Salt/IV).
                        if (_hash.Length != computedHash.Length) throw new CryptographicException("Hash values are different lengths! Aborting decryption."); // Should never happen, but checking for good measure
                        for (int i = 0; i < _hash.Length; i++) // Compare given hash value with computed hash value
                        {
                            if (_hash[i] != computedHash[i]) throw new CryptographicException("Hash values do not match! Aborting decryption.\n" +
                                "Possible Causes:\n" +
                                "1. Incorrect password provided.\n" +
                                "2. File contents have been tampered with."); 
                        }
                    }
                    using (var writer = new FileStream(_destFile, FileMode.Create, FileAccess.Write)) // Open output file for writing
                    using (var cs = new CryptoStream(writer, aes.CreateDecryptor(), CryptoStreamMode.Write)) // Open CryptoStream for writing decrypted to output file
                    {
                        int bytesRead;
                        Span<byte> _buffer = stackalloc byte[256000]; // 256kb Stack allocated buffer
                        while ((bytesRead = reader.Read(_buffer)) > 0) // Keep reading until 0 bytes read
                        {
                            cs.Write(_buffer.Slice(0, bytesRead)); // // Write (decrypted) to output file
                        }
                    } // Close output file, CryptoStream
                }
            } // Close source file
        }
    }
}