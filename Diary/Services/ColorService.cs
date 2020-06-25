using Diary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Diary.Services {
    class ColorService {
        private List<ColorGroup> colorGroups = new List<ColorGroup>();

        public ColorService() {
            AddDesignColors();
            AddDefaultColors();
        }

        public IEnumerable<ColorGroup> GroupedColors => colorGroups;

        private void AddDesignColors() {
            colorGroups.Add(new ColorGroup("Designfarben", new SolidColorBrush[] {
                new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), new SolidColorBrush(Color.FromArgb(255, 231, 230, 230)),
                new SolidColorBrush(Color.FromArgb(255, 68, 84, 106)), new SolidColorBrush(Color.FromArgb(255, 68, 114, 196)), new SolidColorBrush(Color.FromArgb(255, 237, 125, 49)), 
                new SolidColorBrush(Color.FromArgb(255, 165, 165, 165)), new SolidColorBrush(Color.FromArgb(255, 255, 192, 0)), new SolidColorBrush(Color.FromArgb(255, 91, 155, 213)), 
                new SolidColorBrush(Color.FromArgb(255, 112, 173, 71)), 

                new SolidColorBrush(Color.FromArgb(255, 242, 242, 242)), new SolidColorBrush(Color.FromArgb(255, 127, 127, 127)), new SolidColorBrush(Color.FromArgb(255, 208, 206, 206)),
                new SolidColorBrush(Color.FromArgb(255, 214, 220, 228)), new SolidColorBrush(Color.FromArgb(255, 217, 226, 243)), new SolidColorBrush(Color.FromArgb(255, 251, 229, 213)), 
                new SolidColorBrush(Color.FromArgb(255, 237, 237, 237)), new SolidColorBrush(Color.FromArgb(255, 255, 242, 204)), new SolidColorBrush(Color.FromArgb(255, 222, 235, 246)), 
                new SolidColorBrush(Color.FromArgb(255, 226, 239, 217)), 

                new SolidColorBrush(Color.FromArgb(255, 216, 216, 216)), new SolidColorBrush(Color.FromArgb(255, 89, 89, 89)), new SolidColorBrush(Color.FromArgb(255, 174, 171, 171)),
                new SolidColorBrush(Color.FromArgb(255, 173, 185, 202)), new SolidColorBrush(Color.FromArgb(255, 180, 198, 231)), new SolidColorBrush(Color.FromArgb(255, 247, 203, 172)), 
                new SolidColorBrush(Color.FromArgb(255, 219, 219, 219)), new SolidColorBrush(Color.FromArgb(255, 254, 229, 153)), new SolidColorBrush(Color.FromArgb(255, 187, 213, 236)), 
                new SolidColorBrush(Color.FromArgb(255, 197, 224, 179)), 

                new SolidColorBrush(Color.FromArgb(255, 191, 191, 191)), new SolidColorBrush(Color.FromArgb(255, 63, 63, 63)), new SolidColorBrush(Color.FromArgb(255, 117, 112, 112)),
                new SolidColorBrush(Color.FromArgb(255, 132, 150, 176)), new SolidColorBrush(Color.FromArgb(255, 142, 170, 219)), new SolidColorBrush(Color.FromArgb(255, 244, 177, 131)), 
                new SolidColorBrush(Color.FromArgb(255, 201, 201, 201)), new SolidColorBrush(Color.FromArgb(255, 255, 217, 101)), new SolidColorBrush(Color.FromArgb(255, 156, 195, 229)), 
                new SolidColorBrush(Color.FromArgb(255, 168, 208, 141)),

                new SolidColorBrush(Color.FromArgb(255, 165, 165, 165)), new SolidColorBrush(Color.FromArgb(255, 38, 38, 38)), new SolidColorBrush(Color.FromArgb(255, 58, 56, 56)),
                new SolidColorBrush(Color.FromArgb(255, 50, 63, 79)), new SolidColorBrush(Color.FromArgb(255, 47, 84, 150)), new SolidColorBrush(Color.FromArgb(255, 197, 90, 17)), 
                new SolidColorBrush(Color.FromArgb(255, 123, 123, 123)), new SolidColorBrush(Color.FromArgb(255, 191, 144, 0)), new SolidColorBrush(Color.FromArgb(255, 46, 117, 181)), 
                new SolidColorBrush(Color.FromArgb(255, 83, 129, 53)), 

                new SolidColorBrush(Color.FromArgb(255, 127, 127, 127)), new SolidColorBrush(Color.FromArgb(255, 12, 12, 12)), new SolidColorBrush(Color.FromArgb(255, 23, 22, 22)),
                new SolidColorBrush(Color.FromArgb(255, 34, 42, 53)), new SolidColorBrush(Color.FromArgb(255, 31, 56, 100)), new SolidColorBrush(Color.FromArgb(255, 131, 60, 11)), 
                new SolidColorBrush(Color.FromArgb(255, 82, 82, 82)), new SolidColorBrush(Color.FromArgb(255, 127, 96, 0)), new SolidColorBrush(Color.FromArgb(255, 30, 78, 121)), 
                new SolidColorBrush(Color.FromArgb(255, 55, 86, 35)), 
            }));
        }

        private void AddDefaultColors() {
            colorGroups.Add(new ColorGroup("Standardfarben", new SolidColorBrush[] {
                new SolidColorBrush(Color.FromArgb(255, 192, 0, 0)), new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), new SolidColorBrush(Color.FromArgb(255, 255, 192, 0)),
                new SolidColorBrush(Color.FromArgb(255, 255, 255, 0)), new SolidColorBrush(Color.FromArgb(255, 146, 208, 80)), new SolidColorBrush(Color.FromArgb(255, 0, 176, 80)), 
                new SolidColorBrush(Color.FromArgb(255, 0, 176, 240)), new SolidColorBrush(Color.FromArgb(255, 0, 112, 192)), new SolidColorBrush(Color.FromArgb(255, 0, 32, 96)), 
                new SolidColorBrush(Color.FromArgb(255, 112, 48, 160)), 
            }));
        }

        

    }
}
