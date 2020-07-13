using Cysharp.Text;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace screenclock_movie
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var args = e.Args;
            var wins = new List<Window>();

            if (args.Length > 0)
            {
                if (args[0].StartsWith("/S", StringComparison.InvariantCultureIgnoreCase))
                {
                    wins.AddRange(ScreenSaver());
                }
                else if (args[0].StartsWith("/C", StringComparison.InvariantCultureIgnoreCase))
                {
                    wins.Add(new SettingsWindow());
                }
                else if (args[0].StartsWith("/P", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (args.Length < 2)
                    {
                        var ex = new ArgumentException(ZString.Join(" ", args));
                        _ = MessageBox.Show(ex.Message, ex.ParamName, MessageBoxButton.OK, MessageBoxImage.Error);
                        throw ex;
                    }

                    //var previewWndHandle = new IntPtr(long.Parse(args[1], CultureInfo.InvariantCulture));
                    //wins.Add(new ScreenSaverWindow(previewWndHandle));
                }
                else
                {
                    var ex = new ArgumentException(ZString.Join(" ", args));
                    _ = MessageBox.Show(ex.Message, ex.ParamName, MessageBoxButton.OK, MessageBoxImage.Error);
                    throw ex;
                }
            }
            else
            {
                wins.Add(new SettingsWindow());
            }

            foreach (var win in wins)
            {
                win.Show();
            }
        }

        private static List<Window> ScreenSaver()
        {
            var (path, blur) = LoadSettings();

            var wins = new List<Window>();

            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                wins.Add(new ScreenSaverWindow(screen.Bounds, path, blur));
            }

            return wins;
        }

        private static (string, double) LoadSettings()
        {
            var settings = new Settings();
            settings.LoadSettings();

            if (!Directory.Exists(settings.Path))
            {
                return (string.Empty, settings.Blur);
            }

            var mp4s = Directory.GetFiles(settings.Path, "*.mp4");
            var mkvs = Directory.GetFiles(settings.Path, "*.mkv");
            var files = mp4s.Concat(mkvs).ToList();
            var rand = new Random();
            var path = files.Count == 0 ? string.Empty : files[rand.Next(0, files.Count - 1)];

            return (path, settings.Blur);
        }
    }
}
