using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LightBlue.MultiHost.Controls
{
    /// <summary>
    ///     Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification
    {
        private readonly TaskCompletionSource<object> _removeTask;

        public Notification()
        {
            InitializeComponent();
            MouseLeave += OnMouseLeave;
            MouseEnter += OnMouseEnter;
            _removeTask = new TaskCompletionSource<object>();
            PreviewMouseUp += OnMakeVisible;
        }

        private void OnMakeVisible(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = App.Current.MainWindow;
            if (mainWindow.WindowState == WindowState.Minimized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToElementState(this, "MouseEnter", true);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToElementState(this, "MouseLeave", true);
        }

        public Task Disappear()
        {
            VisualStateManager.GoToElementState(this, "Disappear", true);
            return _removeTask.Task;
        }

        private void Disappeared(object sender, EventArgs e)
        {
            _removeTask.TrySetResult(null);
        }
    }
}