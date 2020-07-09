using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Utils {
    static class ByteArrayHasher {
        public static string Hash(byte[] arr) {
            using(SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider()) {
                return Convert.ToBase64String(sha1.ComputeHash(arr));
            }
        }
    }
}
