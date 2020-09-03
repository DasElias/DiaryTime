using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Diary.Views {
    class FontFamilyOptions {
        private static readonly string[] IGNORED_FONT_NAMES = new string[] {
            "HoloLens MDL2 Assets",
            "Marlett",
            "Segoe MDL2 Assets",
            "Segoe UI Emoji",
            "Segoe UI Symbol",
            "Symbol",
            "Webdings",
            "Wingdings"
        };

        private static List<FontFamilyWrapper> fontFamilies;

        public ReadOnlyCollection<FontFamilyWrapper> FontFamilies {
            get {
                if(fontFamilies == null) InitFontFamilies();
                return fontFamilies.AsReadOnly();
            }
        }

        public FontFamilyWrapper Find(string fontFamilyName) {
            return fontFamilies.Find(font => font.Source == fontFamilyName);
        }

        public int FindIndex(string fontFamilyName) {
            return fontFamilies.FindIndex(font => font.Source == fontFamilyName);
        }

        private void InitFontFamilies() {
            fontFamilies = new List<FontFamilyWrapper>();

            List<string> fontNames = GetFontFamilies().OrderBy(f => f).ToList();
            foreach(string f in fontNames) {
                // check if we want to ignore that font
                if(IGNORED_FONT_NAMES.Any(fontName => fontName.Equals(f, StringComparison.InvariantCultureIgnoreCase))) {
                    continue;
                }

                fontFamilies.Add(new FontFamilyWrapper(f));
            }
        }

        private static List<string> GetFontFamilies() {
            List<string> fontList = new List<string>();
            SharpDX.DirectWrite.Factory factory = new SharpDX.DirectWrite.Factory();

            var fontCollection = factory.GetSystemFontCollection(false);
            var familCount = fontCollection.FontFamilyCount;

            for(int i = 0; i < familCount; i++) {
                var fontFamily = fontCollection.GetFontFamily(i);
                var familyNames = fontFamily.FamilyNames;

                int index;
                if(!familyNames.FindLocaleName(CultureInfo.CurrentCulture.Name, out index)) {
                    if(!familyNames.FindLocaleName("en-us", out index)) {
                        index = 0;
                    }
                }

                string name = familyNames.GetString(index);
                fontList.Add(name);
            }
            return fontList;
        }
    }
}
