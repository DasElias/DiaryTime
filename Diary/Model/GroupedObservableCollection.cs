using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Diary.Model {
    class GroupedObservableCollection<K, E> : ObservableCollection<Grouping<K, E>> where K : IComparable<K> where E : IComparable<E> {
        private readonly Func<E, K> readKey;
        private Grouping<K, E> lastAffectedGroup;

        public GroupedObservableCollection(Func<E, K> readKey) {
            this.readKey = readKey;
        }

        public GroupedObservableCollection(Func<E, K> readKey, IEnumerable<E> items) : this(readKey) {
            foreach(var i in items) {
                this.Add(i);
            }
        }

        public void Add(E item) {
            var key = readKey(item);
            var group = FindOrCreateGroup(key);
            for(int i = 0; i < group.Count; i++) {
                var elemAtPos = group[i];
                if(elemAtPos.CompareTo(item) <= 0) {
                    group.Insert(i, item);
                    return;
                }
            }

            group.Add(item);
        }

        public void Remove(E item) {
            K key = readKey(item);
            int groupIndexToRemove = -1;


            for(int i = 0; i < Count; i++) {
                var grouping = this[i];

                if(grouping.Key.Equals(key)) {
                    grouping.Remove(item);
                    if(grouping.Count == 0) {
                        groupIndexToRemove = i;
                        break;
                    } else {
                        return;
                    }
                }
            }

            if(groupIndexToRemove > 0) {
                RemoveGroupAt(groupIndexToRemove);
            }
        }

        public void RemoveAll(Predicate<E> shouldRemove) {
            for(int i = Count - 1; i >= 0; i--) {
                var grouping = this[i];

                for(int j = grouping.Count - 1; j >= 0; j--) {
                    var elem = grouping[j];
                    if(shouldRemove(elem)) {
                        grouping.RemoveAt(j);
                    }
                }

                if(grouping.Count == 0) {
                    RemoveGroupAt(i);
                }
            }
        }

        private void RemoveGroupAt(int index) {
            var group = this[index];
            if(group == lastAffectedGroup) {
                lastAffectedGroup = null;
            }

            RemoveAt(index);
        }

        public bool Contains(E elemToFind) {
            K key = readKey(elemToFind);
            foreach(var grouping in this) {
                if(grouping.Key.Equals(key)) {
                    foreach(var e in grouping) {
                        if(e.Equals(elemToFind)) return true;
                    }

                    return false;
                }
            }

            return false;
        }

        private Grouping<K, E> FindOrCreateGroup(K keyToFind) {
            if(lastAffectedGroup != null && lastAffectedGroup.Key.Equals(keyToFind)) {
                return lastAffectedGroup;
            }

            var groupsWithIndex = this.Select((group, index) => new { group, index });
            // returns null if group wasn't found
            var match = groupsWithIndex.FirstOrDefault(elem => elem.group.Key.CompareTo(keyToFind) <= 0);

            Grouping<K, E> result;
            if(match == null) {
                // group doesn't exist
                result = new Grouping<K, E>(keyToFind);
                Add(result);
            } else if(!match.group.Key.Equals(keyToFind)) {
                // group doesn't exist
                result = new Grouping<K, E>(keyToFind);
                this.Insert(match.index, result);
            } else {
                result = match.group;
            }

            this.lastAffectedGroup = result;
            return result;
        }
    }
}
