using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    class MockEncryptionService : AbstractEncryptor {
        public MockEncryptionService() : base("") {

        }

        public override string Decrypt(string data) {
            return data;
        }

        public override string Decrypt(string data, string customPlainPassword) {
            return data;
        }

        public override string Encrypt(string data) {
            return data;
        }

        public override string Encrypt(string data, string customPlainPassword) {
            return data;
        }
    }
}
