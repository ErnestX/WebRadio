using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace Radio
{
    /* implement INotifyPropertyChanged in order to notify the view model 
    whether the play command can be executed */ 
    class RadioViewModel : INotifyPropertyChanged 

    {
        private RadioModel radioModel;
        private Boolean isConnected;

        public Uri Url { protected set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Boolean IsConnected {
            private set
            {
                SetProperty(ref isConnected, value);
            }
            get
            {
                return this.isConnected;
            }
        }
        public ICommand PlayCommand { protected set; get; }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public RadioViewModel()
        {
            radioModel = new RadioModel();
            radioModel.Connected += ConnectedEventHandler;

            this.PlayCommand = new DelegateCommand(ExecutePlayCommand);

            // TODO stub
            this.Url = new Uri("https://www.youtube.com/watch?v=9ORO3cnPu7k");
        }

        private void ExecutePlayCommand(object obj)
        {
            // TODO
            Logger.Info("Executing Play Command");

            if (this.Url != null) 
            {
                radioModel.ConnectToURL(this.Url);
            }
            else
            {
                // TODO 
                Logger.Info("URL invalid");
            }
        }

        private void ConnectedEventHandler(object sender, ConnectedEventArgs e)
        {
            Logger.Info("ViewModel: connected!");
            IsConnected = true;
        }

        // If URL is valid, return sanitized URL; else, return null
        private String ValidateAndSanitizeURL(String url)
        {
            if (url == null) 
            {
                return null;
            } 
            else
            {
                // TODO: stub
                return url;
            }
        }

        protected bool SetProperty<T>(ref T storage, T value,
                              [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
