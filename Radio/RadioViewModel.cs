using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using NLog.Fluent;
using System.Windows.Input;

namespace Radio
{
    /* implement INotifyPropertyChanged in order to notify the view model 
    whether the play command can be executed */
    class RadioViewModel : INotifyPropertyChanged 

    {
        // Model
        private RadioModel radioModel;

        public String Url { protected set; get; }

        // ICommand implementations
        public ICommand PlayCommand { protected set; get; }

        // required for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // create and set up logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //constructor
        public RadioViewModel()
        {
            // alloc and init model
            radioModel = new RadioModel();
            radioModel.ConnectionStateChanged += ConnectionStatusChangedHandler; 
            
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

        private void ConnectionStatusChangedHandler(Boolean newStatus)
        {
            Logger.Info("ViewModel: connection state changed to {0}", newStatus); 

        }

        // If URL is valid, return sanitized URL; else, return null
        private String ValidateAndSanitizeURL(String url)
        {
            // TODO stub
            return url; 
        }
    }
}
