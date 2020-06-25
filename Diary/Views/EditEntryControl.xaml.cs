﻿using Diary.Services;
using Diary.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.WebUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Diary.Views {
    public sealed partial class EditEntryControl : UserControl {
        public event RoutedEventHandler EntryChanged;

        private ColorService colorService = new ColorService();
        private SolidColorBrush currentColorBrush = null;

        private FontFamilyOptions fontFamilies = new FontFamilyOptions();
        private bool shouldSuppressFontFamilyChange = false;

        private FontSizeCollection fontSizes = new FontSizeCollection();
        private bool shouldSuppressFontSizeChange = false;

        public EditEntryControl() {
            this.InitializeComponent();
            this.Loaded += Control_Loaded;

            // init font family picker
            InitFontFamilyPicker();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e) {
            Editor.SelectionFlyout.ShouldConstrainToRootBounds = true;
        }

        public string RawText {
            get {
                string outString;
                Editor.Document.GetText(TextGetOptions.IncludeNumbering, out outString);
                return outString;
            }
        }

        public string RtfText {
            get {
                /*
                 * we remove trailing whitespaces as a workaround, because if we have an empty line at the end of the text,
                 * it will have an invalid font size when loading the text again in the Editor
                 */

                // get the actual size of the text
                Editor.Document.GetText(TextGetOptions.UseLf, out string text);
                text = text.TrimEnd();

                // get the text in the total range - to avoid getting extra lines
                var range = Editor.Document.GetRange(0, text.Length);
                range.GetText(TextGetOptions.FormatRtf, out string value);

                // return the value
                return value;
            }
            set {
                Editor.Document.SetText(TextSetOptions.FormatRtf, value);
                Editor.TextDocument.ClearUndoRedoHistory();
                UpdateUndoRedoButtons();
            }
        }

        public string Title {
            get {
                return titleBox.Text;
            }
            set {
                titleBox.Text = value;
            }
        }

        public bool IsSpellCheckingEnabled {
            get {
                return Editor.IsSpellCheckEnabled;
            }
            set {
                Editor.IsSpellCheckEnabled = value;
            }
        }


        public void Clear() {
            // by setting the text with LoadFromStream, we clear the undo/redo history
            using(var stream = StringFromStream.Generate("u")) {
                using(var memoryStream = stream.AsRandomAccessStream()) {
                    Editor.Document.LoadFromStream(TextSetOptions.None, memoryStream);
                }
            }

            Editor.Document.SetText(TextSetOptions.None, "a");
            Editor.Document.Selection.Expand(TextRangeUnit.Paragraph);

            string defaultFontSize = fontSizes[5];
            fontSizeBox.SelectedValue = defaultFontSize;

            Editor.Document.SetText(TextSetOptions.None, "");

            int defaultFontFamily = fontFamilies.IndexOf(fontFamilies.Find("Calibri"));
            fontFamilyBox.SelectedIndex = defaultFontFamily;


            //var undoLimit = Editor.Document.UndoLimit;
            //Editor.Document.UndoLimit = 0;
            //Editor.Document.UndoLimit = undoLimit;

            Editor.TextDocument.ClearUndoRedoHistory();
            UpdateUndoRedoButtons();
        }

        private int CountEmptyTrailingParagraphs() {
            string rawText = this.RawText;
            int counter = 0;

            for(int i = rawText.Length - 1; i >= 0; i--) {
                if(rawText[i] == '\r') counter++;
                else break;
            }

            return counter;
        }

        private void InitFontFamilyPicker() {
            foreach(FontFamily ff in fontFamilies) {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = ff;
                item.FontFamily = ff;
                fontFamilyBox.Items.Add(item);
            }

        }

        private void HandleUndoButton_Click(object sender, RoutedEventArgs e) {
            Editor.Document.Undo();
        }

        private void HandleRedoButton_Click(object sender, RoutedEventArgs e) {
            Editor.Document.Redo();
        }

        private void HandleBoldBtn_Click(object sender, RoutedEventArgs e) {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                selection.CharacterFormat.Bold = FormatEffect.Toggle;
            }
        }

        private void HandleItalicBtn_Click(object sender, RoutedEventArgs e) {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                selection.CharacterFormat.Italic = FormatEffect.Toggle;
            }
        }

        private void HandleUnderlineBtn_Click(object sender, RoutedEventArgs e) {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                ITextCharacterFormat charFormatting = selection.CharacterFormat;
                if(charFormatting.Underline == UnderlineType.None) {
                    charFormatting.Underline = UnderlineType.Single;
                } else {
                    charFormatting.Underline = UnderlineType.None;
                }
            }
        }

        private void HandleStrikeThroughBtn_Click(object sender, RoutedEventArgs e) {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                selection.CharacterFormat.Strikethrough = FormatEffect.Toggle;
            }
        }

        private void HandleAlignLeftBtn_Click(object sender, RoutedEventArgs e) {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                UpdateAlignmentButtonsToggleState();
            }
        }

        private void HandleAlignCenterBtn_Click(object sender, RoutedEventArgs e) {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                UpdateAlignmentButtonsToggleState();
            }
        }

        private void HandleAlignRightBtn_Click(object sender, RoutedEventArgs e) {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                UpdateAlignmentButtonsToggleState();
            }
        }

        private void HandleFontFamilyBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(e.AddedItems.Count == 0) return;
            if(shouldSuppressFontFamilyChange) {
                shouldSuppressFontFamilyChange = false;
                return;
            }

            FontFamilyWrapper ff = (e.AddedItems[0] as ComboBoxItem)?.Content as FontFamilyWrapper;
            if(ff != null) {
                ITextSelection selection = Editor.Document.Selection;
                if(selection != null) {
                    selection.CharacterFormat.Name = ff.ToString();
                }

                Editor.Focus(FocusState.Programmatic);
            }
        }

        private void HandleFontSizeBox_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            if(shouldSuppressFontSizeChange) {
                shouldSuppressFontSizeChange = false;
                return;
            }

            string fontSizeStringForSelection = fontSizeBox.SelectedValue.ToString();
            fontSizeStringForSelection = fontSizeStringForSelection.Replace(",", ".");
            double val = Convert.ToDouble(fontSizeStringForSelection);
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                selection.CharacterFormat.Size = (float) val;
            }

            Editor.Focus(FocusState.Programmatic);
        }

        private void HandleUnorderedListBtn_Click(object sender, RoutedEventArgs e) {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                ITextParagraphFormat paragraphFormat = selection.ParagraphFormat;
                if(paragraphFormat.ListType == MarkerType.Bullet) {
                    paragraphFormat.ListType = MarkerType.None;
                } else {
                    paragraphFormat.ListType = MarkerType.Bullet;
                }
            }
        }

        private void HandleOrderedListBtn_Click(object sender, RoutedEventArgs e) {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                ITextParagraphFormat paragraphFormat = selection.ParagraphFormat;
                if(paragraphFormat.ListType == MarkerType.Arabic) {
                    paragraphFormat.ListType = MarkerType.None;
                } else {
                    paragraphFormat.ListType = MarkerType.Arabic;
                }
            }
        }

        private void HandleTextColorButton_Click(SplitButton sender, SplitButtonClickEventArgs args) {
            // button part of the split button was clicked
            ChangeColor();
        }

        private void HandleTextColorButton_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // flyout part
            currentColorBrush = (SolidColorBrush) e.AddedItems[0];
            currentSelectedColorRectangle.Fill = currentColorBrush;
            ChangeColor();
            textColorButtonFlyout.Hide();
        }

        private void ChangeColor() {
            ITextSelection selection = Editor.Document.Selection;
            if(selection != null) {
                ITextCharacterFormat charFormat = selection.CharacterFormat;
                charFormat.ForegroundColor = currentColorBrush.Color;
                selection.CharacterFormat = charFormat;
            }
        }

        private void HandleTemplatePicker_ApplyStyleTemplate(StyleTemplateButton sender, StyleTemplate chosenTemplate) {
            chosenTemplate.Apply(Editor.Document.Selection);
        }

        private void UpdateFormatBar() {
            UpdateFontButtonsToggleState();
            UpdateAlignmentButtonsToggleState();
            UpdateFontFamilyChooser();
            UpdateFontSizeChooser();
        }

        private void HandleEditor_SelectionChanged(object sender, RoutedEventArgs e) {
            UpdateFormatBar();
        }

        private void UpdateFontButtonsToggleState() {
            boldButton.IsChecked = (Editor.Document.Selection.CharacterFormat.Bold == FormatEffect.On);
            italicButton.IsChecked = (Editor.Document.Selection.CharacterFormat.Italic == FormatEffect.On);
            underlineButton.IsChecked = (Editor.Document.Selection.CharacterFormat.Underline == UnderlineType.Single);
            strikeButton.IsChecked = (Editor.Document.Selection.CharacterFormat.Strikethrough == FormatEffect.On);

        }

        private void UpdateAlignmentButtonsToggleState() {
            alignLeftButton.IsChecked = (Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Left);
            alignCenterButton.IsChecked = (Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Center);
            alignRightButton.IsChecked = (Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Right);

        }

        private void UpdateFontFamilyChooser() {
            string currentFontInSelection = Editor.Document.Selection.CharacterFormat.Name;
            string currentFontInBox = (fontFamilyBox.SelectedValue as ComboBoxItem)?.Content.ToString();
            if(currentFontInSelection != currentFontInBox) {
                shouldSuppressFontFamilyChange = true;
                fontFamilyBox.SelectedIndex = fontFamilies.FindIndex(currentFontInSelection);
            }
        }

        private void UpdateFontSizeChooser() {
            float currentFontSizeInSelection = Editor.Document.Selection.CharacterFormat.Size;
            double currentFontSizeInBox = Convert.ToDouble(fontSizeBox.SelectedValue);
            //if(FloatingPointComparison.IsEqual(currentFontSizeInSelection, 9.8f, 0.01f)) {
            //    Editor.Document.Selection.CharacterFormat.Size = (float) currentFontSizeInBox;
            //    return;
            //}

            double delta = Math.Abs(currentFontSizeInSelection - currentFontSizeInBox);
            const double maxDelta = 0.01f;
            if(delta > maxDelta) {
                shouldSuppressFontSizeChange = true;
                fontSizeBox.SelectedValue = Convert.ToString(currentFontSizeInSelection);
            }
        }

        private void HandleBackground_PointerPressed(object sender, PointerRoutedEventArgs e) {
            // prevent focus loss for the editor
            e.Handled = true;
        }

        private void HandleEditor_TextChanged(object sender, RoutedEventArgs e) {
            EntryChanged.Invoke(sender, e);
            UpdateUndoRedoButtons();
        }

        private void HandleTextChanged_TitleBox(object sender, TextChangedEventArgs e) {
            EntryChanged.Invoke(sender, e);
        }

        private void UpdateUndoRedoButtons() {
            undoButton.IsEnabled = Editor.Document.CanUndo();
            redoButton.IsEnabled = Editor.Document.CanRedo();
        }
    }
}