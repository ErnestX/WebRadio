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
        private Mp3WaveProvider waveProvider;

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

        void StreamAudioFromUrl(Uri streamUrl)
        {
            waveProvider = new Mp3WaveProvider(streamUrl, 1000000); // use a huge buffer for testing to eliminate download interruptions as a factor
            Console.WriteLine("Mp3WaveProvider Initialized");
            var reader = new Mp3FileReader(@"C:\Users\Jialiang\Desktop\radioDebug.mp3");


            WaveOut wo = new WaveOut();
            //wo.DesiredLatency = 200;
            //wo.NumberOfBuffers = 2;
            wo.Init(waveProvider);
            //wo.Init(reader);
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

                    // test NAudio
                    Console.WriteLine("start audio test");
                    Uri testUrl = new Uri("https://file-examples-com.github.io/uploads/2017/11/file_example_MP3_700KB.mp3");
                    //Uri testUrl = new Uri("http://www.hochmuth.com/mp3/Tchaikovsky_Nocturne__orch.mp3");
                    StreamAudioFromUrl(testUrl);

                    //// testing: inspect the debug mp3 file
                    //waveProvider.Dispose();
                    //Mp3FileReader mp3FileReader = new Mp3FileReader("C:\\Users\\%USERPROFILE%\\Desktop\\radioDebug.mp3");
                    //Console.WriteLine("Printing WaveFormat Object of the downloaded file: ");
                    //Console.WriteLine(mp3FileReader.WaveFormat.ToString());
                }
            }

        }
    }
}
