using System;
using System.Collections;
using System.IO;
using System.Linq;
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
        public void ReadBytesFromStream_NotOverRead_ReadAndOutputCorrectly()
        {
            byte[] randomBytes = new byte[1000];
            Random rnd = new Random();
            rnd.NextBytes(randomBytes);

            byte[] bf = new byte[1000];
            using (var testStream = new MemoryStream(randomBytes))
            {
                int bytesUnread = StreamReader.ReadBytesFromStreamAsync(testStream, bf, 0, 500).Result;
                Assert.AreEqual(0, bytesUnread);
                bytesUnread = StreamReader.ReadBytesFromStreamAsync(testStream, bf, 500, 500).Result;
                Assert.AreEqual(0, bytesUnread);
            }
            Assert.IsTrue(bf.SequenceEqual(randomBytes));
        }

        [Test]
        public void ReadBytesFromStream_OverRead_ReadAndOutputCorrectly()
        {
            byte[] randomBytes = new byte[900];
            Random rnd = new Random();
            rnd.NextBytes(randomBytes);

            byte[] bf = new byte[1000];
            using (var testStream = new MemoryStream(randomBytes))
            {
                int bytesUnread = StreamReader.ReadBytesFromStreamAsync(testStream, bf, 0, 500).Result;
                Assert.AreEqual(0, bytesUnread);
                bytesUnread = StreamReader.ReadBytesFromStreamAsync(testStream, bf, 500, 500).Result;
                Assert.AreEqual(100, bytesUnread);
            }
            Array.Resize(ref bf, 900);
            Assert.IsTrue(bf.SequenceEqual(randomBytes));
        }
    }
}
