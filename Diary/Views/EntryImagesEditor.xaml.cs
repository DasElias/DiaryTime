using Diary.Model;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Diary.Views {
    public sealed partial class EntryImagesEditor : UserControl {
        private ObservableCollection<BitmapImage> imagesToDisplay = new ObservableCollection<BitmapImage>();
        private HashSet<StoredImage> storedImages = new HashSet<StoredImage>();
        private List<StoredImage> addedImages = new List<StoredImage>();
        private List<StoredImage> removedImages = new List<StoredImage>();

        public EntryImagesEditor() {
            this.InitializeComponent();
        }

        public ReadOnlyCollection<StoredImage> AddedImages {
            get {
                return addedImages.AsReadOnly();
            }
        }
        public ReadOnlyCollection<StoredImage> RemovedImages {
            get {
                return removedImages.AsReadOnly();
            }
        }

        public async Task LoadImages(ReadOnlyCollection<StoredImage> images) {
            foreach(StoredImage img in images) {
                BitmapImage bitmapImage = await ByteArrayToBitmapImageHelper.ConvertByteArrayToBitmapImage(img.ImageData);
                imagesToDisplay.Add(bitmapImage);

                storedImages.Add(img);
            }
        }

        public void ClearImageChanges() {
            addedImages.Clear();
            removedImages.Clear();
        }

        public void Clear() {
            imagesToDisplay.Clear();
            storedImages.Clear();
            addedImages.Clear();
            removedImages.Clear();
        }

        private async void HandleImageList_ItemClick(object sender, ItemClickEventArgs e) {
            BitmapImage img = (BitmapImage) e.ClickedItem;
            ContentDialog c = new ImageViewContentDialog(img);
            await c.ShowAsync();
        }

        private async void HandleAddImageBtn_Tapped(object sender, TappedRoutedEventArgs e) {
            Windows.Storage.Pickers.FileOpenPicker open = new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".png");
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".jpeg");

            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();
            if(file != null) {
                byte[] imageData = await StorageFileToByteArrayHelper.GetBytesAsync(file);
                StoredImage image = new StoredImage(imageData);

                if(storedImages.Contains(image)) {
                    await DisplayImageAlreadyExistsWarning();
                } else {
                    addedImages.Add(image);
                    storedImages.Add(image);

                    BitmapImage bitmapImage = await ByteArrayToBitmapImageHelper.ConvertByteArrayToBitmapImage(imageData);
                    imagesToDisplay.Add(bitmapImage);
                }
            }
        }

        private async Task DisplayImageAlreadyExistsWarning() {
            ContentDialog dialog = new ContentDialog() {
                Title = "Fehler",
                Content = "Das Bild wurde bereits einmal zum Tagebucheintrag hinzugefügt.",
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }
    }
}
