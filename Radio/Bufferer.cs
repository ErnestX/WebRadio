using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Radio
{
    /// <summary>
    /// obtain data from a stream and encapsulates the buffering logic
    /// </summary>
    class Bufferer
    {
        const int INITIAL_BUFFER_NUM = 4;

        private BufferReuseManager buffersManager;
        private Queue<byte[]> filledBuffers;
        private static SemaphoreSlim readStreamSemaphore = new SemaphoreSlim(1, 1);

        public int DefaultBufferSize { get; }

        // return false if end of stream is reached and no filledBuffer is left, else true
        public bool EndOfStream { get; private set; }

        private Stream sourceStream;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Mp3WaveProviderDebug");

        public Bufferer(Stream stream, int bufferSize)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("source stream is not readable");
            }

            sourceStream = stream;
            DefaultBufferSize = bufferSize;
            this.InitializeBuffers();
            this.StartBuffering();
    }

        private void InitializeBuffers()
        {
            buffersManager = new BufferReuseManager(DefaultBufferSize, INITIAL_BUFFER_NUM);
            filledBuffers = new Queue<byte[]>();
        }

        private void StartBuffering()
        {
            // start with two buffers to guarantee at least one filled buffer in reserve
                this.FillABufferFromSourceStream();
                this.FillABufferFromSourceStream();
        }

        public byte[] GetNextBuffer()
        {
            if (filledBuffers.Count < 1)
            {
                Task.Run((Func<Task>)(async () => { this.FillABufferFromSourceStreamAsync(); })).Wait();
                this.FillABufferFromSourceStreamAsync();
            }
            else if (filledBuffers.Count < 2)
            {
                this.FillABufferFromSourceStreamAsync();
            }

            return filledBuffers.Count < 1 ? null : filledBuffers.Dequeue();
        }

        public void TryRecycleBuffer(byte[] bf)
        {
            if (buffersManager.BelongToTheManager(bf))
            {
                buffersManager.RecycleUsedBuffer(bf);
            }
        }

        /// <returns>false if the end of the stream has been reached and no data was read, ortherwise true</returns>
        private async Task<bool> FillABufferFromSourceStreamAsync()
        {
            await readStreamSemaphore.WaitAsync();
            bool result;
            try
            {
                result = await Task.Run(this.FillABufferFromSourceStream);
            }
            finally
            {
                readStreamSemaphore.Release();
            }
            return result;
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
                Debug.Assert(buffer != null);
                filledBuffers.Enqueue(buffer);
                Logger.Debug(">>>>>>>Downloaded a buffer; filledBuffers count: {0}", filledBuffers.Count);
                return true;
            }
            else if (unreadBytes < buffer.Length)
            {
                // buffer is not completely filled, crop unused portion
                byte[] croppedBf = new byte[buffer.Length - unreadBytes];
                Array.Copy(buffer, croppedBf, croppedBf.Length);
                buffersManager.RecycleUsedBuffer(buffer);
                Debug.Assert(croppedBf != null);
                filledBuffers.Enqueue(croppedBf);
                Logger.Debug(">>>>>>>Downloaded a buffer; filledBuffers count: {0}", filledBuffers.Count);
                return true;
            }
            else
            {
                // nothing is read
                return false;
            }
        }
    }
}
