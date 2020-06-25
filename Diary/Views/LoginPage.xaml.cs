using Diary.Events;
using Diary.Services;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class LoginPage : Page {
        public LoginPage() {
            this.InitializeComponent();
        }

        private async void HandleSignInBtn_Click(object sender, RoutedEventArgs e) {
            await Login();
        }

        private async void HandlePasswordBox_KeyDown(object sender, KeyRoutedEventArgs e) {
            if(e.Key == Windows.System.VirtualKey.Enter) {
                await Login();
            }
        }

        private async Task Login() {
            try {
                var encryptor = new EncryptionService(passwordBox.Password);
                var persistor = new DatabasePersistorService(encryptor);
                PersistorEncryptorArgument arg = new PersistorEncryptorArgument(persistor, encryptor);
                Frame.Navigate(typeof(ShellPage), arg);
            } catch(InvalidPasswordException) {
                errorMsgField.Text = "Falsches Passwort. Versuche es bitte erneut.";
            } catch(SqliteException) {
                await ShowFileErrorDialogAsync();
            }
        }

        private async Task ShowFileErrorDialogAsync() {
            ContentDialog errorDialog = new ContentDialog() {
                Title = "Ein Fehler ist aufgetreten",
                Content = "Die gespeicherten Tagebucheinträge konnten nicht gelesen werden, da die Datei irreparabel beschädigt ist.",
                PrimaryButtonText = "Beenden",
                SecondaryButtonText = "Zurücksetzen"
            };
            ContentDialogResult result = await errorDialog.ShowAsync();

            if(result == ContentDialogResult.Primary) {
                Windows.ApplicationModel.Core.CoreApplication.Exit();
            } else {
                await DatabasePersistorService.DropDatabase();
                await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync("");
            }

        }
    }

}
