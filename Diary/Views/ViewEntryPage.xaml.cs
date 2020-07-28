using Diary.Events;
using Diary.Model;
using Diary.Services;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Diary.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewEntryPage : Page {
        private ResourceLoader resourceLoader;

        private AbstractPersistorService persistorService;
        private DiaryEntry entry;
        private bool wasFirstLoaded = false;

        public ViewEntryPage() {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            resourceLoader = ResourceLoader.GetForCurrentView();

            this.Loaded += Page_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            EntryArgument arg = (EntryArgument) e.Parameter;
            persistorService = arg.PersistorService;
            entry = arg.Entry;

            if(wasFirstLoaded) {
                UpdateContent();
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            if(! wasFirstLoaded) {
                UpdateContent();
                wasFirstLoaded = true;
            }
        }


        private void UpdateContent() {
            entryButtonBarControl.DateText = entry.DateString;
            titleElement.Text = entry.Title;

            contentElement.IsReadOnly = false;
            contentElement.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, entry.RtfText);
            contentElement.Document.ApplyDisplayUpdates();
            contentElement.IsReadOnly = true;

            bool hasEntryImages = entry.StoredImages.Count > 0;
            entryImagesEditor.Visibility = hasEntryImages ? Visibility.Visible : Visibility.Collapsed;
            if(hasEntryImages) {
                entryImagesEditor.Clear();
                _ = entryImagesEditor.LoadImages(entry.StoredImages);
            }

        }

        private async void HandleEditBtn_Click(object sender, RoutedEventArgs e) {
            if(entry.IsToday) {
                StartEdit();
                return;
            }

            ContentDialog editConfirmationDialog = new ContentDialog {
                Title = resourceLoader.GetString("editEntry"),
                Content = string.Format(resourceLoader.GetString("confirmEditPastDay"), entry.DateString),
                PrimaryButtonText = resourceLoader.GetString("abort"),
                SecondaryButtonText = resourceLoader.GetString("continue")
            };

            ContentDialogResult result = await editConfirmationDialog.ShowAsync();
            if(result == ContentDialogResult.Secondary) {
                StartEdit();
            }
        }

        private void StartEdit() {
            EntryArgument arg = new EntryArgument(entry, persistorService);
            Frame.Navigate(typeof(EditPage), arg);
        }
    }
}
