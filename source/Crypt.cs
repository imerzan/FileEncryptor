using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FileEncryptor
{
    internal static class Crypt
    {

        public static void EncryptFile(string _password, string _sourceFile, string _destFile = null, bool _hashOutput = false, string _hashDest = null)
        {
            if (_destFile is null | _destFile == String.Empty) _destFile = _sourceFile + ".encrypted"; // Set default output destination
            if (_hashDest is null | _hashDest == String.Empty) _hashDest = _sourceFile + ".hash"; // Set default hashfile destination
            if (!File.Exists(_sourceFile)) throw new FileNotFoundException("Source filepath not found!");
            using (var psk = new Rfc2898DeriveBytes(_password, 8, 1000)) // 1000 Iterations of Rfc2898 using 8 byte Salt
            using (var aes = new AesManaged() { Mode = CipherMode.CBC, KeySize = 256 }) // AES-CBC 256 Bits
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.BlockSize = 128; // 128 Bits Block/IV
                byte[] _key = psk.GetBytes(96); // Derive key from RFC2898
                aes.Key = _key.Take(32).ToArray(); // Use 32 Bytes (256 Bits) for Encryption Key
                aes.GenerateIV(); // Generate Crypto Random IV
                using (var writer = new FileStream(_destFile, FileMode.Create, FileAccess.Write)) // Open output file for writing
                {
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
                if (_hashOutput) // true , hash output file using HMAC-SHA-512
                {
                    using (var hashReader = new FileStream(_destFile, FileMode.Open, FileAccess.Read)) // Open output file for reading
                    using (var hashWriter = new FileStream(_hashDest, FileMode.Create, FileAccess.Write)) // Open hash file for writing
                    using (var hmac = new HMACSHA512(_key.Skip(32).Take(64).ToArray())) // Take next 64 bytes (512 Bits) for Hash Key
                    {
                        byte[] computedHash = hmac.ComputeHash(hashReader); // Compute hash from output file
                        hashWriter.Write(computedHash, 0, computedHash.Length); // Write to hash file
                    } // Close output file, hash file
                }
            }

        }

        public static void DecryptFile(string _password, string _sourceFile, string _destFile = null, string _hashFile = null)
        {
            if (_destFile is null | _destFile == String.Empty) _destFile = _sourceFile + ".plaintext"; // Set default output destination
            if (!File.Exists(_sourceFile)) throw new FileNotFoundException("Source filepath not found!");
            if (!(_hashFile is null) & _hashFile != String.Empty)
            {
                if (!File.Exists(_hashFile)) throw new FileNotFoundException("Hash filepath not found!");
            }
            using (var reader = new FileStream(_sourceFile, FileMode.Open, FileAccess.Read)) // Open source file for reading
            {
                byte[] _salt = new byte[8];
                byte[] _iv = new byte[16];
                reader.Read(_salt, 0, _salt.Length); // Read 8 bytes for Salt
                reader.Read(_iv, 0, _iv.Length); // Read 16 bytes for IV
                using (var psk = new Rfc2898DeriveBytes(_password, _salt, 1000)) // 1000 Iterations of Rfc2898 using provided 8 byte Salt
                using (var aes = new AesManaged() { Mode = CipherMode.CBC, KeySize = 256 }) // AES-CBC 256 Bits
                {
                    aes.Padding = PaddingMode.PKCS7;
                    aes.BlockSize = 128; // 128 Bits Block/IV
                    byte[] _key = psk.GetBytes(96); // Derive key from RFC2898
                    aes.Key = _key.Take(32).ToArray(); // Use 32 Bytes (256 Bits) for Encryption Key
                    aes.IV = _iv; // Use provided 16 byte IV
                    if (!(_hashFile is null) & _hashFile != String.Empty) // Check designated hash file
                    {
                        using (var hashReader = new FileStream(_hashFile, FileMode.Open, FileAccess.Read)) // Open hash file for reading
                        using (var hmac = new HMACSHA512(_key.Skip(32).Take(64).ToArray())) // Take next 64 bytes (512 Bits) for Hash Key
                        {
                            byte[] _hash = new byte[64]; // Hash file should be exactly 64 bytes
                            hashReader.Read(_hash, 0, _hash.Length); // Read hash from *hash* file
                            reader.Position = 0; // Read from beginning of *source* file (including the Salt/IV)
                            byte[] computedHash = hmac.ComputeHash(reader); // Compute hash from *source* file
                            reader.Position = 24; // Move back to previous reader position (after Salt/IV).
                            for (int i = 0; i < _hash.Length; i++)
                            {
                                if (_hash[i] != computedHash[i]) throw new CryptographicException("Hash values do not match!"); // Compare hash values from source & hash files
                            }
                        } // Close hash file
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