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
    class MyWaveProvider : IWaveProvider, IDisposable
    {
        const int WAV_HEADER_SIZE = 43; 
        const int WAV_NUMCHANNELS_OFFSET = 22;
        const int WAV_NUMCHANNELS_SIZE = 2;
        const int WAV_SAMPLERATE_OFFSET = 24;
        const int WAV_SAMPLERATE_SIZE = 4;

        private HttpWebResponse response;
        private Stream sourceStream;
        private bool doneReadingBeingReadBuffer;
        private int beingReadBufferUnreadIndexBookmark;
        private byte[] beingReadBuffer;
        private Bufferer bufferer;
#if DEBUG
        private bool LOG_STREAM_TO_FILE = true; // switch off to avoid conflict when running tests in parallel
        private Stream debugFileStream;
#endif
        public Uri Url { get; }
        public WaveFormat WaveFormat {private set; get;}

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Mp3WaveProviderDebug");

        public MyWaveProvider(Uri url, int bufferSize)
        {
            // init stream
            this.Url = url;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url.ToString());
            response = (HttpWebResponse)req.GetResponse();
            sourceStream = response.GetResponseStream();

            bufferer = new Bufferer(sourceStream, bufferSize);

            // read header
            beingReadBuffer = bufferer.GetNextBuffer();
            while(beingReadBuffer.Length < WAV_HEADER_SIZE)
            {
                beingReadBuffer = ConCatByteArr(beingReadBuffer, bufferer.GetNextBuffer());
            }

            byte[] numOfChannelsBytes = new byte[WAV_NUMCHANNELS_SIZE];
            Array.Copy(beingReadBuffer, WAV_NUMCHANNELS_OFFSET, numOfChannelsBytes, 0, WAV_NUMCHANNELS_SIZE);
            int numOfChannels = BitConverter.ToInt16(numOfChannelsBytes, 0);
            Logger.Debug("num of channels: {0}", numOfChannels);

            byte[] sampleRateBytes = new byte[WAV_SAMPLERATE_SIZE];
            Array.Copy(beingReadBuffer, WAV_SAMPLERATE_OFFSET, sampleRateBytes, 0, WAV_SAMPLERATE_SIZE);
            int sampleRate = BitConverter.ToInt32(sampleRateBytes, 0);
            Logger.Debug("sample rate: {0}", sampleRate);

            this.WaveFormat = new WaveFormat(sampleRate, numOfChannels);

            doneReadingBeingReadBuffer = false;
            beingReadBufferUnreadIndexBookmark = 0;

#if DEBUG
            if (LOG_STREAM_TO_FILE)
            {
                debugFileStream = File.Create(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + @"\radioDebug.wav"); 
            }
#endif

            byte[] ConCatByteArr(byte[] arrA, byte[] arrB)
            {
                byte[] result = new byte[arrA.Length + arrB.Length];
                arrA.CopyTo(result, 0);
                arrB.CopyTo(result, arrA.Length);
                return result;
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int writtenByteCount = 0;
            writeDataFromFilledBuffers(ref writtenByteCount);

            Logger.Debug(".......Read returns with result: {0}", writtenByteCount);
            return writtenByteCount;

            void writeDataFromFilledBuffers(ref int wbc)
            {
                if (doneReadingBeingReadBuffer)
                {
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
                    doneReadingBeingReadBuffer = true;
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
                    doneReadingBeingReadBuffer = false;
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

        ~MyWaveProvider()
        {
            this.Dispose();
        }
    }
}
