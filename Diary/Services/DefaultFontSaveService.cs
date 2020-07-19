using Diary.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    static class DefaultFontSaveService {
        private const string DEFAULT_FONT_FAMILY = "Calibri";
        private const string DEFAULT_FONT_SIZE = "13";

        private const string FONT_FAMILY_KEY = "FontFamily";
        private const string FONT_SIZE_KEY = "FontSize";

        public static void SetDefaultFont(DefaultFont fontData) {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[FONT_FAMILY_KEY] = fontData.FontFamily;
            roamingSettings.Values[FONT_SIZE_KEY] = fontData.FontSize;
        }

        public static void SetDefaultFontFamily(string fontFamily) {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[FONT_FAMILY_KEY] = fontFamily;
        }

        public static void SetDefaultFontSize(string fontSizeString) {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[FONT_SIZE_KEY] = fontSizeString;
        }

        public static DefaultFont GetDefaultFont() {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            string fontFamily = roamingSettings.Values[FONT_FAMILY_KEY] as string;
            string fontSize = roamingSettings.Values[FONT_SIZE_KEY] as string;

            if(fontFamily == null || fontSize == null) {
                return new DefaultFont(DEFAULT_FONT_FAMILY, DEFAULT_FONT_SIZE);
            } else {
                return new DefaultFont(fontFamily, fontSize);
            }
        }
    }
}
