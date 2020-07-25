using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Resources;

namespace Diary.Utils {
    class XamlResourceLoader : CustomXamlResourceLoader {
        private readonly ResourceLoader _loader;

        public XamlResourceLoader() {
            _loader = ResourceLoader.GetForViewIndependentUse();
        }

        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType) {
            return _loader.GetString(resourceId) ?? resourceId;
        }
    }
}
