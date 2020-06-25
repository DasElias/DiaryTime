using Diary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Diary.Services {
    class EmptyDiaryPersistorService : AbstractPersistorService {
        private EncryptionService encryptor;

        public EmptyDiaryPersistorService(EncryptionService encryptor) {
            this.encryptor = encryptor;
        }

        public override void Export(IStorageFile storageFile) {
            
        }

        public override void Import(StorageFile storageFile) {

        }

        protected override void CreateEntryImpl(DiaryEntry entry) {

        }

        protected override DiaryEntry LoadImpl(DateTime date) {
            return new DiaryEntry(date);
        }

        protected override ObservableCollection<DiaryEntryPreview> LoadPreviewsImpl() {
            return new ObservableCollection<DiaryEntryPreview>();
        }

        protected override void RemoveEntryImpl(DiaryEntryPreview entry) {

        }

        protected override void UpdateEntryImpl(DiaryEntry entry) {

        }
    }
}
