using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Radio
{
    /// <summary>
    /// adds a layer to a stream to monitor its reading activity
    /// </summary>
    class MonitoredStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        private Stream sourceStream;
        private Timer timer;
        private int totalBytesRead;

        public delegate void OnUpdateHandler(object sender, OnUpdateEventArgs args);
        public event OnUpdateHandler OnUpdate;

        /// <param name="s">source stream to be monitored</param>
        /// <param name="updateInterval">interval between updates of download speed in milliseconds</param>
        public MonitoredStream(Stream s, int updateInterval)
        {
            sourceStream = s;

            timer = new Timer();
            timer.Interval = updateInterval;
            timer.Tick += OnTickHandler;
            timer.Start();

            totalBytesRead = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Console.WriteLine("MonitoredStream reading...");
            int bytesRead = sourceStream.Read(buffer, offset, count);
            totalBytesRead += bytesRead;
            return bytesRead;
        }

        public override void Flush() {throw new NotImplementedException();}
        public override long Seek(long offset, SeekOrigin origin) {throw new NotSupportedException();}
        public override void Write(byte[] buffer, int offset, int count) {throw new NotSupportedException();}
        public override void SetLength(long value) {throw new NotSupportedException();}

        private void OnTickHandler(object sender, EventArgs args)
        {
            OnUpdateHandler handler = OnUpdate;
            if (handler != null)
            {
                handler(this, new OnUpdateEventArgs(totalBytesRead, timer.Interval));
            }
            totalBytesRead = 0;
        }
    }
}
