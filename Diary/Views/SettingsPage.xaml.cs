using Diary.Events;
using Diary.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Diary.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page {
        private AbstractPersistorService persistorService;
        private AbstractEncryptor encryptor;

        public SettingsPage() {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            shouldNotifyCheckBox.IsChecked = ReminderTimeSaveService.ShouldReminder();
            shouldNotifyTimePicker.Time = ReminderTimeSaveService.GetReminderTime();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            PersistorEncryptorArgument arg = (PersistorEncryptorArgument) e.Parameter;
            this.encryptor = arg.Encryptor;
            this.persistorService = arg.Persistor;
        }

        private async void HandleChangePasswordBtn_Click(object sender, RoutedEventArgs e) {
            ChangePasswordDialog dialog = new ChangePasswordDialog(encryptor.PlainPassword);
            await dialog.ShowAsync();

            if(dialog.Result == ChangePasswortResult.SUCCESSFULLY_CHANGED) {
                string newPassword = dialog.NewPassword;
                encryptor.PlainPassword = newPassword;
            }
        }

        private void HandleCheckBox_Click(object sender, RoutedEventArgs e) {
            ReminderTimeSaveService.SetReminderTime(shouldNotifyCheckBox.IsChecked.GetValueOrDefault(), shouldNotifyTimePicker.Time);
        }

        private void HandleTimePicker_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args) {
            ReminderTimeSaveService.SetReminderTime(shouldNotifyCheckBox.IsChecked.GetValueOrDefault(), shouldNotifyTimePicker.Time);
        }

        private async void HandleExportBtn_Click(object sender, RoutedEventArgs e) {
            CheckPasswordDialog contentDialog = new CheckPasswordDialog("Bitte gib dein Passwort ein.", encryptor.PlainPassword) {
                Title = "Tagebucheinträge exportieren",
                PrimaryButtonText = "Weiter",
                SecondaryButtonText = "Abbrechen"

            };
            ContentDialogResult result = await contentDialog.ShowAsync();
            if(result != ContentDialogResult.Primary) return;

            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("DiaryTime-Export", new List<string>() { ".dt" });
            savePicker.SuggestedFileName = "DiaryTimeExport";
            StorageFile file = await savePicker.PickSaveFileAsync();

            if(file != null) {
                CachedFileManager.DeferUpdates(file);
                persistorService.Export(file);
                await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        private async void HandleImportBtn_Click(object sender, RoutedEventArgs e) {
            CheckPasswordDialog contentDialog = new CheckPasswordDialog("Bitte bestätige durch Eingabe deines Passworts, dass du deine momentanen Tagebucheinträge durch die zu importierenden ersetzen möchtest.\n\nDie aktuellen Tagebucheinträge werden hierdurch unwiderruflich gelöscht werden.", encryptor.PlainPassword) {
                Title = "Tagebucheinträge importieren",
                PrimaryButtonText = "Weiter",
                SecondaryButtonText = "Abbrechen"

            };
            ContentDialogResult result = await contentDialog.ShowAsync();
            if(result != ContentDialogResult.Primary) return;

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".dt");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if(file == null) return;


            ContentDialog successDialog = new ContentDialog() {
                Title = "Import abgeschlossen",
                Content = "Deine bisherigen Tagebucheinträge wurden mit den zu importierenden Einträgen überschrieben. DiaryTime wird nun neugestartet.",
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Rückgängig machen"
            };
            result = await successDialog.ShowAsync();
            if(result != ContentDialogResult.Primary) return;

            persistorService.Import(file);
            await CoreApplication.RequestRestartAsync("");
        }
    }
}
