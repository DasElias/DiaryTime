using Diary.Events;
using Diary.Model;
using Diary.Services;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        private AbstractPersistorService persistorService;
        private DiaryEntry entry;
        private bool wasFirstLoaded = false;

        public ViewEntryPage() {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

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
            UpdateContent();
            wasFirstLoaded = true;
        }


        private void UpdateContent() {
            entryButtonBarControl.DateText = entry.DateString;
            titleElement.Text = entry.Title;

            contentElement.IsReadOnly = false;
            contentElement.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, entry.RtfText);
            contentElement.Document.ApplyDisplayUpdates();
            contentElement.IsReadOnly = true;
        }

        private async void HandleEditBtn_Click(object sender, RoutedEventArgs e) {
            if(entry.IsToday) {
                StartEdit();
                return;
            }

            ContentDialog editConfirmationDialog = new ContentDialog {
                Title = "Eintrag bearbeiten",
                Content = $"Bist du dir sicher, dass du diesen Tagebucheintrag für einen vergangenen Tag ({entry.DateString}) bearbeiten willst?",
                PrimaryButtonText = "Abbrechen",
                SecondaryButtonText = "Fortfahren"
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
