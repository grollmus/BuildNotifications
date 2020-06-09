using System.Linq;
using System.Windows;
using System.Windows.Forms;
using JetBrains.Annotations;
using NLog.Fluent;

namespace BuildNotifications.Services
{
    internal class WindowSettings
    {
        public WindowSettings()
        {
            Top = Left = 0;
            Width = 800;
            Height = 450;
        }

        [UsedImplicitly]
        public double Height { get; set; }

        [UsedImplicitly]
        public double Left { get; set; }

        [UsedImplicitly]
        public WindowState State { get; set; }

        [UsedImplicitly]
        public double Top { get; set; }

        [UsedImplicitly]
        public double Width { get; set; }

        public void ApplyTo(Window window)
        {
            var size = EnsureWindowSize();
            var pos = EnsureWindowVisibility(size);

            window.Width = size.X;
            window.Height = size.Y;
            window.Left = pos.X;
            window.Top = pos.Y;
            window.WindowState = State;
        }

        private Point EnsureWindowSize()
        {
            var x = Width;
            var y = Height;

            Log.Debug().Message($"Loaded Window size: {x}|{y}").Write();
            var screen = VirtualScreen.FromPosition(x, y);

            // Make sure the window is at most as large as the available screen space.
            if (x > screen.Width)
                x = screen.Width;
            if (y > screen.Height)
                y = screen.Height;

            Log.Debug().Message($"Clipped Window size: {x}|{y}").Write();
            return new Point(x, y);
        }

        private Point EnsureWindowVisibility(Point size)
        {
            var x = Left;
            var y = Top;

            Log.Debug().Message($"Loaded Window position: {x}|{y}").Write();
            var screen = VirtualScreen.FromPosition(x, y);

            // If the window is half off the screen, move it into view
            if (y + size.Y / 2 > screen.Height)
                y = screen.Height - size.Y;
            if (x + size.X / 2 > screen.Width)
                x = screen.Width - size.X;

            // Ensure window does not hang off screen
            if (y < screen.Top)
                y = screen.Top;
            if (x < screen.Left)
                x = screen.Left;

            Log.Debug().Message($"Clipped Window position: {x}|{y}").Write();
            return new Point(x, y);
        }

        private class VirtualScreen
        {
            private VirtualScreen(double left, double top, double width, double height)
            {
                Left = left;
                Top = top;
                Width = width;
                Height = height;
            }

            public double Height { get; }
            public double Left { get; }
            public double Top { get; }
            public double Width { get; }

            public static VirtualScreen FromPosition(double x, double y)
            {
                var allScreens = Screen.AllScreens;

                foreach (var screen in allScreens)
                {
                    if (screen.Bounds.Contains((int) x, (int) y))
                        return new VirtualScreen(screen.Bounds.Left, screen.Bounds.Top, screen.Bounds.Width, screen.Bounds.Height);
                }

                var primaryScreen = allScreens.First(x => x.Primary);
                return new VirtualScreen(primaryScreen.Bounds.Left, primaryScreen.Bounds.Top, primaryScreen.Bounds.Width, primaryScreen.Bounds.Height);
            }
        }
    }
}