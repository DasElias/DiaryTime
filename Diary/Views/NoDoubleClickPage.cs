using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Diary.Views {
    public class NoDoubleClickPage : Page {
        private const int MIN_MS_BEFORE_LOADED = 500;

        private DateTime? _loadedTime = null;

        public NoDoubleClickPage() {
            this.Loaded += NoDoubleClickPage_Loaded;
        }

        protected DateTime LoadedTime {
            get {
                return _loadedTime ?? DateTime.UtcNow;
            }
            private set {
                _loadedTime = value;
            }
        }

        protected bool IsReadyToPressButton() {
            double msSinceLoaded = (DateTime.UtcNow - LoadedTime).TotalMilliseconds;
            return this.IsLoaded && msSinceLoaded >= MIN_MS_BEFORE_LOADED;
        }

        private void NoDoubleClickPage_Loaded(object sender, RoutedEventArgs e) {
            LoadedTime = DateTime.UtcNow;
        }
    }
}
