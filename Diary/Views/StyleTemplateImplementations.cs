using Diary.Model;
using Diary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Views {
    class StyleTemplateImplementations : List<StyleTemplate> {
        public StyleTemplateImplementations() {
            DefaultFont defaultFont = DefaultFontSaveService.GetDefaultFont();
            float defaultFontSize = (float) Convert.ToDouble(defaultFont.FontSize);

            Add(new StyleTemplate("Normal", defaultFont.FontFamily, defaultFontSize, "Black"));
            Add(new StyleTemplate("Überschrift 1", "Calibri", 16, "#2F5496"));
            Add(new StyleTemplate("Überschrift 2", "Calibri", 15, "#2F5496"));
            Add(new StyleTemplate("Überschrift 3", "Calibri", 14, "#1F3763"));
            Add(new StyleTemplate("Titel", "Calibri", 20, "Black"));

        }

        public StyleTemplate GetTemplate(string name) {
            return Find(template => template.TemplateName == name);
        }
    }
}
