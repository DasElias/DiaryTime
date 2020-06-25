using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Model {
    class DiaryEntry : DiaryEntryPreview {
        public DiaryEntry(DateTime date) : base(date) {
        }

        public DiaryEntry(DateTime date, string title, string plainText, string rtfText) : base(date, title, plainText) {
            RtfText = rtfText;
        }

        public string RtfText { get; private set; }

        public override void SetText(string plainText, string rtfText) {
            base.SetText(plainText, rtfText);
            RtfText = rtfText;

        }
    }
}
