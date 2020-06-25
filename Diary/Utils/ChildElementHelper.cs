using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Diary.Utils {
    static class ChildElementHelper {
        public static List<FrameworkElement> GetChildren(this DependencyObject parent) {
            var list = new List<FrameworkElement>();

            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
                var child = VisualTreeHelper.GetChild(parent, i);

                if(child is FrameworkElement) {
                    list.Add(child as FrameworkElement);
                }

                list.AddRange(GetChildren(child));
            }

            return list;
        }
    }
}
