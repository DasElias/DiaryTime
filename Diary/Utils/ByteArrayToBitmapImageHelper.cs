using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Diary.Utils {
    static class ByteArrayToBitmapImageHelper {
        public static async Task<BitmapImage> ConvertByteArrayToBitmapImage(byte[] imageData) {
            BitmapImage image = new BitmapImage();
            using(InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream()) {
                await stream.WriteAsync(imageData.AsBuffer());
                stream.Seek(0);
                await image.SetSourceAsync(stream);
            }
            return image;
        }
    }
}
