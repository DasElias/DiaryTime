using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Diary.Services {
    class DatabaseExportService {
        public DatabaseExportService(AbstractPersistorService persistorService) {
            this.PersistorService = persistorService;
        }

        private AbstractPersistorService PersistorService { get; }

        public async void Export(IStorageFile storageFile) {
            StorageFile databaseFile = PersistorService.StorageFile;
            await databaseFile.CopyAndReplaceAsync(storageFile);
        }

        public async void Import(StorageFile storageFile) {
            StorageFile databaseFile = PersistorService.StorageFile;
            await storageFile.CopyAndReplaceAsync(databaseFile);
        }

        public async Task<bool> VerifyForImport(StorageFile storageFile) {
            const string TEMP_FILE_NAME = "temp.db";

            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            IStorageItem tempStorageItem = await folder.TryGetItemAsync(TEMP_FILE_NAME);
            StorageFile tempFile = await (tempStorageItem == null ? folder.CreateFileAsync(TEMP_FILE_NAME) : folder.GetFileAsync(TEMP_FILE_NAME));
            await storageFile.CopyAndReplaceAsync(tempFile);

            AbstractEncryptor mockEncryptor = new MockEncryptionService();

            try {
                var newService = new DatabasePersistorService(mockEncryptor, tempFile.Path);
            } catch(SqliteException) {
                return false;
            } catch(InvalidPasswordException) {
                // do nothing
            }

            return true;
        }
    }
}
