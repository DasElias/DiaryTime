using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Background;
using Windows.UI.Xaml.Media;

namespace Diary.Model {
    class ImageViewModel : BindableBase {
        private ICollection<ImageWrapper> allImages;
        private int index;

        public ImageViewModel(ICollection<ImageWrapper> allImages, int index) {
            this.allImages = allImages;
            this.index = index;
        }

        private int Index {
            get {
                return index;
            }
            set {
                index = value;
                OnPropertyChanged(nameof(ImageSource));
                OnPropertyChanged(nameof(CanNavigateLeft));
                OnPropertyChanged(nameof(CanNavigateRight));
            }
        }

        public bool CanNavigateLeft {
            get {
                return Index > 0;
            }
        }

        public bool CanNavigateRight {
            get {
                return Index < (allImages.Count - 1);
            }
        }

        public ImageSource ImageSource {
            get {
                return allImages.ElementAt(Index).BitmapImage;
            }
        }

        public void NavigateLeft() {
            if(CanNavigateLeft) Index--;
        }
        
        public void NavigateRight() {
            if(CanNavigateRight) Index++;
        }
    }
}
