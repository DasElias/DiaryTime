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
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Diary.Views {
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class NoEntryForDayPage : Page {

        private AbstractPersistorService persistor;
        private DateTime entryDate;
        private bool isForToday = false;

        private string messageText;

        public NoEntryForDayPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            DateArgument arg = (DateArgument) e.Parameter;
            persistor = arg.PersistorService;
            entryDate = arg.Date;
            isForToday = DateUtils.IsToday(entryDate);
            UpdateMessageText();
        }

        private void UpdateMessageText() {
            if(isForToday) {
                messageText = "Du hast heute noch keinen Tagebucheintrag erstellt.";
            } else {
                messageText = "Du hast am " + DateUtils.ToDateString(entryDate) + " noch keinen Tagebucheintrag erstellt.";
            }
        }

        private void HandleCreateNowBtn_Click(object sender, RoutedEventArgs e) {
            if(isForToday) {
                CreateForToday();
            } else {
                CreateForPast();
            }
        }

        private async void CreateForPast() {
            ContentDialog editConfirmationDialog = new ContentDialog {
                Title = "Tag liegt in der Vergagenheit",
                Content = "Bist du dir sicher, dass du einen Tagebucheintrag für einen Tag in der Vergangenheit (" + DateUtils.ToDateString(entryDate) + ") erstellen möchtest?",
                PrimaryButtonText = "Abbrechen",
                SecondaryButtonText = "Fortfahren"
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
            DiaryEntry entry = new DiaryEntry(entryDate);
            Frame.Navigate(typeof(EditPage), new EntryArgument(entry, persistor));
        }
    }
}
