using Diary.Events;
using Diary.Model;
using Diary.Services;
using Diary.Utils;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Diary.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewEntryPage : NoDoubleClickPage {
        private ResourceLoader resourceLoader;

        private AbstractPersistorService persistorService;
        private DiaryEntry entry;
        private bool wasFirstLoaded = false;
        private List<int> debugList = new List<int>();
        private List<long> timeDebugList = new List<long>();

        public ViewEntryPage() {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            resourceLoader = ResourceLoader.GetForCurrentView();

            this.Loaded += Page_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            EntryArgument arg = (EntryArgument) e.Parameter;
            persistorService = arg.PersistorService;
            entry = arg.Entry;

            AddToDebugList(1);
            if(wasFirstLoaded && entry != null) {
                AddToDebugList(2);
                UpdateContent();
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            entryImagesEditor.StopImageLoading();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            AddToDebugList(3);
            if(!wasFirstLoaded) {
                AddToDebugList(4);
                // if the page wasn't unloaded in the meantime
                if(entry != null) {
                    AddToDebugList(5);
                    UpdateContent();
                }
                wasFirstLoaded = true;
            }
        }

        private string Print(object var) {
            if(var == null) return "null";
            else return var.ToString();
        }

        private void AddToDebugList(int debug) {
            this.debugList.Add(debug);
            if(debugList.Count > 100) {
                debugList.RemoveRange(0, 50);
            }
        }

        private void AddToTimeDebugList() {
            long unixSecond = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            timeDebugList.Add(unixSecond);
            if(timeDebugList.Count > 100) {
                timeDebugList.RemoveRange(0, 50);
            }
        }

        private void UpdateContent() {
            AddToTimeDebugList();

            try {
                entryButtonBarControl.DateText = entry.DateString;
                titleElement.Text = entry.Title;

                contentElement.IsReadOnly = false;
                contentElement.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, entry.RtfText);
                contentElement.Document.ApplyDisplayUpdates();
                contentElement.IsReadOnly = true;

                bool hasEntryImages = entry.StoredImages.Count > 0;
                entryImagesEditor.Visibility = hasEntryImages ? Visibility.Visible : Visibility.Collapsed;
                if(hasEntryImages) {
                    entryImagesEditor.Clear();
                    _ = entryImagesEditor.LoadImages(entry.StoredImages);
                }
            } catch(NullReferenceException e) {
                var str = "";
                foreach(int i in debugList) {
                    str += i;
                    str += ", ";
                }

                var timeStr = "";
                foreach(long i in timeDebugList) {
                    timeStr += i;
                    timeStr += ", ";
                }

                Crashes.TrackError(e, new Dictionary<string, string>{
                    { "entryButtonBarControl", Print(entryButtonBarControl) },
                    { "entry", Print(entry) },
                    { "entry.DateString", Print(entry?.DateString) },
                    { "entry.Title", Print(entry?.Title) },
                    { "titleElement", Print(titleElement) },
                    { "contentElement", Print(contentElement) },
                    { "contentElement.Document", Print(contentElement?.Document) },
                    { "entry.StoredImages", Print(entry?.StoredImages) },
                    { "entryImagesEditor", Print(entryImagesEditor) },
                    { "str", str },
                    { "timeStr", timeStr }
                });
            }

        }

        private async void HandleEditBtn_Click(object sender, RoutedEventArgs e) {
            AddToDebugList(6);
            if(!IsReadyToPressButton()) return;
            AddToDebugList(7);

            using(var btnLock = new DoubleClickPreventer(entryButtonBarControl)) {
                if(entry.IsToday) {
                    StartEdit();
                    return;
                }

                ContentDialog editConfirmationDialog = new ContentDialog {
                    Title = resourceLoader.GetString("editEntry"),
                    Content = string.Format(resourceLoader.GetString("confirmEditPastDay"), entry.DateString),
                    PrimaryButtonText = resourceLoader.GetString("abort"),
                    SecondaryButtonText = resourceLoader.GetString("continue")
                };

                ContentDialogResult result = await editConfirmationDialog.ShowAsync();
                if(result == ContentDialogResult.Secondary) {
                    StartEdit();
                }
            }
        }

        private void StartEdit() {
            EntryArgument arg = new EntryArgument(entry, persistorService);
            Frame.Navigate(typeof(EditPage), arg);
        }
    }
}
