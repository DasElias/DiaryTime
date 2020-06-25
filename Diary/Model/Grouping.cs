using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Model {
	class Grouping<K, T> : ObservableCollection<T> {
		public K Key { get; private set; }

		public Grouping(K key) {
			Key = key;
        }

		public Grouping(K key, IEnumerable<T> items) {
			Key = key;
			foreach(var i in items) {
				this.Items.Add(i);
            }
        }
	}
}
