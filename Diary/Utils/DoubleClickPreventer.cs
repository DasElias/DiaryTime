using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Diary.Utils {
    public class DoubleClickPreventer : IDisposable {
        private readonly Control _button;

        public DoubleClickPreventer(object sender) {
            _button = (Control) sender;
            _button.IsEnabled = false;
        }

        public void Dispose() {
            _button.IsEnabled = true;
        }
    }
}
