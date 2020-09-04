using Microsoft.Scripting;
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
    class MonitoredMp3WaveProvider : IWaveProvider, IDisposable
    {
        const int BYTE_READ_FOR_INIT = 16384; // have to cover the first two frame headers. 
        const int INITIAL_BUFFER_NUM = 4;
        private HttpWebResponse response;
        private Stream stream;
#if DEBUG
        private Stream debugFileStream = File.Create("C:\\Users\\Jialiang\\Desktop\\radioDebug.mp3"); // for writting down the downloaded stream for debugging. 
#endif
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
            HttpWebRequest req;
            HttpWebResponse tempRes = null;

            // initalize stream here??????

            try // init WaveFormat from a fragment of the mp3 file
            {
                req = (HttpWebRequest)WebRequest.Create(mp3Url.ToString());
                tempRes = (HttpWebResponse)req.GetResponse();
                Stream tempStream = tempRes.GetResponseStream();

                byte[] bfr = new byte[BYTE_READ_FOR_INIT];
                int bytesRead = tempStream.Read(bfr, 0, bfr.Length);
                Mp3FileReader mp3FileReader = new Mp3FileReader(new MemoryStream(bfr));
                WaveFormat = mp3FileReader.WaveFormat;
                Url = mp3Url;

                Console.WriteLine("Printing WaveFormat Object: ");
                Console.WriteLine(WaveFormat.ToString());
            }
            catch (Exception ex)
            {
                // TODO: display message in UI
                Console.WriteLine("failed to read url: ");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (tempRes != null)
                {
                    tempRes.Close();
                }
            }

            this.InitializeStream();
        }

        private void InitializeStream()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url.ToString());
            response = (HttpWebResponse)req.GetResponse(); // after making this work, reuse the response from before instead of opening this new one
            stream = response.GetResponseStream();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int numOfBytesReadIntoBuffer;
            numOfBytesReadIntoBuffer = stream.Read(buffer, offset, count);
#if DEBUG
            debugFileStream.Write(buffer, offset, numOfBytesReadIntoBuffer); // write down the content read for debugging
#endif
            Console.WriteLine("bytes read from stream: {0}", numOfBytesReadIntoBuffer);

            return numOfBytesReadIntoBuffer;
        }

        public void Dispose()
        {
            if (response != null)
            {
                response.Close();
            }
#if DEBUG
            if (debugFileStream != null)
            {
                debugFileStream.Close();
            }
#endif
        }

        ~MonitoredMp3WaveProvider()
        {
            this.Dispose();
        }
    }
}
