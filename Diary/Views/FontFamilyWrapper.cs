using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Diary.Views {
    class FontFamilyWrapper : FontFamily {
        public FontFamilyWrapper(string fontFamilyName) : base(fontFamilyName) {

        }

        public override string ToString() {
            return Source;
        }
    }
}
