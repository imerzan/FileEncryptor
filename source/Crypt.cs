using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FileEncryptor
{
    internal static class Crypt
    {
        private static readonly byte[] _Pepper = new byte[] { 0x7B, 0x72, 0xEB, 0x2D, 0x6F, 0xB2, 0x91, 0xE6 }; // Feel free to modify to your own random values

        public static void EncryptFile(string password, string sourceFile, string destFile)
        {
            if (!File.Exists(sourceFile)) throw new FileNotFoundException("Source file not found!");
            if (destFile is null || destFile.Trim() == String.Empty) throw new IOException("Must provide a destination filepath!");
            byte[] salt = GetRandomBytesCrypto(8); // Get Crypto-Random Salt 8 bytes
            using (var psk = new Rfc2898DeriveBytes(password, CombineBytes(salt, _Pepper), 237531, HashAlgorithmName.SHA512)) // 237,531 Iterations of Rfc2898 using 16 bytes salt/pepper
            using (var aes = new AesManaged()
            {
                Mode = CipherMode.CBC,
                KeySize = 256, // AES-CBC 256 Bits
                Padding = PaddingMode.PKCS7
            })
            {
                byte[] key = psk.GetBytes(96); // Derive key from Rfc2898
                aes.Key = key.Take(32).ToArray(); // Use 32 Bytes (256 Bits) for Encryption Key
                aes.GenerateIV(); // Generate Crypto Random IV
                using (var writer = new FileStream(destFile, FileMode.Create, FileAccess.ReadWrite)) // Open output file for writing
                {
                    writer.Write(Header.GetBytes(), 0, 2); // Write header (2 bytes)
                    writer.Position = 66; // Leave 64 bytes at the start of the file (after 2 byte header) for the hash to be written later.
                    writer.Write(salt, 0, salt.Length); // Write salt as plaintext (8 bytes) (Pepper NOT included)
                    writer.Write(aes.IV, 0, aes.IV.Length); // Write IV as plaintext (16 bytes)
                    using (var reader = new FileStream(sourceFile, FileMode.Open, FileAccess.Read)) // Open source file for reading
                    using (var cs = new CryptoStream(writer, aes.CreateEncryptor(), CryptoStreamMode.Write, true)) // Open CryptoStream for writing encrypted to output file
                    {
                        int bytesRead;
                        Span<byte> buffer = stackalloc byte[256000]; // 256kb Stack allocated buffer
                        while ((bytesRead = reader.Read(buffer)) > 0) // Keep reading until 0 bytes read
                        {
                            cs.Write(buffer.Slice(0, bytesRead)); // Write (encrypted) to output file
                        }
                    } // Close source file, CryptoStream
                    using (var hmac = new HMACSHA512(key.Skip(32).Take(64).ToArray())) // Take next 64 bytes (512 Bits) for Hash Key
                    {
                        writer.Position = 66; // Calculate hash starting with SALT/IV/PAYLOAD
                        byte[] computedHash = hmac.ComputeHash(writer); // Compute hash from output file
                        writer.Position = 2; // Write to blank space at beginning of file (after 2 byte header)
                        writer.Write(computedHash, 0, computedHash.Length); // Write hash value (64 bytes)
                    }
                } // Close output file
            }
        }

        public static void DecryptFile(string password, string sourceFile, string destFile)
        {
            if (!File.Exists(sourceFile)) throw new FileNotFoundException("Source file not found!");
            if (destFile is null || destFile.Trim() == String.Empty) throw new IOException("Must provide a destination filepath!");
            byte[] header = new byte[2];
            using (var reader = new FileStream(sourceFile, FileMode.Open, FileAccess.Read)) // Open source file for reading header
            {
                reader.Read(header, 0, header.Length); // Read Header (2 bytes)
            }
            const string headerException = "Invalid file header!\n" +
            "Possible Causes:\n" +
            "1. File was encoded using a newer version of FileEncryptor than the one you are currently using.\n" +
            "2. File header was modified/corrupted.";
            if (header[0] != 0xFF) throw new IOException(headerException); // First byte always 0xFF
            switch (header[1]) // Evaluate Header for Version and run appropriate Decryption Function
            {
                case (byte)Version.CurrentVersion:
                    DecryptFile_V3(password, sourceFile, destFile);
                    break;
                case (byte)Version.V2:
                    DecryptFile_V2(password, sourceFile, destFile);
                    break;
                case (byte)Version.V1:
                    DecryptFile_V1(password, sourceFile, destFile);
                    break;
                default:
                    throw new IOException(headerException);
            }
        }

        // Version Definitions
        private static void DecryptFile_V1(string password, string sourceFile, string destFile)
        {
            using (var reader = new FileStream(sourceFile, FileMode.Open, FileAccess.Read)) // Open source file for reading
            {
                byte[] hash = new byte[64];
                byte[] salt = new byte[8];
                byte[] iv = new byte[16];
                reader.Position = 2; // Skip Header
                reader.Read(hash, 0, hash.Length); // Read 64 bytes for Hash
                reader.Read(salt, 0, salt.Length); // Read 8 bytes for Salt
                reader.Read(iv, 0, iv.Length); // Read 16 bytes for IV
                using (var psk = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA512)) // 100,000 Iterations of Rfc2898 using provided 8 byte Salt
                using (var aes = new AesManaged()
                {
                    Mode = CipherMode.CBC,
                    KeySize = 256, // AES-CBC 256 Bits
                    Padding = PaddingMode.PKCS7
                })
                {
                    byte[] key = psk.GetBytes(96); // Derive key from RfcC2898
                    aes.Key = key.Take(32).ToArray(); // Use 32 Bytes (256 Bits) for Encryption Key
                    aes.IV = iv; // Use provided 16 byte IV
                    using (var hmac = new HMACSHA512(key.Skip(32).Take(64).ToArray())) // Take next 64 bytes (512 Bits) for Hash Key
                    {
                        reader.Position = 66; // Read Salt/IV/Payload (skip header/hash portion)
                        byte[] computedHash = hmac.ComputeHash(reader); // Compute hash from *source* file
                        reader.Position = 90; // Move back to previous reader position (after Salt/IV).
                        ValidateHash(hash, computedHash);
                    }
                    using (var writer = new FileStream(destFile, FileMode.Create, FileAccess.Write)) // Open output file for writing
                    using (var cs = new CryptoStream(writer, aes.CreateDecryptor(), CryptoStreamMode.Write)) // Open CryptoStream for writing decrypted to output file
                    {
                        int bytesRead;
                        Span<byte> buffer = stackalloc byte[256000]; // 256kb Stack allocated buffer
                        while ((bytesRead = reader.Read(buffer)) > 0) // Keep reading until 0 bytes read
                        {
                            cs.Write(buffer.Slice(0, bytesRead)); // // Write (decrypted) to output file
                        }
                    } // Close output file, CryptoStream
                }
            } // Close source file
        } // End DecryptFile_V1()

        private static void DecryptFile_V2(string password, string sourceFile, string destFile)
        {
            using (var reader = new FileStream(sourceFile, FileMode.Open, FileAccess.Read)) // Open source file for reading
            {
                byte[] hash = new byte[64];
                byte[] salt = new byte[8];
                byte[] iv = new byte[16];
                reader.Position = 2; // Skip Header
                reader.Read(hash, 0, hash.Length); // Read 64 bytes for Hash
                reader.Read(salt, 0, salt.Length); // Read 8 bytes for Salt
                reader.Read(iv, 0, iv.Length); // Read 16 bytes for IV
                using (var psk = new Rfc2898DeriveBytes(password, CombineBytes(salt, _Pepper), 100000, HashAlgorithmName.SHA512)) // 100,000 Iterations of Rfc2898 using 16 bytes salt/pepper
                using (var aes = new AesManaged()
                { 
                    Mode = CipherMode.CBC, 
                    KeySize = 256, // AES-CBC 256 Bits
                    Padding = PaddingMode.PKCS7
                }) 
                {
                    byte[] key = psk.GetBytes(96); // Derive key from RfcC2898
                    aes.Key = key.Take(32).ToArray(); // Use 32 Bytes (256 Bits) for Encryption Key
                    aes.IV = iv; // Use provided 16 byte IV
                    using (var hmac = new HMACSHA512(key.Skip(32).Take(64).ToArray())) // Take next 64 bytes (512 Bits) for Hash Key
                    {
                        reader.Position = 66; // Read Salt/IV/Payload (skip header/hash portion)
                        byte[] computedHash = hmac.ComputeHash(reader); // Compute hash from *source* file
                        reader.Position = 90; // Move back to previous reader position (after Salt/IV).
                        ValidateHash(hash, computedHash);
                    }
                    using (var writer = new FileStream(destFile, FileMode.Create, FileAccess.Write)) // Open output file for writing
                    using (var cs = new CryptoStream(writer, aes.CreateDecryptor(), CryptoStreamMode.Write)) // Open CryptoStream for writing decrypted to output file
                    {
                        int bytesRead;
                        Span<byte> buffer = stackalloc byte[256000]; // 256kb Stack allocated buffer
                        while ((bytesRead = reader.Read(buffer)) > 0) // Keep reading until 0 bytes read
                        {
                            cs.Write(buffer.Slice(0, bytesRead)); // // Write (decrypted) to output file
                        }
                    } // Close output file, CryptoStream
                }
            } // Close source file
        } // End DecryptFile_V2()

        private static void DecryptFile_V3(string password, string sourceFile, string destFile)
        {
            using (var reader = new FileStream(sourceFile, FileMode.Open, FileAccess.Read)) // Open source file for reading
            {
                byte[] hash = new byte[64];
                byte[] salt = new byte[8];
                byte[] iv = new byte[16];
                reader.Position = 2; // Skip Header
                reader.Read(hash, 0, hash.Length); // Read 64 bytes for Hash
                reader.Read(salt, 0, salt.Length); // Read 8 bytes for Salt
                reader.Read(iv, 0, iv.Length); // Read 16 bytes for IV
                using (var psk = new Rfc2898DeriveBytes(password, CombineBytes(salt, _Pepper), 237531, HashAlgorithmName.SHA512)) // 237,531 Iterations of Rfc2898 using 16 bytes salt/pepper
                using (var aes = new AesManaged()
                {
                    Mode = CipherMode.CBC,
                    KeySize = 256, // AES-CBC 256 Bits
                    Padding = PaddingMode.PKCS7
                })
                {
                    byte[] key = psk.GetBytes(96); // Derive key from RfcC2898
                    aes.Key = key.Take(32).ToArray(); // Use 32 Bytes (256 Bits) for Encryption Key
                    aes.IV = iv; // Use provided 16 byte IV
                    using (var hmac = new HMACSHA512(key.Skip(32).Take(64).ToArray())) // Take next 64 bytes (512 Bits) for Hash Key
                    {
                        reader.Position = 66; // Read Salt/IV/Payload (skip header/hash portion)
                        byte[] computedHash = hmac.ComputeHash(reader); // Compute hash from *source* file
                        reader.Position = 90; // Move back to previous reader position (after Salt/IV).
                        ValidateHash(hash, computedHash);
                    }
                    using (var writer = new FileStream(destFile, FileMode.Create, FileAccess.Write)) // Open output file for writing
                    using (var cs = new CryptoStream(writer, aes.CreateDecryptor(), CryptoStreamMode.Write)) // Open CryptoStream for writing decrypted to output file
                    {
                        int bytesRead;
                        Span<byte> buffer = stackalloc byte[256000]; // 256kb Stack allocated buffer
                        while ((bytesRead = reader.Read(buffer)) > 0) // Keep reading until 0 bytes read
                        {
                            cs.Write(buffer.Slice(0, bytesRead)); // // Write (decrypted) to output file
                        }
                    } // Close output file, CryptoStream
                }
            } // Close source file
        } // End DecryptFile_V3()

        private static byte[] GetRandomBytesCrypto(int length) // Used in V2 and later
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[length];
                rng.GetBytes(bytes);
                return bytes;
            }
        }

        private static byte[] CombineBytes(byte[] arr1, byte[] arr2) // returns new byte array: Array1 followed by Array2
        {
            byte[] newArr = new byte[arr1.Length + arr2.Length];
            Buffer.BlockCopy(arr1, 0, newArr, 0, arr1.Length);
            Buffer.BlockCopy(arr2, 0, newArr, arr1.Length, arr2.Length);
            return newArr;
        }

        private static void ValidateHash(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length) throw new CryptographicException("INVALID HASH: Hash values are different lengths!"); // Should never happen, but checking for good measure
            for (int i = 0; i < hash1.Length; i++) // Compare given hash value with computed hash value
            {
                if (hash1[i] != hash2[i]) throw new CryptographicException("INVALID HASH: Hash values do not match!\n" +
                    "Possible Causes:\n" +
                    "1. Incorrect password provided.\n" +
                    "2. File contents have been modified.");
            }
        }
    }
}