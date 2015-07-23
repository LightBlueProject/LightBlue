using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace LightBlue.MultiHost.Infrastructure.Controls
{
    [TemplatePart(Name = TextBoxTemplatePart, Type = typeof(TextBox))]
    public class FifoLog : Control
    {
        private readonly int _maxLines;
        private readonly object _bufferLock = new object();
        private readonly LinkedList<string> _internalBuffer = new LinkedList<string>(new[] { string.Empty });
        private readonly DispatcherTimer _timer;
        private bool _needsDump;
        private TextBox _textBox;
        private const string TextBoxTemplatePart = "PART_LogContentDisplayer";

        static FifoLog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FifoLog), new FrameworkPropertyMetadata(typeof(FifoLog)));
        }

        public FifoLog(int maxLines)
        {
            _maxLines = maxLines;
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, OnTimerTick, Dispatcher);
            Loaded += (s, a) =>
            {
                DumpText();
                _timer.Start();
            };
            Unloaded += (s, a) => _timer.Stop();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBox = (TextBox)GetTemplateChild(TextBoxTemplatePart);
            _textBox.UndoLimit = 0;
            _textBox.SelectionChanged += TextWasSelected;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (_textBox != null && e.Key == Key.Enter && _textBox.SelectionLength > 0)
            {
                _textBox.SelectionLength = 0;
            }
        }

        private void TextWasSelected(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_textBox.SelectionLength == 0)
            {
                if (!_timer.IsEnabled) _timer.Start();
            }
            else
            {
                _timer.Stop();
            }
        }

        public void Write(string logItem)
        {
            if (logItem.EndsWith(Environment.NewLine))
            {
                lock (_bufferLock)
                {
                    _internalBuffer.AddLast(logItem);
                    if (_internalBuffer.Count > _maxLines)
                    {
                        _internalBuffer.RemoveFirst();
                    }
                }
            }
            else
            {
                lock (_bufferLock)
                {
                    _internalBuffer.Last.Value += logItem;
                }
            }
            _needsDump = true;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            DumpText();
        }

        private void DumpText()
        {
            if (_textBox == null)
            {
                return;
            }

            if (_needsDump)
            {
                _needsDump = false;
                lock (_bufferLock)
                {
                    UpdateTextInternal();
                    _textBox.CaretIndex = _textBox.Text.Length;
                    _textBox.ScrollToEnd();
                }
            }
        }

        private void UpdateTextInternal()
        {
            _textBox.SetValue(TextBox.TextProperty, string.Join(string.Empty, _internalBuffer));
        }

        public void WriteLine(string logItem)
        {
            Write(logItem + Environment.NewLine);
        }

        public void Clear()
        {
            lock (_bufferLock)
            {
                _internalBuffer.Clear();
            }
        }
    }
}
