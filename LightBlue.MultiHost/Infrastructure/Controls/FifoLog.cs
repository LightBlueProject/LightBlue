using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace LightBlue.MultiHost.Infrastructure.Controls
{
    public class FifoLog : Control
    {
        static FifoLog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FifoLog), new FrameworkPropertyMetadata(typeof(FifoLog)));
        }

        public static readonly DependencyProperty TextBoxProperty =
            DependencyProperty.Register(
                "TextBox",
                typeof(TextBox),
                typeof(FifoLog));

        public TextBox TextBox
        {
            get { return (TextBox)GetValue(TextBoxProperty); }
            set { SetValue(TextBoxProperty, value); }
        }

        private readonly int _capacity;
        private int _size;
        private int _selectionStart;
        private int _selectionEnd;

        public FifoLog(int capacity)
        {
            _capacity = capacity;
            TextBox = new TextBox();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            TextBox.ScrollToEnd();
        }

        public void Write(string logItem)
        {
            // Check if new log exceeds capacity
            var remaining = _capacity - _size;

            if (logItem.Length > remaining)
            {
                // Reduce size
                var sizeToReduce = logItem.Length - remaining;

                SaveSelection();

                sizeToReduce = GetValidUnicodeSizeToReduce(sizeToReduce);

                TextBox.Select(0, sizeToReduce);
                TextBox.SelectedText = string.Empty;
                _size -= sizeToReduce;

                TransformSavedSelection(sizeToReduce);
                ReapplySavedSelection();
            }

            TextBox.AppendText(logItem);
            _size += logItem.Length;
            TextBox.ScrollToEnd();
        }

        public void WriteLine(string logItem)
        {
            Write(logItem + "\r\n");
        }

        public void Clear()
        {
            TextBox.Clear();
            _size = 0;
        }

        private void SaveSelection()
        {
            _selectionStart = TextBox.SelectionStart;
            _selectionEnd = _selectionStart + TextBox.SelectionLength;
        }

        private void TransformSavedSelection(int sizeToReduce)
        {
            _selectionStart = Math.Max(0, _selectionStart - sizeToReduce);
            _selectionEnd = Math.Max(0, _selectionEnd - sizeToReduce);
        }

        private void ReapplySavedSelection()
        {
            TextBox.Select(_selectionStart, _selectionEnd - _selectionStart);
        }

        private int GetValidUnicodeSizeToReduce(int sizeToReduce)
        {
            TextBox.Select(0, sizeToReduce);

            if (IsInvalidUnicode(TextBox.SelectedText))
            {
                sizeToReduce++;
            }

            return sizeToReduce;
        }

        private static bool IsInvalidUnicode(string text)
        {
            // https://mnaoumov.wordpress.com/2014/06/14/stripping-invalid-characters-from-utf-16-strings/
            var invalidCharactersRegex = new Regex("([\ud800-\udbff](?![\udc00-\udfff]))|((?<![\ud800-\udbff])[\udc00-\udfff])");
            return invalidCharactersRegex.IsMatch(text);
        }
    }
}
