using Diary.Events;
using Diary.Model;
using Diary.Services;
using Diary.Utils;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Devices.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Diary.Views {
    public sealed partial class ShellPage : Page {
        private static readonly List<Color> DENSITY_COLORS;
        private static readonly Brush SELECTED_BACKGROUND_COLOR;

        private static ShellPage instance;

        static ShellPage() {
            DENSITY_COLORS = new List<Color>();
            DENSITY_COLORS.Add(Color.FromArgb(255, 255, 255, 255));

            SELECTED_BACKGROUND_COLOR = (Brush) Application.Current.Resources["SystemControlHighlightAccent2RevealBackgroundBrush"];
        }

        private AbstractEncryptor encryptor = null;
        private AbstractPersistorService persistorService = null;
        private ExecutorAtMidnight executorAtMidnight = null;

        /*
         * Sometimes, we want programmatically to unselect all dates in the CalendarView, for example when navigating to ListViewPage.
         * Nevertheless, the user should not be able to unselect an entry, therefore we cancel all unselect events, except this flag is set.
         */
        private bool allowEntryUnselect = false;

        /*
         * When editing today's entry, we don't need want ViewEntryPage to open first. Therefore, this flag allows to supress to open ViewEntryPage
         * when the calendar's selection has changed. Please remember that you have to set this flag to false by yourself.
         */
        private bool shouldSuppressOpenViewOnCalendarChange = false;

        private DateTimeOffset? calendarViewSelectedDate;

        public ShellPage() {
            this.InitializeComponent();
            Loaded += Page_Loaded;
            instance = this;
        }


        private void Page_Loaded(object sender, RoutedEventArgs e) {
            SelectToday();
            executorAtMidnight = new ExecutorAtMidnight(HandleOnMidnight);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            PersistorEncryptorArgument arg = (PersistorEncryptorArgument) e.Parameter;
            encryptor = arg.Encryptor;
            persistorService = arg.Persistor;
            persistorService.DiaryEntries.CollectionChanged += HandleDiaryEntries_CollectionChanged;
        }

        internal static void SelectCalendarItem(DiaryEntryPreview entry) {
            instance.SelectDate(entry.Date);
        }

        internal static void UnselectCalendarItems() {
            instance.UnselectDate();
        }

        private void HandleEditTodayEntry_Tapped(object sender, TappedRoutedEventArgs e) {
            shouldSuppressOpenViewOnCalendarChange = true;
            SelectToday();
            shouldSuppressOpenViewOnCalendarChange = false;

            if(persistorService.ContainsEntryForDate(DateTime.Today)) {
                DiaryEntry entry = persistorService.LoadEntry(DateTime.Today);
                EntryArgument arg = new EntryArgument(entry, persistorService);
                ShellFrame.Navigate(typeof(EditPage), arg);
            } else {
                NavigateToNoEntryPage(DateTime.Today);
            }
        }

        private void HandleViewTodayEntry_Tapped(object sender, TappedRoutedEventArgs e) {
            SelectToday();
        }

        private void HandleOpenListView_Tapped(object sender, TappedRoutedEventArgs e) {
            UnselectCalendarItems();

            PersistorArgument arg = new PersistorArgument(persistorService);
            ShellFrame.Navigate(typeof(ListViewPage), arg);
        }

        private void HandleCalendar_SelectedDateChange(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args) {
            if(args.AddedDates.Count > 0) {
                DateTimeOffset selected = args.AddedDates[0];

                // we don't want to navigate, if this event was only fired because of the undo of an unselection event
                if(selected != this.calendarViewSelectedDate) {
                    this.calendarViewSelectedDate = selected;
                    
                    if(! shouldSuppressOpenViewOnCalendarChange) {
                        DateTime dateTime = selected.DateTime;
                        NavigateOnCalenderClick(dateTime);
                    }
                }
            } else if(args.RemovedDates.Count > 0 && !allowEntryUnselect) {
                // cancel unselect event
                calendarView.SelectedDates.Add(args.RemovedDates[0]);
                return;
            } else if(args.RemovedDates.Count > 0) {
                // selection was cleared, for example due to navigation to the settings page
                this.calendarViewSelectedDate = null;
            }

            // update selected background
            var dayItems = calendarView.GetChildren().OfType<CalendarViewDayItem>();
            foreach(CalendarViewDayItem elem in dayItems) {
                UpdateSelectedBackground(elem);
            }

        }

        private void HandleOnMidnight() {
            var dayItems = calendarView.GetChildren().OfType<CalendarViewDayItem>();
            foreach(CalendarViewDayItem elem in dayItems) {
                if(DateUtils.IsToday(elem.Date)) {
                    elem.IsBlackout = false;
                }
            }
        }

        private void HandleDiaryEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            var dayItems = calendarView.GetChildren().OfType<CalendarViewDayItem>();
            foreach(CalendarViewDayItem elem in dayItems) {
                UpdateDensityBars(elem);
            }
        }

        private void HandleCalendar_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args) {
            if(args.Phase == 0) {
                // render basic day items
                UpdateSelectedBackground(args.Item);

                // register callback for next phase
                args.RegisterUpdateCallback(HandleCalendar_CalendarViewDayItemChanging);
            } else if(args.Phase == 1) {
                // set blackout dates
                bool isDayInFuture = DateUtils.IsInFuture(args.Item.Date);
                if(isDayInFuture) {
                    args.Item.IsBlackout = true;
                }

                // register callback for next phase
                args.RegisterUpdateCallback(HandleCalendar_CalendarViewDayItemChanging);
            } else if(args.Phase == 2) {
                // set density bars, but not for days in the future to avoid unnecessary processing

                UpdateDensityBars(args.Item);
            }
        }

        private void UpdateDensityBars(CalendarViewDayItem item) {
            bool isDayInFuture = DateUtils.IsInFuture(item.Date);
            if(!isDayInFuture && persistorService.ContainsEntryForDate(item.Date.Date)) {
                item.SetDensityColors(DENSITY_COLORS);
            } else {
                item.SetDensityColors(null);
            }
        }

        private void UpdateSelectedBackground(CalendarViewDayItem item) {
            bool isDayInFuture = DateUtils.IsInFuture(item.Date);
            if(!isDayInFuture) {
                if(calendarView.SelectedDates.Contains(item.Date)) {
                    item.Background = SELECTED_BACKGROUND_COLOR;
                } else {
                    item.Background = null;
                }
            }
        }

        private void SelectToday() {
            SelectDate(DateTimeOffset.Now);
        }

        private void SelectDate(DateTimeOffset d) {
            try {
                UnselectDate();
                calendarView.SelectedDates.Add(d);
                calendarView.SetDisplayDate(d);
            } catch(Exception e) {
                Crashes.TrackError(e);
            }
        }

        private void UnselectDate() {
            allowEntryUnselect = true;
            calendarView.SelectedDates.Clear();
            allowEntryUnselect = false;
        }

        private void NavigateOnCalenderClick(DateTime date) {
            if(persistorService.ContainsEntryForDate(date)) NavigateToEntry(date);
            else NavigateToNoEntryPage(date);
        }

        private void NavigateToNoEntryPage(DateTime date) {
            DateArgument noEntryForDayArg = new DateArgument(date, persistorService);
            ShellFrame.Navigate(typeof(NoEntryForDayPage), noEntryForDayArg);
        }

        private void NavigateToEntry(DateTime date) {
            DiaryEntry entry = persistorService.LoadEntry(date);
            EntryArgument arg = new EntryArgument(entry, persistorService);
            ShellFrame.Navigate(typeof(ViewEntryPage), arg);
        }

        private void HandleSettings_Tapped(object sender, TappedRoutedEventArgs e) {
            UnselectCalendarItems();

            PersistorEncryptorArgument arg = new PersistorEncryptorArgument(persistorService, encryptor);
            ShellFrame.Navigate(typeof(SettingsPage), arg);
        }
    }
}
