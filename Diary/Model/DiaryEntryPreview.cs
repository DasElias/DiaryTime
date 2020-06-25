using Diary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Model {
    class DiaryEntryPreview : IEquatable<DiaryEntryPreview>, IComparable<DiaryEntryPreview> {
        public DiaryEntryPreview(DateTime date){
            Date = date;
            Title = "Tagebucheintrag vom " + DateUtils.ToDateString(date);
            PlainContent = "";
        }

        public DiaryEntryPreview(DateTime date, string title, string plainContent) {
            Date = date;
            Title = title;
            PlainContent = plainContent;
        }

        public DateTime Date { get; }
        public string Title { get; set; }
        public string PlainContent { get; private set; }
        public string TrimmedPlainContent {
            get {
                return PlainContent.Trim();
            }
        }

        public string DateString {
            get {
                return Date.ToString("d.M.yyyy");
            }
        }

        public bool IsToday {
            get {
                return DateUtils.IsToday(Date);
            }
        }

        public virtual void SetText(string plainText, string rtfText) {
            PlainContent = plainText;
        }

        public override bool Equals(object obj) {
            return Equals(obj as DiaryEntryPreview);
        }

        public bool Equals(DiaryEntryPreview other) {
            return other != null &&
                   DateUtils.CompareDay(Date, other.Date);
        }

        public override int GetHashCode() {
            return 884517729 + Date.GetHashCode();
        }

        public int CompareTo(DiaryEntryPreview other) {
            return Date.CompareTo(other.Date);
        }

        public static bool operator==(DiaryEntryPreview left, DiaryEntryPreview right) {
            return EqualityComparer<DiaryEntryPreview>.Default.Equals(left, right);
        }

        public static bool operator !=(DiaryEntryPreview left, DiaryEntryPreview right) {
            return !(left == right);
        }
    }
}
