using Diary.Services;
using Microsoft.AppCenter.Crashes;
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
    public sealed partial class PrivacyDialog : ContentDialog {
        private ResourceLoader resourceLoader;

        public PrivacyDialog() {
            this.InitializeComponent();
            this.Loaded += HandlePrivacyDialog_Loaded;
            resourceLoader = ResourceLoader.GetForCurrentView();

            this.Title = resourceLoader.GetString("privacy");
            this.PrimaryButtonText = resourceLoader.GetString("save");
            this.PrimaryButtonClick += ContentDialog_PrimaryButtonClick;
            this.SecondaryButtonText = resourceLoader.GetString("abort");
            this.SecondaryButtonClick += ContentDialog_SecondaryButtonClick;
        }

        private void HandlePrivacyDialog_Loaded(object sender, RoutedEventArgs e) {
            consentCheckBox.IsChecked = CrashReportConfirmationService.ShouldSend();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            bool isChecked = consentCheckBox.IsChecked.GetValueOrDefault();
            CrashReportConfirmationService.Update(isChecked);
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            this.Hide();
        }
    }
}
