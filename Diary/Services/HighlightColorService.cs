using Diary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Diary.Services {
    class HighlightColorService {
        private List<HighlightColor> colors = new List<HighlightColor>();

        public HighlightColorService() {
            AddColors();
        }

        public ReadOnlyCollection<HighlightColor> Colors {
            get {
                return colors.AsReadOnly();
            }
        }

        private void AddColors() {
            AddTransparent();
            AddRGB(255, 255, 0);
            AddRGB(0, 255, 0);
            AddRGB(255, 0, 255);
            AddRGB(0, 0, 255);
            AddRGB(255, 0, 0);
            AddRGB(0, 0, 128);
            AddRGB(0, 128, 128);
            AddRGB(0, 128, 0);
            AddRGB(128, 0, 128);
            AddRGB(128, 0, 0);
            AddRGB(128, 128, 0);
            AddRGB(128, 128, 128);
            AddRGB(192, 192, 192);
            AddRGB(0, 0, 0);
        }

        private void AddTransparent() {
            Color color = Windows.UI.Colors.Transparent;
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new SvgImageSource(new Uri("ms-appx:///Assets/NoFill.svg"));
            colors.Add(new HighlightColor(brush, color));
        
        }

        private void AddRGB(byte r, byte g, byte b) {
            Color color = Color.FromArgb(255, r, g, b);
            Brush brush = new SolidColorBrush(color);
            colors.Add(new HighlightColor(brush, color));
        }
    }
}
