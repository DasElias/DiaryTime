using Diary.Utils;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Diary.Model {
    public class StoredImage {
        private byte[] imageData;
        private string hash = null;

        public StoredImage(byte[] imageData) {
            this.imageData = imageData;
        }

        public StoredImage(byte[] imageData, string hash) {
            this.imageData = imageData;
            this.hash = hash;
        }

        public byte[] ImageData {
            get {
                return imageData;
            }
        }

        public string Hash {
            get {
                if(hash == null) {
                    hash = ByteArrayHasher.Hash(imageData);
                }
                return hash;
            }
        }

        public override bool Equals(object obj) {
            return obj is StoredImage image &&
                   Hash == image.Hash;
        }

        public override int GetHashCode() {
            return -1545866855 + EqualityComparer<string>.Default.GetHashCode(Hash);
        }

        private async Task<BitmapImage> LoadBitmapImage() {
            return await BitmapImageToByteArrayHelper.ConvertByteArrayToBitmapImage(imageData);
        }

        public static bool operator ==(StoredImage left, StoredImage right) {
            return EqualityComparer<StoredImage>.Default.Equals(left, right);
        }

        public static bool operator !=(StoredImage left, StoredImage right) {
            return !(left == right);
        }
    }
}
