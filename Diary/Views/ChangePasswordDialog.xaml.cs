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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Diary.Views {
    public enum ChangePasswortResult {
        SUCCESSFULLY_CHANGED,
        CANCEL
    }

    public sealed partial class ChangePasswordDialog : ContentDialog {
        private ResourceLoader resourceLoader;
        private string oldPassword;

        public ChangePasswordDialog(string oldPassword) {
            this.oldPassword = oldPassword;

            this.InitializeComponent();
            resourceLoader = ResourceLoader.GetForCurrentView();

            IsPrimaryButtonEnabled = false;
        }

        public ChangePasswortResult Result { get; set; } = ChangePasswortResult.CANCEL;
        public string NewPassword { get; private set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            bool isOldPwEqual = this.oldPassword == oldPwBox.Password;
            bool isNewPwEqual = newPwBox.Password == newRepeatPwBox.Password;
            if(isOldPwEqual && isNewPwEqual) {
                Result = ChangePasswortResult.SUCCESSFULLY_CHANGED;
                NewPassword = newPwBox.Password;
            } else {
                args.Cancel = true;
                if(! isOldPwEqual) errorMsgField.Text = resourceLoader.GetString("oldPasswordIsIncorrect");
                else if(! isNewPwEqual) errorMsgField.Text = resourceLoader.GetString("passwordsAreNotTheSame");
            }

            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            Result = ChangePasswortResult.CANCEL;
            this.Hide();
        }

        private void HandleOldPw_PasswordChanged(object sender, RoutedEventArgs e) {
            UpdatePrimaryButton();
        }

        private void HandleNewPw_PasswordChanged(object sender, RoutedEventArgs e) {
            UpdatePrimaryButton();
        }

        private void HandleNewPwRepeat_PasswordChanged(object sender, RoutedEventArgs e) {
            UpdatePrimaryButton();
        }

        private void UpdatePrimaryButton() {
            bool isEnabled = oldPwBox.Password.Length > 0 && newPwBox.Password.Length > 0 && newRepeatPwBox.Password.Length > 0;
            IsPrimaryButtonEnabled = isEnabled;
        }
    }
}
