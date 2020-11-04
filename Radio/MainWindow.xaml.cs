using NAudio.Wave;
using NLog;
using System;
using System.ComponentModel;
using System.Windows;


namespace Radio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RadioViewModel radioViewModel;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public MainWindow()
        {
            InitializeComponent();

            playButton.Content = "play";
            downloadSpeed.Text = "0KB/s";

            this.radioViewModel = new RadioViewModel();
            radioViewModel.PropertyChanged += OnStateChanged;
            this.DataContext = radioViewModel;
        }

        void OnStateChanged(object sender, PropertyChangedEventArgs args)
        {
            Logger.Info("OnStateChanged sent by: {0}", args.PropertyName);

            switch (args.PropertyName)
            {
                case "IsPlaying":
                    if (radioViewModel.IsPlaying)
                    {
                        playButton.Content = "playing!";
                    }
                    break;
                case "downloadSpeedKBPerSec":
                    downloadSpeed.Text = String.Format("{0}KB/s", radioViewModel.DownloadSpeedKBPerSec);
                    break;
            }

        }
    }
}
