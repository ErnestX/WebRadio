using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;

namespace Radio.UnitTests
{
    [TestFixture,SingleThreaded] // prevent parallel read requests
    class MyBufferedWaveProviderTests
    {
        const string VALID_WAV_STREAM_1 = "https://file-examples-com.github.io/uploads/2017/11/file_example_WAV_1MG.wav"; // 1MB
        const string VALID_MP3_STREAM_1 = "https://file-examples-com.github.io/uploads/2017/11/file_example_MP3_700KB.mp3"; //746kb, 27sec, 224kbps
        const string VALID_MP3_STREAM_2 = "http://www.hochmuth.com/mp3/Tchaikovsky_Nocturne__orch.mp3"; // 794kb, 4min 30sec, 24kbps

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Read_BufferSizeEqualToReadIncrement_OutputCorrectly()
        {
            ReadTestHelper(VALID_WAV_STREAM_1, 1024, 1024);
            ReadTestHelper(VALID_WAV_STREAM_1, 4096, 4096);
            ReadTestHelper(VALID_WAV_STREAM_1, 4095, 4095);
            ReadTestHelper(VALID_WAV_STREAM_1, 2083, 2083);
        }

        [Test]
        public void Read_OneBufferFitsAll_OutputCorrectly()
        {
            ReadTestHelper(VALID_WAV_STREAM_1, 800000, 512);
        }

        [Test]
        public void Read_BufferSizeLargerThanReadIncrement_OutputCorrectly()
        {
            ReadTestHelper(VALID_WAV_STREAM_1, 1025, 1024); 
            ReadTestHelper(VALID_WAV_STREAM_1, 2047, 1024); 
            ReadTestHelper(VALID_WAV_STREAM_1, 2048, 1024); 
            ReadTestHelper(VALID_WAV_STREAM_1, 2049, 1024); 
            ReadTestHelper(VALID_WAV_STREAM_1, 4096, 512);
        }

        [Test]
        public void Read_BufferSizeSmallerThanReadIncrement_OutputCorrectly()
        {
            ReadTestHelper(VALID_WAV_STREAM_1, 1024, 1025);
            ReadTestHelper(VALID_WAV_STREAM_1, 1024, 2048);
            ReadTestHelper(VALID_WAV_STREAM_1, 1024, 2049);
            ReadTestHelper(VALID_WAV_STREAM_1, 512, 4095);
        }

        public void ReadTestHelper(string url, int bufferSize, int readIncrement)
        {
            HttpWebRequest req1;
            HttpWebResponse tempRes1 = null;
            MyBufferedWaveProvider mwp1 = null;

            byte[] expectedBuffer = new byte[1024 * 1024 * 2]; // assume the test stream size < 2MB
            int expectedOffset = 0;
            int bytesRead;
            int zeroCounter = 0;

            try
            {
                req1 = (HttpWebRequest)WebRequest.Create(url);
                tempRes1 = (HttpWebResponse)req1.GetResponse();
                Stream tempStream = tempRes1.GetResponseStream();

                do
                {
                    bytesRead = tempStream.Read(expectedBuffer, expectedOffset, readIncrement);
                    expectedOffset += bytesRead;

                    if (bytesRead == 0)
                    {
                        zeroCounter++;
                    }
                    else
                    {
                        zeroCounter = 0;
                    }
                } while (zeroCounter < 4);
            }
            finally
            {
                if (tempRes1 != null)
                {
                    tempRes1.Close();
                }

                if (mwp1 != null)
                {
                    mwp1.Dispose();
                }
            }

            HttpWebRequest req2;
            HttpWebResponse tempRes2 = null;
            MyBufferedWaveProvider mwp2 = null;

            
            byte[] testBuffer = new byte[1024 * 1024 * 2]; // assume the test stream size < 2MB
            int testOffset = 0;
            zeroCounter = 0;

            try
            {
                req2 = (HttpWebRequest)WebRequest.Create(url);
                tempRes2 = (HttpWebResponse)req2.GetResponse();
                Stream tempStream = tempRes2.GetResponseStream();

                mwp2 = new MyBufferedWaveProvider(tempStream, bufferSize);
                do
                {
                    bytesRead = mwp2.Read(testBuffer, testOffset, readIncrement);
                    testOffset += bytesRead;

                    if (bytesRead == 0)
                    {
                        zeroCounter++;
                    }
                    else
                    {
                        zeroCounter = 0;
                    }
                } while (zeroCounter < 4);
            }
            finally
            {
                if (tempRes2 != null)
                {
                    tempRes2.Close();
                }

                if (mwp2 != null)
                {
                    mwp2.Dispose();
                }
            }

            Assert.AreEqual(256, expectedBuffer.Distinct().Count());
            Assert.IsTrue(testBuffer.Length == expectedBuffer.Length);
            TestContext.Out.WriteLine("expected read length: {0}; actual read length: {1}", expectedOffset, testOffset);
            for (int i = 0; i < expectedBuffer.Length; i++)
            {
                Assert.AreEqual(expectedBuffer[i], testBuffer[i], String.Format("inequity found at index: {0}", i));
            }
        }
    }
}

