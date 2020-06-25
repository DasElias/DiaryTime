using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Diary.Model {
    class ColorGroup : IGrouping<string, SolidColorBrush> {
        private List<SolidColorBrush> items;
        
        public ColorGroup(string categoryName, IEnumerable<SolidColorBrush> items) {
            Key = categoryName;
            this.items = new List<SolidColorBrush>(items);
        }

        public string Key { get; }

        public IEnumerator<SolidColorBrush> GetEnumerator() {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return items.GetEnumerator();
        }
    }
}
