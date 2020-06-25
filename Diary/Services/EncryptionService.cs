using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    class EncryptionService : AbstractEncryptor {
        public EncryptionService(string password) : base(password) {

        }

        public override string Encrypt(string plainText) {
            return Encrypt(plainText, PlainPassword);
        }

        public override string Decrypt(string encryptedText) {
            return Decrypt(encryptedText, PlainPassword);
        }

        public override string Encrypt(string data, string customPlainPassword) {
            return EncryptImpl(data, HashPassword(customPlainPassword));
        }

        public override string Decrypt(string data, string customPlainPassword) {
            return DecryptImpl(data, HashPassword(customPlainPassword));
        }

        public static byte[] HashPassword(string plainPassword) {
            using(SHA256 shaProvider = SHA256.Create()) {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
                return shaProvider.ComputeHash(passwordBytes);
            }
        }

        public static string EncryptImpl(string data, byte[] hashedPassword) {
            byte[] iv = new byte[16];
            byte[] encryptedBuffer;

            using(Aes aes = Aes.Create()) {
                aes.Key = hashedPassword;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using(MemoryStream memoryStream = new MemoryStream()) {
                    using(CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
                        using(StreamWriter streamWriter = new StreamWriter(cryptoStream)) {
                            streamWriter.Write(data);
                        }
                    }
                    encryptedBuffer = memoryStream.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedBuffer);
        }

        public static string DecryptImpl(string data, byte[] hashedPassword) {
            byte[] iv = new byte[16];
            byte[] decryptedBuffer = Convert.FromBase64String(data);

            try {
                using(Aes aes = Aes.Create()) {
                    aes.Key = hashedPassword;
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using(MemoryStream memoryStream = new MemoryStream(decryptedBuffer)) {
                        using(CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
                            using(StreamReader streamReader = new StreamReader(cryptoStream)) {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            } catch(CryptographicException) {
                throw new InvalidPasswordException();
            }
        }
    }
}
