using NAudio.Wave;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Radio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RadioViewModel radioViewModel;

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
            Console.WriteLine("start NAudio test");
            Uri testUrl = new Uri("https://file-examples-com.github.io/uploads/2017/11/file_example_MP3_700KB.mp3");
            StreamAudioFromUrl(testUrl);
        }

        //void StreamAudioFromUrl(Uri streamUrl)
        //{
        //    var url = streamUrl.ToString();
        //    var mf = new MediaFoundationReader(url);
        //    var wo = new WaveOutEvent();
        //    wo.Init(mf);
        //    wo.Play();
        //    Console.WriteLine("NAudio playing");
        //}

        void StreamAudioFromUrl(Uri streamUrl)
        {
            HttpWebRequest req;
            HttpWebResponse res = null;

            try
            {
                req = (HttpWebRequest)WebRequest.Create(streamUrl.ToString());
                res = (HttpWebResponse)req.GetResponse();
                Stream stream = res.GetResponseStream();

                byte[] buffer = new byte[4096];
                int numOfbytesReadIntoBuffer;
                bool isFirstIteration = true;
                while ((numOfbytesReadIntoBuffer = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (isFirstIteration)
                    {
                        Mp3FileReader mp3FileReader = new Mp3FileReader(new MemoryStream(buffer));
                        Console.WriteLine("Printing WaveFormat Object: ");
                        Console.WriteLine(mp3FileReader.WaveFormat.ToString());
                        
                        isFirstIteration = false; 
                    }

                    for (int i = 0; i < numOfbytesReadIntoBuffer; i++)
                    {
                        Console.WriteLine(buffer[i].ToString());
                    }
                }
            }
            finally
            {
                if (res != null)
                    res.Close();
            }
            //Console.In.Read();
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
