﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using NLog.Fluent;
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
        private Boolean isTransmittingData;

        public String Url { protected set; get; }

        // required for INotifyPropertyChanged
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

        // Playing can be true only if Connected is true
        protected Boolean IsTransmittingData { private set; get; }

        // ICommand implementations
        public ICommand PlayCommand { protected set; get; }

        // create and set up logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //constructor
        public RadioViewModel()
        {
            // alloc and init model
            radioModel = new RadioModel();
            radioModel.Connected += ConnectedEventHandler;

            // set up commands
            this.PlayCommand = new DelegateCommand(ExecutePlayCommand);

            // TODO stub
            this.Url = "test url";
        }

        private void ExecutePlayCommand(object obj)
        {
            // TODO
            Logger.Info("Executing Play Command");

            if (this.Url != null) {
                radioModel.ConnectToURL(this.Url);
            }
            else
            {
                // TODO 
                Logger.Info("URL invalid");
            }       
        }

        private void ConnectedEventHandler(object sender, EventArgs e)
        {
            Logger.Info("ViewModel: connected!");
            IsConnected = true;
        }

        // If URL is valid, return sanitized URL; else, return null
        private String ValidateAndSanitizeURL(String url)
        {
            // TODO stub
            return url; 
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