using Microsoft.Scripting;
using NAudio.Wave;
using NLog.Targets.Wrappers;
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
    class Mp3WaveProvider : IWaveProvider, IDisposable
    {
        const int BYTE_NEEDED_FOR_INIT = 16384; // have to cover the first two frame headers. 
        const int INITIAL_BUFFER_NUM = 4;

        private HttpWebResponse response;
        private Stream sourceStream;
        //private Stream bufferedStream;
        //private int streamReadPosition;

        private BuffersManager buffersManager;
        private int beingReadBufferUnreadIndexBookmark;
        private byte[] beingReadBuffer;
        private Queue<byte[]> filledBuffers;
#if DEBUG
        //private Stream debugFileStream = File.Create("C:\\Users\\%USERPROFILE%\\Desktop\\radioDebug.mp3"); // TODO: Change to a proper directory. for writting down the downloaded stream for debugging. 
#endif
        public Uri Url { get; }
        public WaveFormat WaveFormat {private set; get;}

        //public int SpeedCalcUnitSize { get; }
        //private int NumOfUnitPerBuffer { get; }
        public int DefaultBufferSize {get;}

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public Mp3WaveProvider(Uri mp3Url, int bufferSize)
        {
            HttpWebRequest req;
            HttpWebResponse tempRes = null;

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

            DefaultBufferSize = bufferSize;
            this.InitializeBuffers();

            this.StartBuffering();

            beingReadBufferUnreadIndexBookmark = 0;
    }

        private void InitializeStream()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url.ToString());
            response = (HttpWebResponse)req.GetResponse();
            sourceStream = response.GetResponseStream();
        }

        private void InitializeBuffers()
        {
            buffersManager = new BuffersManager(DefaultBufferSize, INITIAL_BUFFER_NUM);
            filledBuffers = new Queue<byte[]>();
        }

        private void StartBuffering()
        {
            // start with two buffers to guarantee at least one filled buffer in reserve
            this.FillABufferFromSourceStream();
            //this.FillABufferFromSourceStream();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (filledBuffers.Count < 1)
            {
                // TODO: stub. wait for download
                bool result = this.FillABufferFromSourceStream();
                if (! result)
                {
                    return 0;
                }
            }
            
            Debug.Assert(filledBuffers.Count > 0);
            int writtenByteCount = 0;
            writeDataFromFilledBuffers(ref writtenByteCount);

            if (filledBuffers.Count < 1)
            {
                this.FillABufferFromSourceStream();
                //this.FillABufferFromSourceStream();
            }
            else if (filledBuffers.Count < 2)
            {
                this.FillABufferFromSourceStream();
            }

            Logger.Debug("Read returns with result: {0}", writtenByteCount);
            return writtenByteCount;

            void writeDataFromFilledBuffers(ref int wbc)
            {
                if (filledBuffers.Count > 0)
                {
                    if (beingReadBufferUnreadIndexBookmark == 0)
                    {
                        // this is a new buffer
                        beingReadBuffer = filledBuffers.Dequeue();
                    }

                    Debug.Assert(beingReadBuffer != null);
                    Debug.Assert(beingReadBuffer.Length > beingReadBufferUnreadIndexBookmark);

                    int unreadBytesInBuffer = beingReadBuffer.Length - beingReadBufferUnreadIndexBookmark;
                    if (count >= unreadBytesInBuffer)
                    {
                        // this whole buffer will fit; write the rest of the buffer from the bookmark and clear bookmark
                        int bytesToWrite = unreadBytesInBuffer;

                        Logger.Debug("[if]   beingReadBuffer size: {0}, bookmark: {1}, buffer size: {2}, offset: {3}, writtenByteCount so far: {4}, bytesToWrite: {5}",
                            beingReadBuffer.Length, beingReadBufferUnreadIndexBookmark, buffer.Length, offset, wbc, bytesToWrite);

                        Array.Copy(beingReadBuffer, beingReadBufferUnreadIndexBookmark, buffer, offset + wbc, bytesToWrite);
                        wbc += bytesToWrite;
                        beingReadBufferUnreadIndexBookmark = 0;

                        if (buffersManager.BelongToTheManager(beingReadBuffer))
                        {
                            buffersManager.RecycleUsedBuffer(beingReadBuffer);
                        }

                        writeDataFromFilledBuffers(ref wbc); // continue the recursion
                    }
                    else
                    {
                        // this whole buffer is more than needed; write as much data as possible, bookmark the index
                        int bytesToWrite = count - wbc;

                        Logger.Debug("[else] beingReadBuffer size: {0}, bookmark: {1}, buffer size: {2}, offset: {3}, writtenByteCount: {4}, bytesToWrite: {5}",
                            beingReadBuffer.Length, beingReadBufferUnreadIndexBookmark, buffer.Length, offset, wbc, bytesToWrite);

                        Array.Copy(beingReadBuffer, beingReadBufferUnreadIndexBookmark, buffer, offset + wbc, bytesToWrite);
                        wbc += bytesToWrite;
                        beingReadBufferUnreadIndexBookmark = bytesToWrite;
                        // all bytes written; end recursion
                    }
                } // else, we don't have a filled buffer left (not enough data); end recursion
            }
        }

        /// <returns>false if the end of the stream has been reached and no data was read, ortherwise true</returns>
        private bool FillABufferFromSourceStream()
        {
            byte[] buffer = buffersManager.CheckoutNewBuffer();
            int unreadBytes = StreamReader.ReadBytesFromStream(sourceStream, buffer, 0, buffer.Length);
            Debug.Assert(unreadBytes <= buffer.Length);

            if (unreadBytes == 0)
            {
                // buffer is full
                filledBuffers.Enqueue(buffer);
                return true;
            }
            else if (unreadBytes < buffer.Length)
            {
                // buffer is not completely filled. remove invalid data
                byte[] croppedBf = new byte[buffer.Length - unreadBytes];
                Array.Copy(buffer, croppedBf, croppedBf.Length);
                buffersManager.RecycleUsedBuffer(buffer);
                filledBuffers.Enqueue(croppedBf);
                return true;
            }
            else
            {
                // nothing is read
                return false;
            }
        }

        public void Dispose()
        {
            if (response != null)
            {
                response.Close();
            }
#if DEBUG
            //if (debugFileStream != null)
            //{
            //    debugFileStream.Close();
            //}
#endif
        }

        ~Mp3WaveProvider()
        {
            this.Dispose();
        }
    }
}
