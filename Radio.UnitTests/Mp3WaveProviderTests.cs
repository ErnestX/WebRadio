using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;

namespace Radio.UnitTests
{
    class Mp3WaveProviderTests
    {
        const string VALID_MP3_STREAM_1 = "https://file-examples-com.github.io/uploads/2017/11/file_example_MP3_700KB.mp3"; //746kb, 27sec, 224kbps
        const string VALID_MP3_STREAM_2 = "http://www.hochmuth.com/mp3/Tchaikovsky_Nocturne__orch.mp3"; // 794kb, 4min 30sec, 24kbps

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateNewProviderFromUri_ValidAndSuccessful_CanRetrieveCorrectWaveFormat() { 
      
            Mp3WaveProvider provider = new Mp3WaveProvider(new Uri(VALID_MP3_STREAM_1), 20480);
            Assert.AreEqual("16 bit PCM: 32kHz 2 channels", provider.WaveFormat.ToString());
        }

        [Test]
        public void Read_BufferSizeEqualToReadIncrement_OutputCorrectly()
        {
            ReadTestHelper(VALID_MP3_STREAM_1, 1024, 1024);
            ReadTestHelper(VALID_MP3_STREAM_1, 4096, 4096);
            ReadTestHelper(VALID_MP3_STREAM_1, 4095, 4095);
            ReadTestHelper(VALID_MP3_STREAM_1, 2083, 2083);
        }

        [Test]
        public void Read_BufferSizeLargerThanReadIncrement_OutputCorrectly()
        {
            ReadTestHelper(VALID_MP3_STREAM_1, 1025, 1024); // error: destination array not long enough
            ReadTestHelper(VALID_MP3_STREAM_1, 4096, 1024); // error: destination array not long enough
        }

        [Test]
        public void Read_BufferSizeSmallerThanReadIncrement_OutputCorrectly()
        {
            ReadTestHelper(VALID_MP3_STREAM_1, 1024, 2048);
            ReadTestHelper(VALID_MP3_STREAM_1, 1024, 2049);
            ReadTestHelper(VALID_MP3_STREAM_1, 512, 4095);
        }

        public void ReadTestHelper(string url, int bufferSize, int readIncrement)
        {
            HttpWebRequest req;
            HttpWebResponse tempRes = null;
            Mp3WaveProvider mwp = null;

            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                tempRes = (HttpWebResponse)req.GetResponse();
                Stream tempStream = tempRes.GetResponseStream();

                byte[] expectedBuffer = new byte[1024 * 800];
                int offset = 0;
                int bytesRead;
                do
                {
                    bytesRead = tempStream.Read(expectedBuffer, offset, readIncrement);
                    offset += bytesRead;
                } while (bytesRead > 0);


                mwp = new Mp3WaveProvider(new Uri(url), bufferSize);
                byte[] testBuffer = new byte[1024 * 800];
                offset = 0;
                do
                {
                    bytesRead = mwp.Read(testBuffer, offset, readIncrement);
                    offset += bytesRead;
                } while (bytesRead > 0);

                Assert.IsTrue(testBuffer.Length == expectedBuffer.Length);
                for (int i = 0; i < expectedBuffer.Length; i++)
                {       
                    Assert.AreEqual(expectedBuffer[i], testBuffer[i],String.Format("inequity found at index: {0}", i));
                }
            }
            finally
            {
                if (tempRes != null)
                {
                    tempRes.Close();
                }

                if (mwp != null)
                {
                    mwp.Dispose();
                }
            }
        }
    }
}

