﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

[assembly: InternalsVisibleToAttribute("Radio.UnitTests")]
namespace Radio
{
    class RadioModel
    {
        protected Boolean IsConnected { private set; get; }
        protected Uri CurrentResourceUri { get; set; }

        public delegate void StateChangedHandler(object sender, EventArgs e);
        public event StateChangedHandler Connected;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public RadioModel()
        {
            
        }

        public void ConnectToURL(Uri url)
        {
            Logger.Info("connecting to URL: {0}", url);

            Uri resUrl = SoundCloudResourceFinder.FindAudioResBySCLink(url);
            this.CurrentResourceUri = resUrl;
            this.InvokeConnectedEvent(resUrl);
        }

        void InvokeConnectedEvent(Uri resourceUrl)
        {
            //stub
            if (Connected != null)
                Connected.Invoke(this, null);
        }
    }
}
