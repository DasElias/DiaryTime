using Diary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Security.Cryptography;
using Diary.Utils;
using System.IO.IsolatedStorage;
using Windows.Storage;
using System.Collections.ObjectModel;
using Nito.AsyncEx;

namespace Diary.Services {
    class DatabasePersistorService : AbstractPersistorService {
        // When the database file has been created, but there is no diary entry present yet, we don't have 
        // any possiblity to verify that the entered password is correct. Therefore, we add an entry with a specific 
        // date value and this text as plain text in hashed form to be able to check later if the password is correct.
        private const string PW_CHECK_ENTRY_PLAIN_TEXT = "DiaryTime Password Check";
        private static readonly DateTime PW_CHECK_ENTRY_DATE = DateTime.MinValue;

        private SqliteConnection connection;
        private AbstractEncryptor encryptor;

        public DatabasePersistorService(AbstractEncryptor encryptor, string databaseName) : this(encryptor, AsyncContext.Run(() => GetStorageFileFromDatabaseName(databaseName))) {

        }

        public DatabasePersistorService(AbstractEncryptor encryptor, StorageFile dbFile) {
            this.encryptor = encryptor;
            this.StorageFile = dbFile;
            // handler needs to be removed
            encryptor.OnPasswordChanged += HandleEncryptor_PasswordChanged;

            string connectionString = $"Filename={dbFile.Path}";
            connection = new SqliteConnection(connectionString);

            try {
                connection.Open();

                DatabaseStructureService structureService = new DatabaseStructureService(connection);
                var setupResult = structureService.EnsureDatabaseIsSetUp();
                if(setupResult == DatabaseStructureService.CreationResult.DB_DID_EXIST) {
                    DiaryEntry pwCheckEntry = LoadEntry(PW_CHECK_ENTRY_DATE);
                    if(pwCheckEntry.PlainContent != PW_CHECK_ENTRY_PLAIN_TEXT) {
                        throw new InvalidPasswordException();
                    }
                } else {
                    DiaryEntry pwCheckEntry = new DiaryEntry(PW_CHECK_ENTRY_DATE, PW_CHECK_ENTRY_PLAIN_TEXT, PW_CHECK_ENTRY_PLAIN_TEXT, PW_CHECK_ENTRY_PLAIN_TEXT);
                    CreateEntryImpl(pwCheckEntry);
                }
            } catch(Exception) {
                connection.Close();
                throw;
            }
        }

        private static async Task<StorageFile> GetStorageFileFromDatabaseName(string databaseName) {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(databaseName, CreationCollisionOption.OpenIfExists);
            return file;
        }

        public static async Task<bool> DoesDatabaseExist(string dbName) {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            IStorageItem item = await folder.TryGetItemAsync(dbName);
            return item != null;
        }

        public static async Task DropDatabase(string dbName) {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            IStorageItem item = await folder.TryGetItemAsync(dbName);

            if(item != null) {

                await item.DeleteAsync();
            }

        }

        public override StorageFile StorageFile { get; }

        private void HandleEncryptor_PasswordChanged(string oldPlainPw, string newPlainPw) {
            var entries = GetAllEntries(oldPlainPw);
            using(var command = new SqliteCommand()) {
                command.Connection = connection;

                for(int i = 0; i < entries.Count; i++) {
                    DiaryEntry e = entries[i];
                    command.CommandText += string.Format("UPDATE entries SET title = @title{0}, rawText = @rawText{0}, rtfText = @rtfText{0} WHERE date = @date{0};", i);
                    command.Parameters.AddWithValue("@title" + i, encryptor.Encrypt(e.Title, newPlainPw));
                    command.Parameters.AddWithValue("@rawText" + i, encryptor.Encrypt(e.PlainContent, newPlainPw));
                    command.Parameters.AddWithValue("@rtfText" + i, encryptor.Encrypt(e.RtfText, newPlainPw));
                    command.Parameters.AddWithValue("@date" + i, DateUtils.ToUnixtime(e.Date));
                }

                command.ExecuteNonQuery();
            }
        }

        private List<DiaryEntry> GetAllEntries(string decryptPassword) {
            List<DiaryEntry> allEntries = new List<DiaryEntry>();
            using(var command = new SqliteCommand()) {
                command.Connection = connection;
                command.CommandText = "SELECT date, title, rawText, rtfText FROM entries;";

                using(SqliteDataReader reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        DateTime date = DateUtils.FromUnixtime(reader.GetInt64(0));
                        string title = encryptor.Decrypt(reader.GetString(1), decryptPassword);
                        string rawText = encryptor.Decrypt(reader.GetString(2), decryptPassword);
                        string rtfText = encryptor.Decrypt(reader.GetString(3), decryptPassword);

                        allEntries.Add(new DiaryEntry(date, title, rawText, rtfText));
                    }
                }
            }
            return allEntries;
        }

        protected override void CreateEntryImpl(DiaryEntry entry) {
            using(var command = new SqliteCommand()) {
                command.Connection = connection;
                command.CommandText = @"INSERT INTO entries (title, rawText, rtfText, date) VALUES (@title, @rawText, @rtfText, @date);";
                command.Parameters.AddWithValue("@title", encryptor.Encrypt(entry.Title));
                command.Parameters.AddWithValue("@rawText", encryptor.Encrypt(entry.PlainContent));
                command.Parameters.AddWithValue("@rtfText", encryptor.Encrypt(entry.RtfText));
                command.Parameters.AddWithValue("@date", DateUtils.ToUnixtime(entry.Date));
                command.Prepare();

                command.ExecuteNonQuery();
            }

            AddImagesForEntry(entry);
            // we don't have to remove the images for the entry, since the entry was just created

            entry.CommitImageChanges();
        }

