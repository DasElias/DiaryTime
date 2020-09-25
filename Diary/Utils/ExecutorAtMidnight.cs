using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Diary.Utils {
    class ExecutorAtMidnight {
        private Action action;

        public ExecutorAtMidnight(Action action) {
            this.action = action;
            Solution2();
        }

        private async void Solution2() {
            while(true) {
                var nextMidnight = DateTime.Today.AddDays(1);
                double msToNextMidnight = (nextMidnight - DateTime.Now).TotalMilliseconds;

                await Task.Delay((int) msToNextMidnight);
                action();
            }
        }
    }
}

