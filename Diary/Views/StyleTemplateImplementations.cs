using Diary.Model;
using Diary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace Diary.Views {
    class StyleTemplateImplementations : List<StyleTemplate> {
        public StyleTemplateImplementations(ResourceLoader resourceLoader) {
            DefaultFont defaultFont = DefaultFontSaveService.GetDefaultFont();
            float defaultFontSize = (float) Convert.ToDouble(defaultFont.FontSize);

            Add(new StyleTemplate(resourceLoader.GetString("styleTemplate_default"), defaultFont.FontFamily, defaultFontSize, "Black"));
            Add(new StyleTemplate(resourceLoader.GetString("styleTemplate_heading") + " 1", "Calibri", 16, "#2F5496"));
            Add(new StyleTemplate(resourceLoader.GetString("styleTemplate_heading") + " 2", "Calibri", 15, "#2F5496"));
            Add(new StyleTemplate(resourceLoader.GetString("styleTemplate_heading") + " 3", "Calibri", 14, "#1F3763"));
            Add(new StyleTemplate(resourceLoader.GetString("styleTemplate_title"), "Calibri", 20, "Black"));

        }

        public StyleTemplate GetTemplate(string name) {
            return Find(template => template.TemplateName == name);
        }
    }
}
