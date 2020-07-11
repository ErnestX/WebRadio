using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    // TODO singleton
    class RadioModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected Boolean Connected { private set; get; } 
        
        // Playing can be true only if Connected is true
        protected Boolean TransmittingData { private set; get; }

        // create and set up logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // constructor
        public RadioModel()
        {
            //TODO
        }

        public void ConnectToURL(String url)
        {
            //TODO
            Logger.Info("connecting to URL: {0}", url);
        }

        public void StartTransmittingData()
        {
            //TODO
        }
    }
}
