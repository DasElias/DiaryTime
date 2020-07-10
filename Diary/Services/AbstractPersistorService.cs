using Diary.Model;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Diary.Services {
    abstract class AbstractPersistorService {
        private ObservableCollection<DiaryEntryPreview> _diaryEntries = null;

        public ObservableCollection<DiaryEntryPreview> DiaryEntries {
            get {
                if(_diaryEntries == null) {
                    _diaryEntries = LoadPreviewsImpl();
                }
                return _diaryEntries;
            }
        }

        public bool ContainsEntryForDate(DateTime date) {
            return DiaryEntries.Any(entry => DateUtils.CompareDay(entry.Date, date));
        }

        public DiaryEntry LoadEntry(DiaryEntryPreview preview) {
            return LoadEntry(preview.Date);
        }

        public DiaryEntry LoadEntry(DateTime date) {
            return LoadImpl(date);
        }

        public void SaveEntryDraft(DiaryEntry entry) {
            SaveImpl(entry);
        }

        public void SaveEntryFinal(DiaryEntry entry) {
            if(ShouldSaveEntry(entry)) SaveImpl(entry);
            else RemoveEntry(entry);
        }

        private void SaveImpl(DiaryEntry entry) {
            for(int i = 0; i < DiaryEntries.Count; i++) {
                if(DateUtils.CompareDay(entry.Date, DiaryEntries[i].Date)) {
                    UpdateEntryImpl(entry);
                    DiaryEntries[i] = entry;
                    return;
                }
            }

            CreateEntryImpl(entry);
            DiaryEntries.Add(entry);
        }

        private bool ShouldSaveEntry(DiaryEntry entry) {
            string trimmedText = entry.PlainContent.Trim();
            bool hasText = trimmedText.Length > 0;
            bool hasImages = entry.StoredImages.Count + entry.AddedImages.Count - entry.RemovedImages.Count > 0;
            return hasText || hasImages;
        }

        public void RemoveEntry(DiaryEntryPreview entry) {
            bool wasSuccessfullyRemoved = DiaryEntries.Remove(entry);
            if(wasSuccessfullyRemoved) RemoveEntryImpl(entry);
        }

        public abstract void Export(IStorageFile storageFile);
        public abstract void Import(StorageFile storageFile);

        protected abstract ObservableCollection<DiaryEntryPreview> LoadPreviewsImpl();
        protected abstract DiaryEntry LoadImpl(DateTime date);

        protected abstract void CreateEntryImpl(DiaryEntry entry);
        protected abstract void UpdateEntryImpl(DiaryEntry entry);

        protected abstract void RemoveEntryImpl(DiaryEntryPreview entry);

    }
}
