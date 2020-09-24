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

        private HttpWebResponse response;
        private Stream sourceStream;
        private bool requestNextBuffer;
        private int beingReadBufferUnreadIndexBookmark;
        private byte[] beingReadBuffer;
        private Bufferer bufferer;
#if DEBUG
        private bool LOG_STREAM_TO_FILE = false; // switch off to avoid conflict when running tests in parallel
        private Stream debugFileStream;
#endif
        public Uri Url { get; }
        public WaveFormat WaveFormat {private set; get;}

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Mp3WaveProviderDebug");

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

            bufferer = new Bufferer(sourceStream, bufferSize);
            requestNextBuffer = true;
            beingReadBufferUnreadIndexBookmark = 0;

#if DEBUG
            if (LOG_STREAM_TO_FILE)
            {
                debugFileStream = File.Create(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + @"\radioDebug.mp3"); 
            }
#endif
        }

        private void InitializeStream()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url.ToString());
            response = (HttpWebResponse)req.GetResponse();
            sourceStream = response.GetResponseStream();
        }



        public int Read(byte[] buffer, int offset, int count)
        {
            int writtenByteCount = 0;
            writeDataFromFilledBuffers(ref writtenByteCount);

            Logger.Debug(".......Read returns with result: {0}", writtenByteCount);
            return writtenByteCount;

            void writeDataFromFilledBuffers(ref int wbc)
            {
                if (requestNextBuffer)
                {
                    //if (filledBuffers.Count <= 0)
                    //if (bufferer.EndOfStream)
                    //{
                    //    return;
                    //}
                    //beingReadBuffer = filledBuffers.Dequeue();
                    beingReadBuffer = bufferer.GetNextBuffer();
                    if (beingReadBuffer == null)
                    {
                        return;
                    }
                }

                Debug.Assert(beingReadBuffer != null);
                Debug.Assert(beingReadBuffer.Length > beingReadBufferUnreadIndexBookmark);

                int unreadBytesInBuffer = beingReadBuffer.Length - beingReadBufferUnreadIndexBookmark;
                if ((count - wbc) >= unreadBytesInBuffer)
                {
                    // this whole buffer will fit; write the rest of the buffer from the bookmark and clear bookmark
                    requestNextBuffer = true;
                    int bytesToWrite = unreadBytesInBuffer;

                    Logger.Debug("[if]   beingReadBuffer size: {0}, bookmark: {1}, buffer size: {2}, offset: {3}, writtenByteCount: {4}, bytesToWrite: {5}",
                        beingReadBuffer.Length, beingReadBufferUnreadIndexBookmark, buffer.Length, offset, wbc, bytesToWrite);

                    Array.Copy(beingReadBuffer, beingReadBufferUnreadIndexBookmark, buffer, offset + wbc, bytesToWrite);
#if DEBUG
                    if (LOG_STREAM_TO_FILE)
                    {
                        debugFileStream.Write(beingReadBuffer, beingReadBufferUnreadIndexBookmark, bytesToWrite);
                    }
#endif
                    wbc += bytesToWrite;
                    beingReadBufferUnreadIndexBookmark = 0;

                    bufferer.TryRecycleBuffer(beingReadBuffer);

                    writeDataFromFilledBuffers(ref wbc); // continue the recursion
                }
                else
                {
                    // this whole buffer is more than needed; write as much data as possible, bookmark the index
                    requestNextBuffer = false;
                    int bytesToWrite = count - wbc;

                    Logger.Debug("[else] beingReadBuffer size: {0}, bookmark: {1}, buffer size: {2}, offset: {3}, writtenByteCount: {4}, bytesToWrite: {5}",
                        beingReadBuffer.Length, beingReadBufferUnreadIndexBookmark, buffer.Length, offset, wbc, bytesToWrite);

                    Array.Copy(beingReadBuffer, beingReadBufferUnreadIndexBookmark, buffer, offset + wbc, bytesToWrite);
#if DEBUG
                    if (LOG_STREAM_TO_FILE)
                    {
                        debugFileStream.Write(beingReadBuffer, beingReadBufferUnreadIndexBookmark, bytesToWrite);
                    }
#endif
                    wbc += bytesToWrite;
                    beingReadBufferUnreadIndexBookmark += bytesToWrite;
                    // all bytes written; end recursion
                }
            }
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

        ~Mp3WaveProvider()
        {
            this.Dispose();
        }
    }
}
