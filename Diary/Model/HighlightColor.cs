using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Diary.Model {
    class HighlightColor {
        public HighlightColor(Brush fill, Color color) {
            this.Fill = fill;
            this.Color = color;
        }

        public Brush Fill { get; }
        public Color Color { get; }
    }
}
