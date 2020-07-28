using Diary.Services;
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

namespace Diary {
    /// <summary>
    /// Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
    /// </summary>
    sealed partial class App : Application {
        public App() {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            CustomXamlResourceLoader.Current = new XamlResourceLoader();
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird. Weitere Einstiegspunkte
        /// werden z. B. verwendet, wenn die Anwendung gestartet wird, um eine bestimmte Datei zu öffnen.
        /// </summary>
        /// <param name="e">Details über Startanforderung und -prozess.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e) {
            Start();

            //  DiaryReminderToastService.ShowToastImmediately();
            // DiaryReminderToastService.ScheduleToast(DateTime.Now.AddSeconds(15));
        }

        protected override void OnActivated(IActivatedEventArgs args) {
            Start();
        }

        private void Start() {
            Frame frame = Window.Current.Content as Frame;

            if(frame == null) {
                frame = new Frame();
                Window.Current.Content = frame;
                if(AsyncContext.Run(DatabasePersistorService.DoesDatabaseExist)) {
                    frame.Navigate(typeof(LoginPage));

                } else {
                    frame.Navigate(typeof(FirstLoginPage));

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
    }
}
