using Microsoft.Scripting;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    class MonitoredMp3WaveProvider : IWaveProvider, IDisposable
    {
        const int BYTE_NEEDED_FOR_INIT = 16384; // have to cover the first two frame headers. 
        const int INITIAL_BUFFER_NUM = 4;

        private HttpWebResponse response;
        private Stream sourceStream;
        private Stream bufferedStream;
        private int streamBufferedPosition;

        private BuffersManager buffersManager;
        private byte[] readBuffer;
        private Queue<byte[]> filledBuffers;
        private byte[] downloadingBuffer;
#if DEBUG
        private Stream debugFileStream = File.Create("C:\\Users\\%USERPROFILE%\\Desktop\\radioDebug.mp3"); // for writting down the downloaded stream for debugging. 
#endif
        public Uri Url { get; }
        public WaveFormat WaveFormat {private set; get;}

        public int SpeedCalcUnitSize { get; }
        private int NumOfUnitPerBuffer { get; }
        public int BufferSize
        {
            get
            {
                return this.SpeedCalcUnitSize * this.NumOfUnitPerBuffer;
            }
        }


        public MonitoredMp3WaveProvider(Uri mp3Url, int speedCalcUnitSize, int numOfUnitPerBuffer)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(mp3Url.ToString());
            HttpWebResponse tempRes = (HttpWebResponse)req.GetResponse();

            try // init WaveFormat from a fragment of the mp3 file
            {
                req = (HttpWebRequest)WebRequest.Create(mp3Url.ToString());
                tempRes = (HttpWebResponse)req.GetResponse();
                Stream tempStream = tempRes.GetResponseStream();

                byte[] bfr = new byte[BYTE_NEEDED_FOR_INIT];
                int bytesRead = tempStream.Read(bfr, 0, bfr.Length);
                Mp3FileReader mp3FileReader = new Mp3FileReader(new MemoryStream(bfr));
                WaveFormat = mp3FileReader.WaveFormat;
                Url = mp3Url;

                Console.WriteLine("Printing WaveFormat Object: ");
                Console.WriteLine(WaveFormat.ToString());
                //Console.WriteLine("bite rate: {0}", WaveFormat.AverageBytesPerSecond);
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

            SpeedCalcUnitSize = speedCalcUnitSize;
            NumOfUnitPerBuffer = numOfUnitPerBuffer;
            this.InitializeBuffers();

            streamBufferedPosition = 0;
        }

        private void InitializeStream()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url.ToString());
            response = (HttpWebResponse)req.GetResponse(); 
            sourceStream = response.GetResponseStream();
        }

        private void InitializeBuffers()
        {
            buffersManager = new BuffersManager(BufferSize, INITIAL_BUFFER_NUM);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int numOfBytesReadIntoBuffer;
            numOfBytesReadIntoBuffer = sourceStream.Read(buffer, offset, count);
#if DEBUG
            debugFileStream.Write(buffer, offset, numOfBytesReadIntoBuffer); // write down the content read for debugging
#endif
            Console.WriteLine("bytes read from stream: {0}", numOfBytesReadIntoBuffer);

            return numOfBytesReadIntoBuffer;
        }

        /// <returns>false if the end of the stream has been reached, ortherwise true</returns>
        private bool fillBufferFromSourceStream()
        {

        }

        /// <returns>number of bytes failed to read because the end of stream is reached; -1 if all bytes are read successfully</returns>
        public int readBytesFromStream(Stream stream, byte[] buffer, int offset, int bytesToRead)
        {
         // TODO: validify input parameters   
            int bytesRead;
            int totalBytesRead = 0;

            int timesReadZeroByte = 0;
            const int TIMES_READ_ZERO_BYTE_BEFORE_ABORT = 3;

            int bytesYetToRead = bytesToRead;
            while (bytesYetToRead > 0)
            {
                bytesRead = stream.Read(buffer, offset, bytesYetToRead);
                totalBytesRead += bytesRead;

                if (bytesRead <= 0)
                {
                    timesReadZeroByte++;
                }

                if (timesReadZeroByte >= TIMES_READ_ZERO_BYTE_BEFORE_ABORT)
                {
                    return bytesYetToRead;
                }

                offset += bytesRead;
                bytesYetToRead = bytesToRead - totalBytesRead;
                Debug.Assert(bytesYetToRead >= 0);
            } 

            return -1;
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
