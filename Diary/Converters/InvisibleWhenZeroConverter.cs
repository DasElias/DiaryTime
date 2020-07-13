using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Diary.Converters {
    class InvisibleWhenZeroConverter : IValueConverter {
        public object Convert(object v, Type t, object p, string l) =>
        Equals(0, (int) v) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object v, Type t, object p, string l) => null;
    }
}
