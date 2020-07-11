using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    class DatabaseStructureService {
        // First database scheme, was used in the first released version. 
        // 
        // entries(date, title, rawText, rtfText)
        private const int VERSION_1 = 1;

        // Added support for images.
        // entries(date, title, rawText, rtfText)
        // images(hash, date, imageData)
        private const int VERSION_2 = 2;

        private const int CURRENT_VERSION = VERSION_2;

        public enum CreationResult {
            DB_WAS_JUST_CREATED,
            DB_DID_EXIST
        }

        private DatabaseVersionService versionService;
        private SqliteConnection connection;

        public DatabaseStructureService(SqliteConnection connection) {
            this.connection = connection;
            this.versionService = new DatabaseVersionService(connection);
        }

        public CreationResult EnsureDatabaseIsSetUp() {
            if(DoesTableExist()) {
                MigrateIfNecessary();
                return CreationResult.DB_DID_EXIST;
            } else {
                CreateTables();
                return CreationResult.DB_WAS_JUST_CREATED;
            }
        }

        private bool DoesTableExist() {
            using(var command = new SqliteCommand()) {
                command.Connection = connection;
                command.CommandText = @"SELECT name 
                                       FROM sqlite_master 
                                       WHERE type = 'table' AND name = 'entries';";

                using(SqliteDataReader reader = command.ExecuteReader()) {
                    return reader.Read();
                }
            }
        }

        private void CreateTables() {
            using(var command = new SqliteCommand()) {
                command.Connection = connection;
                command.CommandText = @"CREATE TABLE entries (" +
                                       "date INTEGER PRIMARY KEY, " +
                                       "title TEXT NOT NULL, " +
                                       "rawText TEXT NOT NULL, " +
                                       "rtfText TEXT NOT NULL);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE images (" +
                                      "hash TEXT," +
                                      "date INTEGER," +
                                      "imageData BLOB NOT NULL," +
                                      "PRIMARY KEY (hash, date)," +
                                      "FOREIGN KEY (date) REFERENCES entries(date)" +
                                      ");";
                command.ExecuteNonQuery();
            }

            versionService.Version = CURRENT_VERSION;
        }

        private void MigrateIfNecessary() {
            if(versionService.Version < VERSION_2) PerformMigrationToVersion2();

        }

        private void PerformMigrationToVersion2() {
            using(var command = connection.CreateCommand()) {
                command.CommandText = "CREATE TABLE images (" +
                                      "hash TEXT," +
                                      "date INTEGER," +
                                      "imageData BLOB NOT NULL," +
                                      "PRIMARY KEY (hash, date)," +
                                      "FOREIGN KEY (date) REFERENCES entries(date)" +
                                      ");";
                command.ExecuteNonQuery();
            }

            versionService.Version = VERSION_2;
        }
    }
}
