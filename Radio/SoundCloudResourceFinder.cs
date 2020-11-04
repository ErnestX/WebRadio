using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Radio
{
    public static class SoundCloudResourceFinder
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");
        public static Uri FindAudioResBySCLink(Uri url)
        {
            //TODO: stub
            //Uri testUrl = new Uri("https://file-examples-com.github.io/uploads/2017/11/file_example_WAV_1MG.wav");
            Uri testUrl = new Uri("https://www.ee.columbia.edu/~dpwe/sounds/music/temple_of_love-sisters_of_mercy.wav");
            //Uri testUrl = new Uri("https://file-examples-com.github.io/uploads/2017/11/file_example_MP3_700KB.mp3");
            //Uri testUrl = new Uri("http://www.hochmuth.com/mp3/Tchaikovsky_Nocturne__orch.mp3");
            return testUrl;
        }
    }
}
