using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    public delegate void PasswordChangedHandler(string oldPlainPw, string newPlainPw);

    abstract class AbstractEncryptor {
        private string _plainPassword;
        public event PasswordChangedHandler OnPasswordChanged;

        public AbstractEncryptor(string plainPassword) {
            _plainPassword = plainPassword;
        }

        public string PlainPassword {
            get {
                return _plainPassword;
            }
            set {
                OnPasswordChanged.Invoke(_plainPassword, value);
                _plainPassword = value;
            }
        }

        public abstract string Encrypt(string data);
        public abstract string Decrypt(string data);
        public abstract string Encrypt(string data, string customPlainPassword);
        public abstract string Decrypt(string data, string customPlainPassword);
    }
}
