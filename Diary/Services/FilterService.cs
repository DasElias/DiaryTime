using Diary.Model;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    static class FilterService {
        public static bool ShouldDisplayEntry(DiaryEntryPreview entry, string searchBoxText) {
            if(IsInTitleOrText(entry, searchBoxText)) return true;
            if(IsInYear(entry, searchBoxText)) return true;
            if(IsInMonth(entry, searchBoxText)) return true;
            if(IsInYear(entry, searchBoxText)) return true;

            return false;
        }

        private static bool IsInTitleOrText(DiaryEntryPreview entry, string searchBoxText) {
            bool isInText = entry.PlainContent.Contains(searchBoxText, StringComparison.InvariantCultureIgnoreCase);
            bool isInTitle = entry.Title.Contains(searchBoxText, StringComparison.InvariantCultureIgnoreCase);
            return isInText || isInTitle;
        }

        private static bool IsInYear(DiaryEntryPreview entry, string searchBoxText) {
            string trimmedSearchBoxText = searchBoxText.Trim();
            if(trimmedSearchBoxText.Length == 4) {
                int year;
                if(int.TryParse(trimmedSearchBoxText, out year)) {
                    return entry.Date.Year == year;
                }
            }

            return false;
        }

        private static bool IsInMonth(DiaryEntryPreview entry, string searchBoxText) {
            string trimmedSearchBoxText = searchBoxText.Trim();
            return entry.Date.ToString("MMMM") == trimmedSearchBoxText;
        }

        private static bool IsInDate(DiaryEntryPreview entry, string searchBoxText) {
            string trimmedSearchBoxText = searchBoxText.Trim();

            DateTime parsedDate;
            if(DateTime.TryParse(trimmedSearchBoxText, out parsedDate)) {
                return DateUtils.CompareDay(parsedDate, entry.Date);
            }

            return false;
        }
    }
}
