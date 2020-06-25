using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Utils {
    static class FloatingPointComparison {
        public static bool IsEqual(float a, float b, float delta) {
            return Math.Abs(a - b) < delta;
        }
    }
}
