using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Diary.Views {
    public sealed partial class CheckPasswordDialog : ContentDialog {
        private string plainPassword;

        public CheckPasswordDialog(string contentText, string plainPassword) {
            this.ContentText = contentText;
            this.plainPassword = plainPassword;

            this.InitializeComponent();
        }

        public string ContentText;
        public string Password;

        private void HandlePrimaryButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            if(Password != plainPassword) {
                args.Cancel = true;
                errorMsgBox.Text = "Falsches Passwort!";
            }
        }

        
    }
}
