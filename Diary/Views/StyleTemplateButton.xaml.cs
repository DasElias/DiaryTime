using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Diary.Views {

    public delegate void ApplyStyleTemplate(StyleTemplateButton sender, StyleTemplate chosenTemplate);

    public sealed partial class StyleTemplateButton : UserControl {

        private StyleTemplateImplementations styleTemplates = new StyleTemplateImplementations();
        private StyleTemplate lastUsedStyleTemplate;

        public StyleTemplateButton() {
            this.InitializeComponent();
            lastUsedStyleTemplate = styleTemplates[1];
        }

        public event ApplyStyleTemplate ApplyStyleTemplate;

        private void HandleTemplateButton_Click(object sender, RoutedEventArgs e) {
            Button btn = (Button) sender;
            string templateName = (btn.Content as TextBlock).Text;
            StyleTemplate template = styleTemplates.GetTemplate(templateName);
            lastUsedStyleTemplate = template;
            this.Bindings.Update();
            styleTemplateFlyout.Hide();
            ApplyStyleTemplate?.Invoke(this, template);
        }

        private void UpdateSelectedTemplate() {
          //  selectedTemplateButton.Template.Bu
        }

        private void HandleTemplateButton_PointerEntered(object sender, PointerRoutedEventArgs e) {
            Button btn = (Button) sender;
            btn.Foreground = new SolidColorBrush(Colors.Red);
        }
    }
}
