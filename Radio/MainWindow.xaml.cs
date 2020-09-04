﻿using NAudio.Wave;
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
        private IWaveProvider waveProvider;

        // create and set up logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            playButton.Content = "play";
            this.radioViewModel = new RadioViewModel();
            radioViewModel.PropertyChanged += OnStateChanged; 
            this.DataContext = radioViewModel;

            // test NAudio
            Console.WriteLine("start audio test");
            Uri testUrl = new Uri("https://file-examples-com.github.io/uploads/2017/11/file_example_MP3_700KB.mp3");
            StreamAudioFromUrl(testUrl);
        }

        void StreamAudioFromUrl(Uri streamUrl)
        {
            waveProvider = new MonitoredMp3WaveProvider(streamUrl);
            WaveOut wo = new WaveOut();
            wo.DesiredLatency = 700;
            wo.NumberOfBuffers = 3;
            wo.Init(waveProvider);
            wo.Play(); // TODO: Call dispose()
            Console.WriteLine("audio playing");
        }

        void OnStateChanged(object sender, PropertyChangedEventArgs args)
        {
            Logger.Info("OnStateChanged sent by: {0}", args.PropertyName);

            if (args.PropertyName.Equals("IsConnected"))
            {
                if (this.radioViewModel.IsConnected)
                {
                    playButton.Content = "connected!";
                }
            }

        }
    }
}
