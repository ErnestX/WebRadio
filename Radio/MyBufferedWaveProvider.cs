﻿using Microsoft.Scripting;
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
    /// <summary>
    /// </summary>
    class MyBufferedWaveProvider : IWaveProvider,IDisposable
    {
        private Stream sourceStream;
        private bool doneReadingBeingReadBuffer;
        private int beingReadBufferUnreadIndexBookmark;
        private byte[] beingReadBuffer;
        private Bufferer bufferer;
#if DEBUG
        private bool LOG_STREAM_TO_FILE = true; // switch off to avoid conflict when running tests in parallel
        private Stream debugFileStream;
#endif
        public WaveFormat WaveFormat {private set; get;}

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Mp3WaveProviderDebug");

        public MyBufferedWaveProvider(Stream stream, int bufferSize)
        {
            sourceStream = stream;

            bufferer = new Bufferer(sourceStream, bufferSize);

            beingReadBuffer = bufferer.GetNextBuffer();
            while(beingReadBuffer.Length < MyWAVReader.WavHeaderSize())
            {
                beingReadBuffer = ConCatByteArr(beingReadBuffer, bufferer.GetNextBuffer());
            }
            byte[] header = (byte[])beingReadBuffer.Clone();
            this.WaveFormat = new WaveFormat(MyWAVReader.GetSampleRateFromHeader(header), MyWAVReader.GetNumChannelFromHeader(header));

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
#if DEBUG
            if (debugFileStream != null)
            {
                debugFileStream.Close();
            }
#endif
        }

        ~MyBufferedWaveProvider()
        {
            this.Dispose();
        }
    }
}
