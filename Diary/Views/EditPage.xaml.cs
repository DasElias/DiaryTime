using Diary.Events;
using Diary.Model;
using Diary.Services;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI.Core;
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
    public sealed partial class EditPage : NoDoubleClickPage {
        private DispatcherTimer saveTimer = new DispatcherTimer() {
            Interval = TimeSpan.FromSeconds(90)
        };
        private ResourceLoader resourceLoader;

        private AbstractPersistorService persistorService;
        private DiaryEntry diaryEntry = null;
        private int changedCharactersCount = 0;
        private bool wasEntryLoaded = false;

        public EditPage() {
            this.InitializeComponent();
            resourceLoader = ResourceLoader.GetForCurrentView();

            this.Loaded += Page_Loaded;
            Application.Current.Suspending += Page_Suspending;
            saveTimer.Tick += OnAutoSave;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            // if the page wasn't unloaded in the meantime
            if(diaryEntry != null) {
                editor.EntryChanged += HandleEditor_EntryChanged;

                entryButtonBarControl.DateText = diaryEntry.DateString;
                editor.Title = diaryEntry.Title;
                if(diaryEntry.RtfText != null && diaryEntry.PlainContent.Trim().Length > 0) {
                    editor.RtfText = diaryEntry.RtfText;
                } else {
                    editor.Clear();
                }

                _ = editor.LoadImages(diaryEntry.StoredImages);

                bool isSpellCheckingEnabled = SpellCheckingSettingService.GetSpellCheckingPreference();
                spellCheckingToggleButton.IsChecked = isSpellCheckingEnabled;
                editor.IsSpellCheckingEnabled = isSpellCheckingEnabled;

                wasEntryLoaded = true;
            }
        }

        private void Page_Suspending(object sender, SuspendingEventArgs e) {
            SaveFinal();
        }

        private void OnAutoSave(object sender, object e) {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                bool wasntUnloadedInMeantime = diaryEntry != null;
                bool wasChangedEnough = changedCharactersCount > 20 || editor.ImageChangesCount > 0;

                if(wasntUnloadedInMeantime && wasChangedEnough) {
                    changedCharactersCount = 0;
                    UpdateEntry();
                    persistorService.SaveEntryDraft(diaryEntry);
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            EntryArgument arg = (EntryArgument) e.Parameter;
            persistorService = arg.PersistorService;
            diaryEntry = arg.Entry;
            changedCharactersCount = 0;

            saveTimer.Start();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            saveTimer.Stop();

            SaveFinal();
        }

        private void SaveFinal() {
            if(diaryEntry != null) {
                if(wasEntryLoaded) {
                    UpdateEntry();
                    persistorService.SaveEntryFinal(diaryEntry);
                    wasEntryLoaded = false;
                }
                diaryEntry = null;
            }
        }
             
        private void HandleEditor_EntryChanged(object sender, RoutedEventArgs e) {
            changedCharactersCount++;
        }


        private void UpdateEntry() {
            diaryEntry.Title = editor.Title;
            diaryEntry.SetText(editor.RawText, editor.RtfText);
            editor.UpdateEntryWithImageChanges(diaryEntry);
        }

        private void HandleViewButton_Click(object sender, RoutedEventArgs e) {
            if(!IsReadyToPressButton()) return;

            EntryArgument arg = new EntryArgument(diaryEntry, persistorService);
            Frame.Navigate(typeof(ViewEntryPage), arg);
        }

        private async void HandleDeleteButton_Click(object sender, RoutedEventArgs e) {
            if(!IsReadyToPressButton()) return;
            using(var btnLock = new DoubleClickPreventer(sender)) {
                ContentDialog confirmationDialog = new ContentDialog {
                    Title = resourceLoader.GetString("deleteEntryLong"),
                    Content = resourceLoader.GetString("confirmDeleteEntry"),
                    PrimaryButtonText = resourceLoader.GetString("abort"),
                    SecondaryButtonText = resourceLoader.GetString("delete")
                };

                ContentDialogResult result = await confirmationDialog.ShowAsync();
                if(result == ContentDialogResult.Secondary) {
                    UpdateEntry();

                    // before deleting, we set diaryEntry to null to prevent an autosave in meantime
                    DiaryEntry temp = diaryEntry;
                    diaryEntry = null;

                    persistorService.RemoveEntry(temp);

                    EntryArgument entryArg = new EntryArgument(temp, persistorService);
                    Frame.Navigate(typeof(EntryWasDeletedPage), entryArg);
                }
            }
        }

        private void HandleSpellCheckingButton_Click(object sender, RoutedEventArgs e) {
            if(!IsReadyToPressButton()) return;

            AppBarToggleButton btn = (AppBarToggleButton) sender;
            bool isChecked = btn.IsChecked.GetValueOrDefault();
            editor.IsSpellCheckingEnabled = isChecked;

            SpellCheckingSettingService.SetSpellCheckingPreference(isChecked);
        }
    }


}
