using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Diary.Utils {
    static class StorageFileToByteArrayHelper {
        public static async Task<byte[]> GetBytesAsync(StorageFile file) {
            byte[] fileBytes = null;
            if(file == null) return null;
            using(var stream = await file.OpenReadAsync()) {
                fileBytes = new byte[stream.Size];
                using(var reader = new DataReader(stream)) {
                    await reader.LoadAsync((uint) stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }
    }
}
