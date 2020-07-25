using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    class FontSizeService {
        public ObservableCollection<string> FontSizes { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> FontSizesForDefaultFont { get; private set; }

        public FontSizeService() {
            InitFontSizes();
        }

        private void InitFontSizes() {
            FontSizes.Add("8");
            FontSizes.Add("9");
            FontSizes.Add("10");
            FontSizes.Add("11");
            FontSizes.Add("12");
            FontSizes.Add("13");
            FontSizes.Add("14");
            FontSizes.Add("15");
            FontSizes.Add("16");
            FontSizes.Add("18");

            FontSizesForDefaultFont = new ObservableCollection<string>(FontSizes);

            FontSizes.Add("20");
            FontSizes.Add("22");
            FontSizes.Add("24");
            FontSizes.Add("26");
            FontSizes.Add("28");
            FontSizes.Add("36");
            FontSizes.Add("48");
            FontSizes.Add("72");
        }
    }
}
