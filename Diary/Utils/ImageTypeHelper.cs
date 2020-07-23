using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Diary.Utils {
    enum ImageType {
        PNG,
        JPEG,
        GIF,
        BMP,
        TIFF
    }

    static class ImageTypeHelper {
        private static readonly byte[] BMP_BEGIN = Encoding.ASCII.GetBytes("BM");
        private static readonly byte[] GIF_BEGIN = Encoding.ASCII.GetBytes("GIF");
        private static readonly byte[] PNG_BEGIN = new byte[] { 137, 80, 78, 71 };
        private static readonly byte[] TIFF_BEGIN = new byte[] { 73, 73, 42, 0 };
        private static readonly byte[] TIFF2_BEGIN = new byte[] { 77, 77, 0, 42 };
        private static readonly byte[] JPEG_BEGIN = new byte[] { 255, 216, 255 };

        public static string DetermineImageExtension(byte[] imageData) {
            ImageType? type = DetermineImageType(imageData);
            if(type == null) return null;

            string enumAsText = type.Value.ToString("G");
            return enumAsText.ToLower();
        }

        public static ImageType? DetermineImageType(byte[] imageData) {
            if(BeginsWith(imageData, BMP_BEGIN)) return ImageType.BMP;
            if(BeginsWith(imageData, GIF_BEGIN)) return ImageType.GIF;
            if(BeginsWith(imageData, PNG_BEGIN)) return ImageType.PNG;
            if(BeginsWith(imageData, TIFF_BEGIN) || BeginsWith(imageData, TIFF2_BEGIN)) return ImageType.TIFF;
            if(BeginsWith(imageData, JPEG_BEGIN)) return ImageType.JPEG;

            return null;
        }

        private static bool BeginsWith(byte[] imageData, byte[] magicValue) {
            if(imageData.Length < magicValue.Length) return false;

            for(int i = 0; i < magicValue.Length; i++) {
                if(imageData[i] != magicValue[i]) return false;
            }

            return true;
        }
    }
}
