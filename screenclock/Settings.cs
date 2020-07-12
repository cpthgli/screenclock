using Microsoft.Win32;

using System;
using System.Globalization;
using System.Linq;

namespace screenclock
{
    class Settings
    {
        public const string RegistryKey = "SOFTWARE\\ScreenClock";
        public string Path { get; set; }
        public double Blur { get; set; }

        public Settings()
        {
            this.Path = string.Empty;
            this.Blur = 0;
        }

        public void LoadSettings()
        {
            var key = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (key is null)
            {
                this.Path = string.Empty;
                this.Blur = 0;
                return;
            }

            var names = key.GetValueNames();
            this.Path = names.Contains("path") ? (string)key.GetValue("path") : string.Empty;
            this.Blur = names.Contains("blur") ? double.Parse((string)key.GetValue("blur"), CultureInfo.InvariantCulture) : 0;
        }

        public void SaveSettings()
        {
            var key = Registry.CurrentUser.CreateSubKey(RegistryKey);
            key.SetValue("path", this.Path);
            key.SetValue("blur", this.Blur);
        }
    }
}
