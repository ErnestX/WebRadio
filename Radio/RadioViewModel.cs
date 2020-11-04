using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using NAudio.Wave;

namespace Radio
{
    /* implement INotifyPropertyChanged in order to notify the view model 
    whether the play command can be executed */ 
    class RadioViewModel : INotifyPropertyChanged 

    {
        private RadioModel radioModel;
        private Boolean isPlaying;
        private double downloadSpeedKBPerSec;
        private IWaveProvider waveProvider;

        public Uri Url { protected set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Boolean IsPlaying 
        {
            private set {SetProperty(ref isPlaying, value);}
            get {return this.isPlaying;}
        }

        public double DownloadSpeedKBPerSec
        {
            private set { SetProperty(ref downloadSpeedKBPerSec, value); }
            get { return this.downloadSpeedKBPerSec; }
        }

        public ICommand PlayCommand { protected set; get; }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public RadioViewModel()
        {
            radioModel = new RadioModel();
            radioModel.OnConnected += ConnectedEventHandler;
            radioModel.OnMonitorUpdate += MonitorUpdateHandler;

            this.PlayCommand = new DelegateCommand(ExecutePlayCommand);

            // TODO: stub. get this from view
            this.Url = new Uri("https://file-examples-com.github.io/uploads/2017/11/file_example_MP3_700KB.mp3");
        }

        private void ExecutePlayCommand(object obj)
        {
            // TODO
            Logger.Info("Executing Play Command");

            if (this.Url != null) 
            {
                if (!this.IsPlaying)
                {
                    radioModel.ConnectToURL(this.Url);
                }
            }
            else
            {
                // TODO 
                Logger.Info("URL invalid");
            }
        }

        private void MonitorUpdateHandler(object sender, OnUpdateEventArgs args)
        {
            this.DownloadSpeedKBPerSec = (double)(args.BytesDownloaded) / (double)args.DownloadTimeMilSec;
        }

        private void ConnectedEventHandler(object sender, ConnectedEventArgs e)
        {
            waveProvider = e.mbwp;
            WaveOutEvent wo = new WaveOutEvent();
            //wo.DesiredLatency = 200;
            //wo.NumberOfBuffers = 2;
            wo.Init(waveProvider);
            wo.Play(); // TODO: Call dispose()
            Console.WriteLine("audio playing");
            Logger.Info("ViewModel: connected!");
            IsPlaying = true;
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
