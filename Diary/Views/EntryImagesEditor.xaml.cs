using Diary.Model;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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

        private ObservableCollection<ImageWrapper> imagesToDisplay = new ObservableCollection<ImageWrapper>();
        private List<StoredImage> addedImages = new List<StoredImage>();
        private List<StoredImage> removedImages = new List<StoredImage>();
        private bool isEditable;

        public EntryImagesEditor() {
            this.InitializeComponent();
            IsEditable = false;
        }

        public bool IsEditable {
            get {
                return isEditable;
            }
            set {
                isEditable = value;
                string key = isEditable ? "EditableTemplate" : "NonEditableTemplate";
                ImagesListView.ItemTemplate = (DataTemplate) Resources[key];
            }
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
                ImageWrapper imageWrapper = new ImageWrapper() {
                    StoredImage = img,
                    BitmapImage = bitmapImage
                };

                imagesToDisplay.Add(imageWrapper);
            }
        }

        public void ClearImageChanges() {
            addedImages.Clear();
            removedImages.Clear();
        }

        public void Clear() {
            imagesToDisplay.Clear();
            addedImages.Clear();
            removedImages.Clear();
        }

        private async void HandleImageList_ItemClick(object sender, ItemClickEventArgs e) {
            ImageWrapper img = (ImageWrapper) e.ClickedItem;
            int index = imagesToDisplay.IndexOf(img);
            ContentDialog c = new ImageViewContentDialog(imagesToDisplay, index);
            await c.ShowAsync();
        }

        private void HandleImageList_DragOver(object sender, DragEventArgs e) {
            if(IsEditable) {
                e.AcceptedOperation = DataPackageOperation.Copy;

                if(e.DragUIOverride != null) {
                    e.DragUIOverride.Caption = "Hinzufügen";
                    e.DragUIOverride.IsContentVisible = true;
                }
            }
        }

        private async void HandleImageList_Drop(object sender, DragEventArgs e) {
            if(e.DataView.Contains(StandardDataFormats.StorageItems)) {
                var items = await e.DataView.GetStorageItemsAsync();
                var storageFiles = new List<StorageFile>();
                foreach(IStorageItem i in items) {
                    var casted = i as StorageFile;
                    if(casted != null) storageFiles.Add(casted);
                }

                await InsertImages(storageFiles);
            }
        }

        private async void HandleAddImageBtn_Tapped(object sender, TappedRoutedEventArgs e) {
            Windows.Storage.Pickers.FileOpenPicker open = new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".png");
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".jpeg");
            open.FileTypeFilter.Add(".bmp");
            open.FileTypeFilter.Add(".gif");

            IReadOnlyList<StorageFile> allFiles = await open.PickMultipleFilesAsync();
            await InsertImages(allFiles);
        }

        private async Task InsertImages(IReadOnlyList<StorageFile> allFiles) {
            foreach(StorageFile file in allFiles) {
                byte[] imageData = await StorageFileToByteArrayHelper.GetBytesAsync(file);
                StoredImage image = new StoredImage(imageData);

                if(Contains(image)) {
                    await DisplayImageAlreadyExistsWarning(file.DisplayName);
                } else {
                    BitmapImage bitmapImage;
                    try {
                        bitmapImage = await ByteArrayToBitmapImageHelper.ConvertByteArrayToBitmapImage(imageData);
                    } catch(Exception) {
                        await DisplayInvalidImageError(file.DisplayName);
                        continue;
                    }

                    bool wasRemovedBefore = removedImages.Remove(image);
                    if(!wasRemovedBefore) {
                        // if the image was removed before, it exists already in the database
                        addedImages.Add(image);
                    }

                    ImageWrapper imageWrapper = new ImageWrapper() {
                        BitmapImage = bitmapImage,
                        StoredImage = image
                    };
                    imagesToDisplay.Add(imageWrapper);
                }
            }
        }

        private bool Contains(StoredImage image) {
            return imagesToDisplay.FirstOrDefault(i => i.StoredImage == image) != null;
        }

        private void HandleRemoveImageBtn_Click(object sender, RoutedEventArgs e) {
            ImageWrapper imageWrapper = (ImageWrapper) ((Button) sender).Tag;
            imagesToDisplay.Remove(imageWrapper);

            bool wasAddedBefore = addedImages.Remove(imageWrapper.StoredImage);
            if(!wasAddedBefore) {
                // if the image was already added before, it doesn't exist in the database yet
                removedImages.Add(imageWrapper.StoredImage);
            }
        }

        private async Task DisplayImageAlreadyExistsWarning(string filename) {
            ContentDialog dialog = new ContentDialog() {
                Title = "Fehler",
                Content = $"Das Bild \"{filename}\" wurde bereits einmal zum Tagebucheintrag hinzugefügt.",
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }

        private async Task DisplayInvalidImageError(string filename) {
            ContentDialog dialog = new ContentDialog() {
                Title = "Fehler",
                Content = $"Bei der Datei \"{filename}\" handelt es sich nicht um eine gültige Bilddatei.",
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }


    }
}
