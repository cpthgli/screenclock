using Microsoft.Win32;

using ModernWpf.Controls;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
    /// SettingsWindows.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            this.InitializeComponent();
            this.LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = new Settings();
            settings.LoadSettings();
            this.Path.Text = settings.Path;
            this.Blur.Value = settings.Blur;
        }

        private void SaveSettings()
        {
            var settings = new Settings()
            {
                Path = this.Path.Text,
                Blur = this.Blur.Value
            };
            settings.SaveSettings();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
