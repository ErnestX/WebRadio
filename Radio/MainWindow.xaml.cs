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
        private MyBufferedWaveProvider waveProvider;

        // create and set up logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public MainWindow()
        {
            InitializeComponent();

            playButton.Content = "play";

            this.radioViewModel = new RadioViewModel();
            radioViewModel.PropertyChanged += OnStateChanged;
            this.DataContext = radioViewModel;
        }

        void OnStateChanged(object sender, PropertyChangedEventArgs args)
        {
            Logger.Info("OnStateChanged sent by: {0}", args.PropertyName);

            if (args.PropertyName.Equals("IsPlaying"))
            {
                playButton.Content = "playing!";
            }

        }
    }
}
