using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace LightBlue.MultiHost.Infrastructure.Controls
{
    [TemplatePart(Name = TextBoxTemplatePart, Type = typeof(TextBoxBase))]
    public class FifoLog : Control
    {
        private readonly int _maxLines;
        private readonly object _bufferLock = new object();
        private readonly LinkedList<string> _internalBuffer = new LinkedList<string>(new[] { string.Empty });
        private readonly DispatcherTimer _timer;
        private bool _needsDump;
        private TextBoxBase _textBox;
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
                    var content = string.Join(string.Empty, _internalBuffer);
                    UpdateTextInternal(content);
                    _textBox.ScrollToEnd();
                }
            }
        }

        private void UpdateTextInternal(string content)
        {
            _textBox.SetValue(TextBox.TextProperty, content);
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
