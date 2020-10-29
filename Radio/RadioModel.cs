using NAudio.Wave;
using System;
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
    class RadioModel : IDisposable
    {
        public Uri CurrentResourceUri { get; private set; }
        public MyBufferedWaveProvider waveProvider { get; private set; }

        public delegate void StateChangedHandler(object sender, ConnectedEventArgs e);
        public event StateChangedHandler Connected;
        private HttpWebResponse response;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public RadioModel()
        {
            
        }

        public void ConnectToURL(Uri url)
        {
            Logger.Info("connecting to URL: {0}", url);

            Uri resUrl = SoundCloudResourceFinder.FindAudioResBySCLink(url);
            this.CurrentResourceUri = resUrl;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resUrl.ToString());
            response = (HttpWebResponse)req.GetResponse();

            this.waveProvider = new MyBufferedWaveProvider(response.GetResponseStream(), 1024 * 1024 * 2); 

            this.InvokeConnectedEvent();
        }

        void InvokeConnectedEvent()
        {
            if (Connected != null)
            {
                Connected.Invoke(this, new ConnectedEventArgs(this.waveProvider));
            }
        }

        public void Dispose()
        {
            if (response != null)
            {
                response.Close();
            }

            this.waveProvider.Dispose();
        }

        ~RadioModel()
        {
            this.Dispose();
        }
    }
}
