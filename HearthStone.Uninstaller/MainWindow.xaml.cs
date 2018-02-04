#region

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

#endregion


namespace HearthStone.UnInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private static string LogConfigPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Blizzard\Hearthstone\log.config";

        private static string HDTDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HearthstoneDeckTracker");

        private static bool LoggingEnabled => File.Exists(LogConfigPath);

        private static bool DataExists => Directory.Exists(HDTDirectory);

        private static bool AutostartEnabled
        {
            get
            {
                var regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                return regKey?.GetValue("Hearthstone Deck Tracker") != null;
            }
        }

        public string TextLogging => LoggingEnabled ? "REMOVE" : "REMOVED";

        public string TextData => DataExists ? "REMOVE" : "REMOVED";

        public string TextAutostart => AutostartEnabled ? "REMOVE" : "REMOVED";

        public SolidColorBrush BackgroundLogging => new SolidColorBrush(LoggingEnabled ? Colors.Red : Colors.Green);

        public SolidColorBrush BackgroundData => new SolidColorBrush(DataExists ? Colors.Red : Colors.Green);

        public SolidColorBrush BackgroundAutostart => new SolidColorBrush(AutostartEnabled ? Colors.Red : Colors.Green);


        private void ButtonAutostart_Click(object sender, RoutedEventArgs e)
        {
            if (!AutostartEnabled)
                return;
            try
            {
                var regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                regKey?.DeleteValue("Hearthstone Deck Tracker", false);
                OnPropertyChanged("TextAutostart");
                OnPropertyChanged("BackgroundAutostart");
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void ButtonLogging_Click(object sender, RoutedEventArgs e)
        {
            if (!LoggingEnabled)
                return;
            try
            {
                File.Delete(LogConfigPath);
                OnPropertyChanged("TextLogging");
                OnPropertyChanged("BackgroundLogging");
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void ButtonData_Click(object sender, RoutedEventArgs e)
        {
            if (!DataExists)
                return;
            try
            {
                var result = MessageBox.Show("Are you sure? This can not be undone!", "Delete HDT Data", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    Directory.Delete(HDTDirectory, true);
                    OnPropertyChanged("TextData");
                    OnPropertyChanged("BackgroundData");
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}