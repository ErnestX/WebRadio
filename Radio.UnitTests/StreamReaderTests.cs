using System;
using System.IO;
using NUnit.Framework;

namespace Radio.UnitTests
{
    class StreamReaderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ReadBytesFromStream_WhenTheStreamIsAMultipleOfTheBufferSize_ReadCorrectly()
        {
            int bufferSize = 16;
            byte[] randomBytes = new byte[bufferSize * 1000];
            Random rnd = new Random();
            rnd.NextBytes(randomBytes);

            using (var testStream = new MemoryStream(randomBytes))
            {
                //MonitoredMp3WaveProvider mwp = new MonitoredMp3WaveProvider()
            }
        }

        [Test]
        public void ReadBytesFromStream_WhenTheStreamIsNotAMultipleOfTheBufferSize_ReadCorrectly()
        {

        }
    }
}
