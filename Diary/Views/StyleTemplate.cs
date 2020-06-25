using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Diary.Views {
    public class StyleTemplate {
        public string TemplateName { get; }
        public string FontFamily { get; }
        public float FontSize { get; }
        public string FontColor { get; }
        private Color FontColorBrush {
            get {
                var color = (Color) XamlBindingHelper.ConvertValue(typeof(Color), FontColor);
                return color;           
            }
        }

        public StyleTemplate(string templateName, string fontFamily, float fontSize, string fontColor) {
            TemplateName = templateName;
            FontFamily = fontFamily;
            FontSize = fontSize;
            FontColor = fontColor;
        }

        public void Apply(ITextSelection selection) {
            if(selection != null) {
                selection.CharacterFormat.Name = FontFamily;
                selection.CharacterFormat.Size = FontSize;
                selection.CharacterFormat.ForegroundColor = FontColorBrush;
            }

        }

        

    }
}
