﻿using Diary.Services;
using Diary.Utils;
using Diary.Views;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Resources;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Diary.Events;

namespace Diary {
    /// <summary>
    /// Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
    /// </summary>
    sealed partial class App : Application {
        public App() {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            CustomXamlResourceLoader.Current = new XamlResourceLoader();

            AppCenter.Start("24bd87e3-d4f1-43b5-aab1-b86c841de6b7", typeof(Analytics), typeof(Crashes));
            CrashReportConfirmationService.LoadValue();

        }

        /// <summary>
        /// Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird. Weitere Einstiegspunkte
        /// werden z. B. verwendet, wenn die Anwendung gestartet wird, um eine bestimmte Datei zu öffnen.
        /// </summary>
        /// <param name="e">Details über Startanforderung und -prozess.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e) {
            bool shouldLaunchPrivate = e.Arguments == "-private";
            string databaseName = GetDatabaseName(shouldLaunchPrivate);

            Start(new LaunchArgument(databaseName));           
            //  DiaryReminderToastService.ShowToastImmediately();
            // DiaryReminderToastService.ScheduleToast(DateTime.Now.AddSeconds(15));
        }

        protected override void OnActivated(IActivatedEventArgs args) {
            Start(new LaunchArgument(GetDatabaseName()));
        }

        private void Start(LaunchArgument arg) {
            Frame frame = Window.Current.Content as Frame;

            if(frame == null) {
                frame = new Frame();
                Window.Current.Content = frame;
                if(AsyncContext.Run(() => DatabasePersistorService.DoesDatabaseExist(arg.DatabaseName))) {
                    frame.Navigate(typeof(LoginPage), arg);

                } else {
                    frame.Navigate(typeof(FirstLoginPage), arg);

                }
            }


            Window.Current.Activate();
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Ausführung der Anwendung angehalten wird.  Der Anwendungszustand wird gespeichert,
        /// ohne zu wissen, ob die Anwendung beendet oder fortgesetzt wird und die Speicherinhalte dabei
        /// unbeschädigt bleiben.
        /// </summary>
        /// <param name="sender">Die Quelle der Anhalteanforderung.</param>
        /// <param name="e">Details zur Anhalteanforderung.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden
            deferral.Complete();
        }

        private string GetDatabaseName(bool shouldLaunchPrivate = false) {
            return shouldLaunchPrivate ? "secret.db" : "diary.db";
        }
    }
}
