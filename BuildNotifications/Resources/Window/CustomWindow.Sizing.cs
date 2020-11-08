using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace BuildNotifications.Resources.Window
{
    public partial class CustomWindow
    {
        private Thickness GetDefaultMarginForDpi()
        {
            var currentDpi = SystemHelper.GetCurrentDpi();
            var thickness = new Thickness(8, 8, 8, 8);
            switch (currentDpi)
            {
                case 120:
                    thickness = new Thickness(7, 7, 4, 5);
                    break;
                case 144:
                    thickness = new Thickness(7, 7, 3, 1);
                    break;
                case 168:
                    thickness = new Thickness(6, 6, 2, 0);
                    break;
                case 192:
                case 240:
                    thickness = new Thickness(6, 6, 0, 0);
                    break;
            }

            return thickness;
        }

        private static Thickness GetFromMinimizedMarginForDpi()
        {
            var currentDpi = SystemHelper.GetCurrentDpi();
            var thickness = new Thickness(7, 7, 5, 7);
            switch (currentDpi)
            {
                case 120:
                    thickness = new Thickness(6, 6, 4, 6);
                    break;
                case 144:
                    thickness = new Thickness(7, 7, 4, 4);
                    break;
                case 168:
                case 192:
                    thickness = new Thickness(6, 6, 2, 2);
                    break;
                case 240:
                    thickness = new Thickness(6, 6, 0, 0);
                    break;
            }

            return thickness;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var width = (double) screen.WorkingArea.Width;
            var workingArea = screen.WorkingArea;
            _previousScreenBounds = new Point(width, workingArea.Height);
        }

        private void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseButtonDown = false;
            _isManualDrag = false;
            ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMouseButtonDown)
                return;

            var currentDpiScaleFactor = SystemHelper.GetCurrentDpiScaleFactor();
            var position = e.GetPosition(this);
            var screen = PointToScreen(position);
            var x = _mouseDownPosition.X - position.X;
            var y = _mouseDownPosition.Y - position.Y;

            if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) > 1)
            {
                var actualWidth = _mouseDownPosition.X;

                if (_mouseDownPosition.X <= 0)
                    actualWidth = 0;
                else if (_mouseDownPosition.X >= ActualWidth)
                    actualWidth = _widthBeforeMaximize;

                if (WindowState == WindowState.Maximized)
                {
                    ToggleWindowState();
                    Top = (screen.Y - position.Y) / currentDpiScaleFactor;
                    Left = (screen.X - actualWidth) / currentDpiScaleFactor;
                    CaptureMouse();
                }

                _isManualDrag = true;

                Top = (screen.Y - _mouseDownPosition.Y) / currentDpiScaleFactor;
                Left = (screen.X - actualWidth) / currentDpiScaleFactor;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                _widthBeforeMaximize = ActualWidth;
                return;
            }

            if (WindowState == WindowState.Maximized)
            {
                var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
                if (Math.Abs(_previousScreenBounds.X - screen.WorkingArea.Width) > DoubleTolerance ||
                    Math.Abs(_previousScreenBounds.Y - screen.WorkingArea.Height) > DoubleTolerance)
                {
                    var width = (double) screen.WorkingArea.Width;
                    var workingArea = screen.WorkingArea;
                    _previousScreenBounds = new Point(width, workingArea.Height);
                    RefreshWindowState();
                }
            }
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var thickness = new Thickness(0);
            if (WindowState != WindowState.Maximized)
            {
                var currentDpiScaleFactor = SystemHelper.GetCurrentDpiScaleFactor();
                var workingArea = screen.WorkingArea;
                MaxHeight = (workingArea.Height + 16) / currentDpiScaleFactor;
                MaxWidth = double.PositiveInfinity;

                if (WindowState != WindowState.Maximized)
                    SetMaximizeButtonsVisibility(Visibility.Visible, Visibility.Collapsed);
            }
            else
            {
                thickness = GetDefaultMarginForDpi();
                if (_previousState == WindowState.Minimized ||
                    Math.Abs(Left - _positionBeforeDrag.X) < DoubleTolerance &&
                    Math.Abs(Top - _positionBeforeDrag.Y) < DoubleTolerance)
                    thickness = GetFromMinimizedMarginForDpi();

                SetMaximizeButtonsVisibility(Visibility.Collapsed, Visibility.Visible);
            }

            _layoutRoot.Margin = thickness;
            _previousState = WindowState;
        }

        private void RefreshWindowState()
        {
            if (WindowState == WindowState.Maximized)
            {
                ToggleWindowState();
                ToggleWindowState();
            }
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var width = (double) screen.WorkingArea.Width;
            var workingArea = screen.WorkingArea;
            _previousScreenBounds = new Point(width, workingArea.Height);
            RefreshWindowState();
        }

        private const double DoubleTolerance = 0.000001;
    }
}