using Diary.Events;
using Diary.Model;
using Diary.Services;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Diary.Views {
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class NoEntryForDayPage : Page {
        private ResourceLoader resourceLoader;

        private AbstractPersistorService persistor;
        private DateTime entryDate;
        private bool isForToday = false;

        private string messageText;

        public NoEntryForDayPage() {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.resourceLoader = ResourceLoader.GetForCurrentView();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            DateArgument arg = (DateArgument) e.Parameter;
            persistor = arg.PersistorService;
            entryDate = arg.Date;
            isForToday = DateUtils.IsToday(entryDate);
            UpdateMessageText();

            // because page is cached
            Bindings.Update();
        }

        private void UpdateMessageText() {
            if(isForToday) {
                messageText = resourceLoader.GetString("noEntryWasCreatedToday");
            } else {
                messageText = string.Format(resourceLoader.GetString("noEntryWasCreatedOn"), DateUtils.ToDateString(entryDate));
            }
        }

        private async void HandleCreateNowBtn_Click(object sender, RoutedEventArgs e) {
            if(isForToday) {
                CreateForToday();
            } else {
                await CreateForPast();
            }
        }

        private async Task CreateForPast() {
            ContentDialog editConfirmationDialog = new ContentDialog {
                Title = resourceLoader.GetString("dayIsInPast"),
                Content = string.Format(resourceLoader.GetString("dayIsInPastBody"), DateUtils.ToDateString(entryDate)),
                PrimaryButtonText = resourceLoader.GetString("abort"),
                SecondaryButtonText = resourceLoader.GetString("continue")
            };

            ContentDialogResult result = await editConfirmationDialog.ShowAsync();
            if(result == ContentDialogResult.Secondary) {
                Navigate();
            }
        }

        private void CreateForToday() {
            Navigate();
        }

        private void Navigate() {
            string title = resourceLoader.GetString("diaryEntryOf") + " " + DateUtils.ToDateString(entryDate);
            DiaryEntry entry = new DiaryEntry(entryDate, title);

            Frame.Navigate(typeof(EditPage), new EntryArgument(entry, persistor));
        }
    }
}
