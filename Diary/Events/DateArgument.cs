using Diary.Model;
using Diary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Events {
    class DateArgument {
        public DateArgument(DateTime date, AbstractPersistorService service) {
            Date = date;
            PersistorService = service;
        }

        public DateTime Date { get; }
        public AbstractPersistorService PersistorService { get; }
    }
}
