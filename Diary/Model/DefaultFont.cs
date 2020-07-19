using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Model {
    class DefaultFont {
        public DefaultFont(string fontFamily, string fontSize) {
            FontFamily = fontFamily;
            FontSize = fontSize;
        }
        
        public string FontFamily { get; set; }
        public string FontSize { get; set; }
    }
}
