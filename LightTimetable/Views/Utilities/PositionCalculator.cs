using System.Windows;
using System.Windows.Controls;
using LightTimetable.Models.Enums;

namespace LightTimetable.Views.Utilities
{
    public static class PositionCalculator
    {
        private static Dock? _taskbarPosition;
        private static double _dockTop;
        private static double _dockLeft;
        private static double _dockRight;
        private static double _dockBottom;

        public static SizeChangedEventHandler Calculate(WindowPosition winPosition)
        {
            _taskbarPosition = GetCurrentTaskbarPosition();
            var taskbarSize = GetCurrentTaskbarHeight();

            _dockTop = _taskbarPosition == Dock.Top ? taskbarSize : 0;
            _dockLeft = _taskbarPosition == Dock.Left ? taskbarSize : 0;
            _dockRight = _taskbarPosition == Dock.Right ? SystemParameters.PrimaryScreenWidth - taskbarSize : SystemParameters.PrimaryScreenWidth;
            _dockBottom = _taskbarPosition == Dock.Bottom ? SystemParameters.PrimaryScreenHeight - taskbarSize : SystemParameters.PrimaryScreenHeight;

            return GetSuitableDelegate(winPosition);
        }

        private static SizeChangedEventHandler GetSuitableDelegate(WindowPosition winPosition)
        {
            return winPosition switch
            {
                WindowPosition.TopLeft => TopLeftSizeChanged,
                WindowPosition.TopRight => TopRightSizeChanged,
                WindowPosition.BottomLeft => BottomLeftSizeChanged,
                WindowPosition.BottomRight => BottomRightSizeChanged,
                _ => BottomRightSizeChanged
            };
        }

        private static void TopLeftSizeChanged(object s, SizeChangedEventArgs? e)
        {
            var win = (Window)s;
            win.Top = _dockTop;
            win.Left = _dockLeft;
        }

        private static void TopRightSizeChanged(object s, SizeChangedEventArgs? e)
        {
            var win = (Window)s;
            var width = e == null ? win.Width : e.NewSize.Width;

            win.Top = _dockTop;
            win.Left = _dockRight - width;
        }

        private static void BottomLeftSizeChanged(object s, SizeChangedEventArgs? e)
        {
            var win = (Window)s;
            var height = e == null ? win.Height : e.NewSize.Height;

            win.Left = _dockLeft;
            win.Top = _dockBottom - height;
        }

        private static void BottomRightSizeChanged(object s, SizeChangedEventArgs? e)
        {
            var win = (Window)s;

            var width = e == null ? win.Width : e.NewSize.Width;
            var height = e == null ? win.Height : e.NewSize.Height;

            win.Top = _dockBottom - height;
            win.Left = _dockRight - width;
        }

        #region Initialization

        private static double GetCurrentTaskbarHeight()
        {
            return _taskbarPosition switch
            {
                Dock.Top => SystemParameters.WorkArea.Top,
                Dock.Bottom => SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Bottom,
                Dock.Left => SystemParameters.WorkArea.Left,
                Dock.Right => SystemParameters.PrimaryScreenWidth - SystemParameters.WorkArea.Right,
                // If taskbar is not in a fixed position
                _ => 0
            };
        }

        private static Dock? GetCurrentTaskbarPosition()
        {
            if (SystemParameters.WorkArea.Left > 0)
                return Dock.Left;

            if (SystemParameters.WorkArea.Top > 0)
                return Dock.Top;

            if (SystemParameters.WorkArea.Left == 0
                && SystemParameters.WorkArea.Width < SystemParameters.PrimaryScreenWidth)
                return Dock.Right;

            if (SystemParameters.WorkArea.Bottom < SystemParameters.PrimaryScreenHeight)
                return Dock.Bottom;

            // If taskbar is not in a fixed position
            return null;
        }

        #endregion

    }
}
