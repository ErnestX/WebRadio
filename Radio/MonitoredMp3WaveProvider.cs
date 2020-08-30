using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    class MonitoredMp3WaveProvider : IWaveProvider
    {
        // have to cover the first two frame headers. 
        const int BYTE_READ_FOR_INIT = 16384;
        const int INITIAL_BUFFER_NUM = 4;
        public Uri Url { get; }
        public WaveFormat WaveFormat {private set; get;}

        public int SpeedCalcUnitSize { get; }
        private int UnitNumPerBuffer { get; }
        public int BufferSize
        {
            get
            {
                return this.SpeedCalcUnitSize * this.UnitNumPerBuffer;
            }
        }

        public MonitoredMp3WaveProvider(Uri mp3Url)
        {
            // init WaveFormat from a fragment of the mp3 file
            HttpWebRequest request;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(mp3Url.ToString());
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                byte[] bfr = new byte[BYTE_READ_FOR_INIT];
                int bytesRead = stream.Read(bfr, 0, bfr.Length);
                Mp3FileReader mp3FileReader = new Mp3FileReader(new MemoryStream(bfr));
                this.WaveFormat = mp3FileReader.WaveFormat;
                this.Url = mp3Url;

                Console.WriteLine("Printing WaveFormat Object: ");
                Console.WriteLine(this.WaveFormat.ToString());
            }
            catch (Exception ex)
            {
                // TODO: display message in UI
                Console.WriteLine("failed to read url: ");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            //HttpWebRequest req;
            //HttpWebResponse res = null;

            //try
            //{
            //    req = (HttpWebRequest)WebRequest.Create(this.Url.ToString());
            //    res = (HttpWebResponse)req.GetResponse();
            //    Stream stream = res.GetResponseStream();

            //    byte[] buffer = new byte[4096];
            //    int numOfbytesReadIntoBuffer;
            //    bool isFirstIteration = true;
            //    while ((numOfbytesReadIntoBuffer = stream.Read(buffer, 0, buffer.Length)) > 0)
            //    {
            //        if (isFirstIteration)
            //        {
            //            Mp3FileReader mp3FileReader = new Mp3FileReader(new MemoryStream(buffer));
            //            Console.WriteLine("Printing WaveFormat Object: ");
            //            Console.WriteLine(mp3FileReader.WaveFormat.ToString());

            //            isFirstIteration = false;
            //        }

            //        for (int i = 0; i < numOfbytesReadIntoBuffer; i++)
            //        {
            //            Console.WriteLine(buffer[i].ToString());
            //        }
            //    }
            //}
            //finally
            //{
            //    if (res != null)
            //        res.Close();
            //}
            return 0;
        }
    }
}
