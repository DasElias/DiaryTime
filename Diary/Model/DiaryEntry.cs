using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Model {
    public class DiaryEntry : DiaryEntryPreview {
        private List<StoredImage> images = new List<StoredImage>();
        private List<StoredImage> imagesToRemove = new List<StoredImage>();
        private List<StoredImage> imagesToAdd = new List<StoredImage>();

        public DiaryEntry(DateTime date, string title) : base(date, title) {
        }

        public DiaryEntry(DateTime date, string title, string plainText, string rtfText) : base(date, title, plainText) {
            RtfText = rtfText;
        }

        public string RtfText { get; private set; }

        public ReadOnlyCollection<StoredImage> StoredImages {
            get {
                return images.AsReadOnly();
            }
        }

        public ReadOnlyCollection<StoredImage> AddedImages {
            get {
                return imagesToAdd.AsReadOnly();
            }
        }

        public ReadOnlyCollection<StoredImage> RemovedImages {
            get {
                return imagesToRemove.AsReadOnly();
            }
        }

        public override void SetText(string plainText, string rtfText) {
            base.SetText(plainText, rtfText);
            RtfText = rtfText;
        }

        public void UpdateImages(ReadOnlyCollection<StoredImage> newImagesToAdd, ReadOnlyCollection<StoredImage> newImagesToRemove) {
            if(imagesToRemove.Count > 0 || imagesToAdd.Count > 0) {
                throw new InvalidOperationException("UpdateImages has been called when there were image changes registered already.");
            }

            imagesToAdd.AddRange(newImagesToAdd);
            imagesToRemove.AddRange(newImagesToRemove);
        }

        public void InsertImageImmediately(StoredImage i) {
            images.Add(i);
        }

        public void CommitImageChanges() {
            foreach(StoredImage i in imagesToRemove) {
                bool wasSuccessful = this.images.Remove(i);
                if(!wasSuccessful) throw new InvalidOperationException("Tried to remove an non-existing image.");
            }

            foreach(StoredImage i in imagesToAdd) {
                this.images.Add(i);
            }

            imagesToAdd.Clear();
            imagesToRemove.Clear();
        }


    }
}
