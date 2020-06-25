using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Diary.Views {
    class FontFamilyOptions : List<FontFamilyWrapper> {
        public FontFamilyOptions() {
            List<string> fontNames = CanvasTextFormat.GetSystemFontFamilies().OrderBy(f => f).ToList();
            foreach(string f in fontNames) {
                Add(new FontFamilyWrapper(f));
            }
        }

        public FontFamilyWrapper Find(string fontFamilyName) {
            return Find(font => font.Source == fontFamilyName);
        }

        public int FindIndex(string fontFamilyName) {
            return FindIndex(font => font.Source == fontFamilyName);
        }
    }
}
