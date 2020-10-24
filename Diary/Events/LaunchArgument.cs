using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Events {
    class LaunchArgument {
        public LaunchArgument(string databaseName) {
            DatabaseName = databaseName;
        }

        public string DatabaseName { get; }
    }
}
