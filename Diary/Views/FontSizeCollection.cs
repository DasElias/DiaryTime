using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Views {
    class FontSizeCollection : ObservableCollection<string> {
        public FontSizeCollection() {
            initFontSizes();
        }

        private void initFontSizes() {
            Add("8");
            Add("9");
            Add("10");
            Add("11");
            Add("12");
            Add("13");
            Add("14");
            Add("15");
            Add("16");
            Add("18");
            Add("20");
            Add("22");
            Add("24");
            Add("26");
            Add("28");
            Add("36");
            Add("48");
            Add("72");
        }
    }
}
