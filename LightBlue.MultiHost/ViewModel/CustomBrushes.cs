using System.Windows.Media;
using LightBlue.MultiHost.Configuration;

namespace LightBlue.MultiHost.ViewModel
{
    class CustomBrushes
    {
        public static SolidColorBrush BloodRed = new SolidColorBrush(new Color { R = 170, G = 0, B = 0, A = 255 });
        public static SolidColorBrush Green = new SolidColorBrush(new Color { R = 44, G = 160, B = 44, A = 255 });
        public static SolidColorBrush Grey = Brushes.Gray;
        public static SolidColorBrush Blue = Brushes.SteelBlue;
        public static SolidColorBrush Main = Brushes.DodgerBlue;

        public static SolidColorBrush GetStatusColour(RoleStatus status, RunnerType runnerType)
        {
            switch (status)
            {
                case RoleStatus.Starting:
                    return runnerType == RunnerType.Thread
                        ? Green
                        : Blue;
                case RoleStatus.Running:
                    return runnerType == RunnerType.Thread
                        ? Green
                        : Blue;
                case RoleStatus.Stopping:
                    return BloodRed;
                case RoleStatus.Crashing:
                    return BloodRed;
                case RoleStatus.Recycling:
                    return BloodRed;
                case RoleStatus.Stopped:
                    return Grey;
                case RoleStatus.Sequenced:
                    return Grey;
                default:
                    return Grey;
            }
        }
    }
}
