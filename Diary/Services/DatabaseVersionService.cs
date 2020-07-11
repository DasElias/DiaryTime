using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    class DatabaseVersionService {
        public DatabaseVersionService(SqliteConnection connection) {
            this.Connection = connection;
        }

        public SqliteConnection Connection { get; set; }

        public int Version {
            get {
                using(var command = Connection.CreateCommand()) {
                    command.CommandText = "PRAGMA user_version;";
                    object value = command.ExecuteScalar();

                    if(value is long) return (int) (long) value;
                    return (int) value;

                }
            }
            set {
                using(var command = Connection.CreateCommand()) {
                    // for some reason, the pragma statement didn't work with a parameter
                    command.CommandText = $"PRAGMA user_version = {value};";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
