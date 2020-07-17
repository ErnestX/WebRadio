using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    // TODO singleton
    class RadioModel
    {
        protected Boolean IsConnected { private set; get; } 
        
        // Playing can be true only if Connected is true
        protected Boolean IsTransmittingData { private set; get; }

        public delegate void StateChangedHandler(object sender, EventArgs e);
        public event StateChangedHandler Connected;
        public event StateChangedHandler TransmissionStateChanged;

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

            //stub
            if (Connected != null)
                Connected.Invoke(this, null); 
        }

        public void StartTransmittingData()
        {
            //TODO
        }
    }
}
