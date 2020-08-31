using Diary.Events;
using Diary.Model;
using Diary.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
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
        private ResourceLoader resourceLoader;

        private FontSizeService fontSizeService = new FontSizeService();
        private bool isDefaultFontPickerInitialized = false;

        private AbstractPersistorService persistorService;
        private AbstractEncryptor encryptor;

        public SettingsPage() {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
            resourceLoader = ResourceLoader.GetForCurrentView();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            shouldNotifyCheckBox.IsChecked = ReminderTimeSaveService.ShouldReminder();
            shouldNotifyTimePicker.Time = ReminderTimeSaveService.GetReminderTime();

            InitDefaultFontPicker();
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
            CheckPasswordDialog contentDialog = new CheckPasswordDialog(resourceLoader.GetString("pleaseEnterPassword"), encryptor.PlainPassword) {
                Title = resourceLoader.GetString("exportDiaryEntries"),
                PrimaryButtonText = resourceLoader.GetString("next"),
                SecondaryButtonText = resourceLoader.GetString("abort")

            };
            ContentDialogResult result = await contentDialog.ShowAsync();
            if(result != ContentDialogResult.Primary) return;

            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add(resourceLoader.GetString("diaryTimeExport"), new List<string>() { ".dt" });
            savePicker.SuggestedFileName = "export";
            StorageFile file = await savePicker.PickSaveFileAsync();

            if(file != null) {
                CachedFileManager.DeferUpdates(file);
                persistorService.Export(file);
                await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        private async void HandleImportBtn_Click(object sender, RoutedEventArgs e) {
            CheckPasswordDialog contentDialog = new CheckPasswordDialog(resourceLoader.GetString("confirmImportDescription"), encryptor.PlainPassword) {
                Title = resourceLoader.GetString("importDiaryEntries"),
                PrimaryButtonText = resourceLoader.GetString("next"),
                SecondaryButtonText = resourceLoader.GetString("abort")

            };
            ContentDialogResult result = await contentDialog.ShowAsync();
            if(result != ContentDialogResult.Primary) return;

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".dt");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if(file == null) return;

            bool isValid = await persistorService.VerifyForImport(file);
            if(!isValid) {
                ContentDialog invalidDialog = new ContentDialog() {
                    Title = resourceLoader.GetString("importFailed"),
                    Content = resourceLoader.GetString("importFailedDescription"),
                    PrimaryButtonText = resourceLoader.GetString("ok")
                };
                await invalidDialog.ShowAsync();
                return;
            }


            ContentDialog successDialog = new ContentDialog() {
                Title = resourceLoader.GetString("importSuccessful"),
                Content = resourceLoader.GetString("importSuccessfulDescription"),
                PrimaryButtonText = resourceLoader.GetString("ok"),
                SecondaryButtonText = resourceLoader.GetString("undo")
            };
            result = await successDialog.ShowAsync();
            if(result != ContentDialogResult.Primary) return;

            persistorService.Import(file);
            await CoreApplication.RequestRestartAsync("");
        }

        private async void HandleSendMailBtn_Click(object sender, RoutedEventArgs e) {
            EmailMessage msg = new EmailMessage();
            EmailRecipient recipient = new EmailRecipient("diarytime@icloud.com", "DiaryTime");
            msg.To.Add(recipient);

            await EmailManager.ShowComposeNewEmailAsync(msg);
        }

        private void InitDefaultFontPicker() {
            FontFamilyOptions fontFamilies = new FontFamilyOptions();
            foreach(FontFamily ff in fontFamilies) {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = ff;
                item.FontFamily = ff;
                fontFamilyBox.Items.Add(item);
            }

            DefaultFont defaultFont = DefaultFontSaveService.GetDefaultFont();
            fontSizeBox.SelectedValue = defaultFont.FontSize;
            fontFamilyBox.SelectedIndex = fontFamilies.IndexOf(fontFamilies.Find(defaultFont.FontFamily));

            isDefaultFontPickerInitialized = true;
        }

        private void HandleFontFamilyBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(isDefaultFontPickerInitialized) {
                UpdateDefaultFont();
            }

        }

        private void HandleFontSizeBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(isDefaultFontPickerInitialized) {
                UpdateDefaultFont();
            }

        }

        private void UpdateDefaultFont() {
            var selectedFontFamilyWrapper = (fontFamilyBox.SelectedItem as ComboBoxItem)?.Content as FontFamilyWrapper;
            string fontFamily = selectedFontFamilyWrapper.Source;
            string fontSize = fontSizeBox.SelectedValue.ToString();

            DefaultFont f = new DefaultFont(fontFamily, fontSize);
            DefaultFontSaveService.SetDefaultFont(f);
        }

        private async void HandlePrivacyBtn_Click(object sender, RoutedEventArgs e) {
            PrivacyDialog dialog = new PrivacyDialog();
            await dialog.ShowAsync();
        }
    }
}
