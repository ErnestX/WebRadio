using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    class StreamReader
    {
        /// <summary>read specified bytes from stream</summary>
        /// <returns>number of bytes failed to read because the end of stream is reached; 0 if all bytes are read successfully</returns>
        static public int ReadBytesFromStream(Stream stream, byte[] buffer, int offset, int bytesToRead)
        {
            int bytesRead;
            int totalBytesRead = 0;

            int timesReadZeroByte = 0;
            const int TIMES_READ_ZERO_BYTE_BEFORE_ABORT = 3;

            int bytesYetToRead = bytesToRead;
            int readOffset = offset;
            while (bytesYetToRead > 0)
            {
                bytesRead = stream.Read(buffer, readOffset, bytesYetToRead);
                totalBytesRead += bytesRead;

                if (bytesRead == 0)
                {
                    timesReadZeroByte++;
                }

                if (timesReadZeroByte >= TIMES_READ_ZERO_BYTE_BEFORE_ABORT)
                {
                    return bytesYetToRead;
                }

                readOffset += bytesRead;
                bytesYetToRead = bytesToRead - totalBytesRead;
                Debug.Assert(bytesYetToRead >= 0);
            }

            return 0;
        }
    }
}
