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
        public void CreateNewProviderFromSoundcloud_ValidAndSuccessful_NoExceptions() {
            // TODO: move to SoundCloudResourceFinderTest and use a SoundCloud url
            Uri uri = new Uri("https://cf-media.sndcdn.com/w9nHQlnktPK7.128.mp3?Policy=eyJTdGF0ZW1lbnQiOlt7IlJlc291cmNlIjoiKjovL2NmLW1lZGlhLnNuZGNkbi5jb20vdzluSFFsbmt0UEs3LjEyOC5tcDMiLCJDb25kaXRpb24iOnsiRGF0ZUxlc3NUaGFuIjp7IkFXUzpFcG9jaFRpbWUiOjE1OTg0MTI1NTh9fX1dfQ__&Signature=MlDwwgfG-Myv8udL0A~5lHMc7e7Yf-OGzIir6YpBXkMdrfiXiux0TbdWkkG6bkSBXFzwO6oZE-PhLeAejW7drTRgvJtA5wxCxRjSfGRZ2OTsYN5RwK7OrxiKRYeWz8aYK8exFy2fYAEdvAJvpykupxLAV-Jen3I3kpVIciZCwqyVPeAIlVOBMmp9ERvRgFEE8XI5n0Da00l8spQuhjT7XstWYaFXEGraBUNkoiNxbCASRelneV5MOw3IURTctoe9x3-xx5KaOhPPnWLSnjNihivG0m56u2bQ6VkcM1H3IvzQJMpSslQby4ucn~G3jUqq~0vmPKB6~jmIz0CjuUGoFQ__&Key-Pair-Id=APKAI6TU7MMXM5DG6EPQ");
            Assert.DoesNotThrow(() => new Mp3WaveProvider(uri, 20480));
        }

        [Test]
        public void Read_WhenCalled_OutputCorrectly()
        {
            test(VALID_MP3_STREAM_1, 2048, 1024);
            void test(string url, int bufferSize, int readIncrement)
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
                    Assert.IsTrue(testBuffer.SequenceEqual(expectedBuffer));
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
}
