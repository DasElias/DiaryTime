using Diary.Model;
using Diary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Events {
    class PersistorArgument {
        public PersistorArgument(AbstractPersistorService service) {
            PersistorService = service;
        }

        public AbstractPersistorService PersistorService { get; }
    }
}
