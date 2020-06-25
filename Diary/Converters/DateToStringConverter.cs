using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Diary.Converters {
    class DateToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            string formatString = parameter as string;

            if(formatString != null && value is DateTime) return ((DateTime) value).ToString(formatString);
            else return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
