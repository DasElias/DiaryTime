﻿using Diary.Events;
using Diary.Services;
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


namespace Diary.Views {
    public sealed partial class FirstLoginPage : Page {
        public FirstLoginPage() {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            ContentDialog welcomeDialog = new ContentDialog() {
                Title = "Herzlich Willkommen bei DiaryTime!",
                Content = "Uns ist es ein Anliegen, deine persönlichen Gedanken, Gefühle und Erlebnisse vor neugierigen Blicken zu schützen. Bitte lege deshalb nun ein Passwort für dein neues Tagebuch fest.",
                PrimaryButtonText = "OK"
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
                errorMsgField.Text = "Bitte wähle dein Passwort!";
                return;
            } else if(pw != repeatedPw) {
                errorMsgField.Text = "Die eingegebenen Passwörter stimmen nicht überein.";
                return;
            }

            var encryptor = new EncryptionService(pw);
            var persistor = new DatabasePersistorService(encryptor);
            PersistorEncryptorArgument arg = new PersistorEncryptorArgument(persistor, encryptor);
            Frame.Navigate(typeof(ShellPage), arg);
        }
    }

}