        protected override void UpdateEntryImpl(DiaryEntry entry) {
            using(var command = new SqliteCommand()) {
                command.Connection = connection;
                command.CommandText = @"UPDATE entries SET title = @title, rawText = @rawText, rtfText = @rtfText WHERE date = @date;";
                command.Parameters.AddWithValue("@title", encryptor.Encrypt(entry.Title));
                command.Parameters.AddWithValue("@rawText", encryptor.Encrypt(entry.PlainContent));
                command.Parameters.AddWithValue("@rtfText", encryptor.Encrypt(entry.RtfText));
                command.Parameters.AddWithValue("@date", DateUtils.ToUnixtime(entry.Date));
                command.Prepare();

                command.ExecuteNonQuery();
            }

            AddImagesForEntry(entry);
            RemoveImagesForEntry(entry);
            entry.CommitImageChanges();
        }

        private void AddImagesForEntry(DiaryEntry entry) {
            if(entry.AddedImages.Count == 0) return;

            using(var transaction = connection.BeginTransaction()) {
                using(var command = connection.CreateCommand()) {
                    command.CommandText = @"INSERT INTO images (hash, imageData, date) VALUES (@hash, @imageData, @date);";

                    var hashParam = command.CreateParameter();
                    hashParam.ParameterName = "@hash";
                    command.Parameters.Add(hashParam);

                    var imageDataParam = command.CreateParameter();
                    imageDataParam.ParameterName = "@imageData";
                    imageDataParam.SqliteType = SqliteType.Blob;
                    command.Parameters.Add(imageDataParam);

                    command.Parameters.AddWithValue("@date", DateUtils.ToUnixtime(entry.Date));

                    foreach(StoredImage img in entry.AddedImages) {
                        hashParam.Value = img.Hash;
                        imageDataParam.Value = img.ImageData;

                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        private void RemoveImagesForEntry(DiaryEntry entry) {
            if(entry.RemovedImages.Count == 0) return;

            using(var transaction = connection.BeginTransaction()) {
                using(var command = connection.CreateCommand()) {
                    command.CommandText = @"DELETE FROM images WHERE date = @date AND hash = @hash;";

                    var hashParam = command.CreateParameter();
                    hashParam.ParameterName = "@hash";
                    command.Parameters.Add(hashParam);

                    command.Parameters.AddWithValue("@date", DateUtils.ToUnixtime(entry.Date));

                    foreach(StoredImage img in entry.RemovedImages) {
                        hashParam.Value = img.Hash;

                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        protected override DiaryEntry LoadImpl(DateTime date) {
            using(var command = new SqliteCommand()) {
                command.Connection = connection;
                command.CommandText = @"SELECT title, rawText, rtfText FROM entries WHERE date = @date;";
                command.Parameters.AddWithValue("@date", DateUtils.ToUnixtime(date));
                command.Prepare();

                using(SqliteDataReader reader = command.ExecuteReader()) {
                    if(reader.Read()) {
                        string title = encryptor.Decrypt(reader.GetString(0));
                        string rawText = encryptor.Decrypt(reader.GetString(1));
                        string rtfText = encryptor.Decrypt(reader.GetString(2));
                        DiaryEntry entry = new DiaryEntry(date, title, rawText, rtfText);
                        LoadImagesForEntry(entry);
                        return entry;
                    }
                }
            }
            return null;
        }

        private void LoadImagesForEntry(DiaryEntry e) {
            using(var command = connection.CreateCommand()) {
                command.CommandText = @"SELECT imageData FROM images WHERE date = @date;";
                command.Parameters.AddWithValue("@date", DateUtils.ToUnixtime(e.Date));
                command.Prepare();

                using(SqliteDataReader reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        byte[] imageData = (byte[]) reader[0];
                        StoredImage img = new StoredImage(imageData);
                        e.InsertImageImmediately(img);
                    }
                }
            }
        }

        protected override ObservableCollection<DiaryEntryPreview> LoadPreviewsImpl() {
            ObservableCollection<DiaryEntryPreview> entries = new ObservableCollection<DiaryEntryPreview>();

            using(var command = new SqliteCommand()) {
                command.CommandText = @"SELECT date, title, rawText FROM entries";
                command.Connection = connection;

                using(SqliteDataReader reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        long unixTime = reader.GetInt64(0);
                        DateTime date = DateUtils.FromUnixtime(unixTime);

                        // the purpose of this entry is to check if the entered password is correct, it should not be shown to the user
                        if(date == PW_CHECK_ENTRY_DATE) continue;

                        string title = encryptor.Decrypt(reader.GetString(1));
                        string rawText = encryptor.Decrypt(reader.GetString(2));
                        entries.Add(new DiaryEntryPreview(date, title, rawText));
                    }
                }
            }

            return entries;
        }

        protected override void RemoveEntryImpl(DiaryEntryPreview entry) {
            RemoveAllImagesForEntry(entry);

            using(var command = new SqliteCommand()) {
                command.CommandText = @"DELETE FROM entries WHERE date = @date;";
                command.Connection = connection;

                command.Parameters.AddWithValue("@date", DateUtils.ToUnixtime(entry.Date));
                command.Prepare();

                command.ExecuteNonQuery();
            }
        }

        private void RemoveAllImagesForEntry(DiaryEntryPreview entry) {
            using(var command = connection.CreateCommand()) {
                command.CommandText = @"DELETE FROM images WHERE date = @date;";
                command.Parameters.AddWithValue("@date", DateUtils.ToUnixtime(entry.Date));
                command.ExecuteNonQuery();
            }
        }
    }
}
