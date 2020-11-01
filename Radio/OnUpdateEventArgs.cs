using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    class OnUpdateEventArgs
    {
        public int BytesDownloaded {get;}
        public int DownloadTimeMilSec { get; }

        public OnUpdateEventArgs(int bytesDownloaded, int downloadTimeMilSec)
        {
            this.BytesDownloaded = bytesDownloaded;
            this.DownloadTimeMilSec = downloadTimeMilSec;
        }
    }
}
