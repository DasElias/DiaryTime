using Diary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Events {
    class PersistorEncryptorArgument {
        public PersistorEncryptorArgument(AbstractPersistorService persistor, AbstractEncryptor encryptor) {
            this.Persistor = persistor;
            this.Encryptor = encryptor;
        }

        public AbstractPersistorService Persistor { get; }
        public AbstractEncryptor Encryptor { get; }
    }
}
