using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Views {
    struct MonthGroupKey : IComparable<MonthGroupKey> {
        private const int MONTHS_PER_YEAR = 12;

        private DateTime _dateTime;

        public MonthGroupKey(DateTime dateTime) {
            _dateTime = dateTime;
        }

        public int Month {
            get {
                return _dateTime.Month;
            }
        }
        public int Year {
            get {
                return _dateTime.Year;
            }
        }

        private int CombineMonthYear() {
            return Month + Year * MONTHS_PER_YEAR;
        }

        public int CompareTo(MonthGroupKey other) {
            int ownMonth = CombineMonthYear();
            int otherMonth = other.CombineMonthYear();

            return ownMonth.CompareTo(otherMonth);
        }

        public override string ToString() {
            return _dateTime.ToString("MMMM") + " " + _dateTime.Year.ToString();
        }

        public override bool Equals(object obj) {
            return obj is MonthGroupKey key &&
                   Month == key.Month &&
                   Year == key.Year;
        }

        public override int GetHashCode() {
            int hashCode = -834659671;
            hashCode = hashCode * -1521134295 + Month.GetHashCode();
            hashCode = hashCode * -1521134295 + Year.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(MonthGroupKey left, MonthGroupKey right) {
            return left.Equals(right);
        }

        public static bool operator !=(MonthGroupKey left, MonthGroupKey right) {
            return !(left == right);
        }
    }
}
