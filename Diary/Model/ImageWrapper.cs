using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Diary.Model {
    public class ImageWrapper {
        public StoredImage StoredImage { get; set; }
        public BitmapImage BitmapImage { get; set; }
    }
}
