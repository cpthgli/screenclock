using Microsoft.VisualBasic;
using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace screenclock_movie
{
    /// <summary>
    /// ScreenSaver.xaml の相互作用ロジック
    /// </summary>
    public partial class ScreenSaverWindow : Window, IDisposable
    {
        #region Win32 API functions
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);
        #endregion

        private Point? MouseLocation { get; set; }
        private bool PreviewMode { get; set; }
        private Timer Timer { get; set; }
        private DateTime Time { get; set; }

        public ScreenSaverWindow()
        {
            this.PreviewMode = false;
            this.InitializeComponent();
            this.Timer = new Timer();
        }

        public ScreenSaverWindow(System.Drawing.Rectangle bounds, string path, double blur)
        {
            this.PreviewMode = false;
            this.InitializeComponent();

            var dpi = VisualTreeHelper.GetDpi(this);

            this.Width = bounds.Width / dpi.DpiScaleX;
            this.Height = bounds.Height / dpi.DpiScaleY;
            this.Left = bounds.Left / dpi.DpiScaleX;
            this.Top = bounds.Top / dpi.DpiScaleY;

            this.Movie.Source = string.IsNullOrWhiteSpace(path) ? null : new Uri(path);
            this.Blur.Radius = blur;

            var now = DateTime.Now;
            this.SetTime(now);
            this.Time = now;

            this.Timer = new Timer(100);
            this.Timer.Elapsed += (_, e) =>
            {
                if (e.SignalTime.Hour != this.Time.Hour || e.SignalTime.Minute != this.Time.Minute)
                {
                    this.SetTime(e.SignalTime);
                    this.Time = e.SignalTime;
                }
            };
            this.Timer.Start();
        }

        private void SetTime(DateTime time)
        {
            var t = time.ToString("HHmm", CultureInfo.InvariantCulture).ToArray();
            this.Dispatcher.Invoke(() =>
            {
                this.H1.Text = t[0].ToString();
                this.H2.Text = t[1].ToString();
                this.M1.Text = t[2].ToString();
                this.M2.Text = t[3].ToString();
            });
        }

        public ScreenSaverWindow(IntPtr PreviewWndHandle)
        {
            this.PreviewMode = true;
            this.InitializeComponent();

            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            _ = SetParent(helper.Handle, PreviewWndHandle);
            _ = SetWindowLong(helper.Handle, -16, new IntPtr(GetWindowLong(helper.Handle, -16) | 0x40000000));
            _ = GetClientRect(PreviewWndHandle, out var ParentRect);

            this.Width = ParentRect.Width;
            this.Height = ParentRect.Height;
            this.Left = 0;
            this.Height = 0;

            this.Timer = new Timer(100);
            this.Timer.Elapsed += (_, e) =>
            {
                if (e.SignalTime.Hour != this.Time.Hour || e.SignalTime.Minute != this.Time.Minute)
                {
                    this.SetTime(e.SignalTime);
                    this.Time = e.SignalTime;
                }
            };
            this.Timer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var CurrentLocation = e.GetPosition(this);
            if (!this.PreviewMode)
            {
                if (!(this.MouseLocation is null))
                {
                    var x2 = Math.Pow(this.MouseLocation.Value.X - CurrentLocation.X, 2);
                    var y2 = Math.Pow(this.MouseLocation.Value.Y - CurrentLocation.Y, 2);
                    if (Math.Sqrt(x2 + y2) > 3)
                    {
                        Application.Current.Shutdown();
                    }
                }

                this.MouseLocation = CurrentLocation;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!this.PreviewMode) Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.PreviewMode) Application.Current.Shutdown();
        }

        public void Dispose()
        {
            this.Timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
