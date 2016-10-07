using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LightBlue.MultiHost
{
    class NotificationAdorner : Adorner
    {
        private Control _child;

        public NotificationAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException();
            return _child;
        }

        public Control Child
        {
            get { return _child; }
            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                }
                _child = value;
                if (_child != null)
                {
                    AddVisualChild(_child);
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var location = new Point(finalSize.Width - _child.DesiredSize.Width, 0);
            var finalRect = new Rect(location, _child.DesiredSize);

            _child.Arrange(finalRect);
            Trace.WriteLine("Measure - " + finalRect );

            return finalSize;
        }
    }
}