using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace LightBlue.MultiHost.Controls
{
    public static class NotificationHub
    {
        private static readonly ObservableCollection<Notification> Notifications = new ObservableCollection<Notification>();
        private static Dispatcher _dispatcher;
        private static Popup _notificationWindow;
        private static DispatcherTimer _adornerRemoveTimer;

        public static void Initialise(Window owner)
        {
            _dispatcher = owner.Dispatcher;
            _notificationWindow = new Popup()
            {
                Child = new ItemsControl
                {
                    ItemsSource = Notifications
                },
                Placement = PlacementMode.Custom,
                AllowsTransparency = true,
                IsOpen = true
            };

            _notificationWindow.CustomPopupPlacementCallback += PlacePopup;

            _adornerRemoveTimer = new DispatcherTimer(DispatcherPriority.Normal, owner.Dispatcher)
            {
                Interval = TimeSpan.FromSeconds(1),
                IsEnabled = true
            };
            _adornerRemoveTimer.Tick += ClearAdorners;

        }

        private static CustomPopupPlacement[] PlacePopup(Size popupsize, Size targetsize, Point offset)
        {
            return new[]
            {
                new CustomPopupPlacement(new Point(SystemParameters.FullPrimaryScreenWidth - popupsize.Width, 0),
                    PopupPrimaryAxis.None),
            };
        }

        private static void ClearAdorners(object sender, EventArgs e)
        {
            var toRemove = new List<Notification>();
            foreach (var adorner in Notifications)
            {
                if (adorner.IsMouseOver)
                {
                    adorner.Tag = TimeSpan.FromSeconds(5);
                }
                else
                {
                    var ttl = TimeSpan.FromSeconds(((TimeSpan) adorner.Tag).TotalSeconds - 1);
                    if (ttl <= TimeSpan.Zero && !adorner.IsMouseOver)
                    {
                        toRemove.Add(adorner);
                    }
                    adorner.Tag = ttl;
                }
            }
            foreach (var adorner in toRemove)
            {
                RemoveAdorner(adorner);
            }
        }

        private static async void RemoveAdorner(Notification adorner)
        {
            await adorner.Disappear();
            Notifications.Remove(adorner);
        }

        public static void Notify(string title, string content, Action onclick)
        {
            EnsureUiThread(() =>
            {
                var notification = new Notification
                {
                    titleText = { Command = new DelegateCommand(onclick) },
                    contentText = { Text = content },
                    Tag = TimeSpan.FromSeconds(5),
                };
                notification.titleText.Inlines.Add(title);
                notification.MouseUp += (s, a) => onclick();
                notification.dismissButton.Click += (s, a) => RemoveAdorner(notification);
                Notifications.Add(notification);

            });
        }

        private static void EnsureUiThread(Action a)
        {
            if (_dispatcher.CheckAccess())
            {
                a();
                return;
            }
            _dispatcher.Invoke(a);
        }

        private class DelegateCommand : ICommand
        {
            private readonly Action _onclick;

            public DelegateCommand(Action onclick)
            {
                _onclick = onclick;
                var warnAsError = CanExecuteChanged;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                _onclick();
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}