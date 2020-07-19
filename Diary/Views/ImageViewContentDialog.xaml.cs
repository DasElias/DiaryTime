using Diary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Shapes;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Diary.Views {
    public sealed partial class ImageViewContentDialog : ContentDialog {
        private ImageViewModel imageViewModel;

        public ImageViewContentDialog(ICollection<ImageWrapper> allImages, int index) {
            this.imageViewModel = new ImageViewModel(allImages, index);
            this.InitializeComponent();
           
        }


        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();

            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach(var p in popups) {
                if(p.Child is Rectangle) {
                    var lockRectangle = p.Child as Rectangle;
                    lockRectangle.Tapped += OnLockRectangleTapped;
                }
            }
        }

        private void OnLockRectangleTapped(object sender, TappedRoutedEventArgs e) {
            this.Hide();
            var lockRectangle = sender as Rectangle;
            lockRectangle.Tapped -= OnLockRectangleTapped;
        }

        private void HandleCloseBtn_Click(object sender, RoutedEventArgs e) {
            this.Hide();
        }

        private void HandleNavigationLeftBtn_Click(object sender, RoutedEventArgs e) {
            imageViewModel.NavigateLeft();

        }

        private void HandleNavigationRightBtn_Click(object sender, RoutedEventArgs e) {
            imageViewModel.NavigateRight();
        }
    }
}
