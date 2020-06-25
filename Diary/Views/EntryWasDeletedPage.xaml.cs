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
    public sealed partial class EntryWasDeletedPage : Page {

        private AbstractPersistorService persistor;
        private DiaryEntry diaryEntry;

        private string messageText;

        public EntryWasDeletedPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            EntryArgument arg = (EntryArgument) e.Parameter;
            diaryEntry = arg.Entry;
            persistor = arg.PersistorService;

            UpdateMessageText();
        }

        private void UpdateMessageText() {
            messageText = $"Der Tagebucheintrag vom {diaryEntry.DateString} wurde gelöscht.";
        }

        private void HandleUndoBtn_Click(object sender, RoutedEventArgs e) {
            persistor.SaveEntryDraft(diaryEntry);

            EntryArgument arg = new EntryArgument(diaryEntry, persistor);
            Frame.Navigate(typeof(EditPage), arg);
        }
    }
}
