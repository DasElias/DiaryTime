using Diary.Model;
using Diary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Events {
    class EntryArgument {
        public EntryArgument(DiaryEntry entry, AbstractPersistorService service) {
            Entry = entry;
            PersistorService = service;
        }

        public DiaryEntry Entry { get; }
        public AbstractPersistorService PersistorService { get; }
    }
}
