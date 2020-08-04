using Diary.Events;
using Diary.Model;
using Diary.Services;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Diary.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListViewPage : Page {
        private static readonly WeekdayNameFormatter weekdayNameFormatter = new WeekdayNameFormatter(ResourceLoader.GetForCurrentView());
        private AbstractPersistorService persistorService;
        private GroupedObservableCollection<MonthGroupKey, DiaryEntryPreview> diaryEntries;

        public ListViewPage() {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            entriesList.SelectedItem = null;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            PersistorArgument arg = (PersistorArgument) e.Parameter;
            persistorService = arg.PersistorService;
            diaryEntries = new GroupedObservableCollection<MonthGroupKey, DiaryEntryPreview>((entry) => new MonthGroupKey(entry.Date), persistorService.DiaryEntries);
        }

        internal static string FormatDate(DateTime date) {
            return date.Month.ToString("MMMM") + " " + date.Year.ToString();

        }

        internal static string FormatWeekday(DateTime date) {
            switch(date.DayOfWeek) {
                case DayOfWeek.Monday:
                    return weekdayNameFormatter.Monday;
                case DayOfWeek.Tuesday:
                    return weekdayNameFormatter.Tuesday;
                case DayOfWeek.Wednesday:
                    return weekdayNameFormatter.Wednesday;
                case DayOfWeek.Thursday:
                    return weekdayNameFormatter.Thursday;
                case DayOfWeek.Friday:
                    return weekdayNameFormatter.Friday;
                case DayOfWeek.Saturday:
                    return weekdayNameFormatter.Saturday;
                case DayOfWeek.Sunday:
                    return weekdayNameFormatter.Sunday;
                default:
                    return "";
            }
        }

        internal static string FormatMonth(DateTime date) {
            return date.Month.ToString() + date.Year.ToString();
        }


        private void HandleSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            List<DiaryEntryPreview> tempFiltered;
            tempFiltered = persistorService.DiaryEntries.Where(entry => {
                return FilterService.ShouldDisplayEntry(entry, searchBox.Text);
            }).ToList();

  

            // remove objects in filteredEntries that are not in tempFiltered
            diaryEntries.RemoveAll(entry => !tempFiltered.Contains(entry));


            // add objects that are in tempFiltered, but not in filteredEntries
            foreach(DiaryEntryPreview entry in tempFiltered) {
                if(!diaryEntries.Contains(entry)) {
                    diaryEntries.Add(entry);
                }
            }

        }

        

        private void HandleEntriesList_ItemClick(object sender, ItemClickEventArgs e) {
            DiaryEntryPreview prev = (DiaryEntryPreview) e.ClickedItem;
            DiaryEntry loadedEntry = persistorService.LoadEntry(prev);

            ShellPage.SelectCalendarItem(prev);

            EntryArgument arg = new EntryArgument(loadedEntry, persistorService);
            Frame.Navigate(typeof(ViewEntryPage), arg);
        }
    }
}
