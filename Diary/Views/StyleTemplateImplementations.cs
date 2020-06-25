using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Views {
    class StyleTemplateImplementations : List<StyleTemplate> {
        public StyleTemplateImplementations() {
            Add(new StyleTemplate("Normal", "Calibri", 13, "Black"));
            Add(new StyleTemplate("Überschrift 1", "Calibri", 15, "#2F5496"));
            Add(new StyleTemplate("Überschrift 2", "Calibri", 14, "#2F5496"));
            Add(new StyleTemplate("Überschrift 3", "Calibri", 13, "#1F3763"));
            Add(new StyleTemplate("Titel", "Calibri", 20, "Black"));

        }

        public StyleTemplate GetTemplate(string name) {
            return Find(template => template.TemplateName == name);
        }
    }
}
