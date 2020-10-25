using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    /// <summary>
    /// adds a layer to a audio stream to decode it to wave data as it comes in
    /// </summary>
    class DecodingStream : Stream
    {
        const int BYTE_NEEDED_FOR_INIT = 16384; // have to cover the first two frame headers. 

        private Stream sourceStream;
        private Mp3Decoder mp3Decoder;
        private byte[] initBuffer;
        private int initBufferUnreadIndexBookmark;

        public WaveFormat WaveFormat { get; }
        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public DecodingStream(Stream s, Mp3Decoder md)
        {
            if (!s.CanRead)
            {
                throw new ArgumentException("source stream is not readable");
            }

            sourceStream = s;
            mp3Decoder = md;

            // read enough data to retrieve the wave format
            initBuffer = new byte[BYTE_NEEDED_FOR_INIT];
            int unreadBytes = StreamReader.ReadBytesFromStream(sourceStream, initBuffer, 0, initBuffer.Length);
            if (unreadBytes > 0)
            {
                Array.Resize(ref initBuffer, initBuffer.Length - unreadBytes);
            }
            Mp3FileReader mp3FileReader = new Mp3FileReader(new MemoryStream(initBuffer));
            WaveFormat = mp3FileReader.WaveFormat;

            initBufferUnreadIndexBookmark = 0;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}
