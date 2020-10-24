using Diary.Events;
using Diary.Services;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
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
    public sealed partial class LoginPage : Page {
        private ResourceLoader resourceLoader;
        private object lastDoubleTappedRect = null;
        private string databaseName;

        public LoginPage() {
            this.InitializeComponent();
            resourceLoader = ResourceLoader.GetForCurrentView();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            databaseName = ((LaunchArgument) e.Parameter).DatabaseName;
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
                var persistor = new DatabasePersistorService(encryptor, databaseName);
                PersistorEncryptorArgument arg = new PersistorEncryptorArgument(persistor, encryptor);
                Frame.Navigate(typeof(ShellPage), arg);
            } catch(InvalidPasswordException) {
                errorMsgField.Text = resourceLoader.GetString("invalidPasswordTryAgain");
            } catch(SqliteException) {
                await ShowFileErrorDialogAsync();
            }
        }

        private async Task ShowFileErrorDialogAsync() {
            ContentDialog errorDialog = new ContentDialog() {
                Title = resourceLoader.GetString("errorHasOccured"),
                Content = resourceLoader.GetString("couldntReadEntries"),
                PrimaryButtonText = resourceLoader.GetString("exitApp"),
                SecondaryButtonText = resourceLoader.GetString("reset")
            };
            ContentDialogResult result = await errorDialog.ShowAsync();

            if(result == ContentDialogResult.Primary) {
                CoreApplication.Exit();
            } else {
                await DatabasePersistorService.DropDatabase(databaseName);
                await CoreApplication.RequestRestartAsync("");
            }

        }

        private async void HandleRect_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) {
            if(lastDoubleTappedRect == null) lastDoubleTappedRect = sender;
            else if(lastDoubleTappedRect != sender) {
                await CoreApplication.RequestRestartAsync("-private");
            }
        }
    }

}
