using Diary.Events;
using Diary.Services;
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
using Windows.UI.Xaml.Navigation;


namespace Diary.Views {
    public sealed partial class FirstLoginPage : Page {
        private ResourceLoader resourceLoader;
        private string databaseName;

        public FirstLoginPage() {
            this.InitializeComponent();
            this.resourceLoader = ResourceLoader.GetForCurrentView();
            this.Loaded += Page_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            databaseName = ((LaunchArgument) e.Parameter).DatabaseName;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            ContentDialog welcomeDialog = new ContentDialog() {
                Title = resourceLoader.GetString("welcomeMessage"),
                Content = resourceLoader.GetString("welcomeMessageBody"),
                PrimaryButtonText = resourceLoader.GetString("ok")
            };
            await welcomeDialog.ShowAsync();
        }

        private void HandleSignInBtn_Click(object sender, RoutedEventArgs e) {
            Login();
        }

        private void HandlePasswordBox_KeyDown(object sender, KeyRoutedEventArgs e) {
            if(e.Key == Windows.System.VirtualKey.Enter) {
                Login();
            }
        }

        private void Login() {
            string pw = newPasswordBox.Password;
            string repeatedPw = repeatPasswordBox.Password;
            if(pw.Length == 0) {
                errorMsgField.Text = resourceLoader.GetString("noPasswordEntered");
                return;
            } else if(pw != repeatedPw) {
                errorMsgField.Text = resourceLoader.GetString("passwordsAreNotTheSame");
                return;
            }

            var encryptor = new EncryptionService(pw);
            var persistor = new DatabasePersistorService(encryptor, databaseName);
            PersistorEncryptorArgument arg = new PersistorEncryptorArgument(persistor, encryptor);
            Frame.Navigate(typeof(ShellPage), arg);
        }
    }

}
