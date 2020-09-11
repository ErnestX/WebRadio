using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Radio.UnitTests
{
    class MonitoredMp3WaveProviderTests
    {
        const string VALID_MP3_STREAM_1 = "https://file-examples-com.github.io/uploads/2017/11/file_example_MP3_700KB.mp3";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateNewProviderFromUri_ValidAndSuccessful_CanRetrieveCorrectWaveFormat() { 
      
            MonitoredMp3WaveProvider provider = new MonitoredMp3WaveProvider(new Uri(VALID_MP3_STREAM_1), 1024, 20);
            Assert.AreEqual("16 bit PCM: 32kHz 2 channels", provider.WaveFormat.ToString());
        }

        [Test]
        public void CreateNewProviderFromSoundcloud_ValidAndSuccessful_NoExceptions() {
            // TODO: use a SoundCloud url
            Uri uri = new Uri("https://cf-media.sndcdn.com/w9nHQlnktPK7.128.mp3?Policy=eyJTdGF0ZW1lbnQiOlt7IlJlc291cmNlIjoiKjovL2NmLW1lZGlhLnNuZGNkbi5jb20vdzluSFFsbmt0UEs3LjEyOC5tcDMiLCJDb25kaXRpb24iOnsiRGF0ZUxlc3NUaGFuIjp7IkFXUzpFcG9jaFRpbWUiOjE1OTg0MTI1NTh9fX1dfQ__&Signature=MlDwwgfG-Myv8udL0A~5lHMc7e7Yf-OGzIir6YpBXkMdrfiXiux0TbdWkkG6bkSBXFzwO6oZE-PhLeAejW7drTRgvJtA5wxCxRjSfGRZ2OTsYN5RwK7OrxiKRYeWz8aYK8exFy2fYAEdvAJvpykupxLAV-Jen3I3kpVIciZCwqyVPeAIlVOBMmp9ERvRgFEE8XI5n0Da00l8spQuhjT7XstWYaFXEGraBUNkoiNxbCASRelneV5MOw3IURTctoe9x3-xx5KaOhPPnWLSnjNihivG0m56u2bQ6VkcM1H3IvzQJMpSslQby4ucn~G3jUqq~0vmPKB6~jmIz0CjuUGoFQ__&Key-Pair-Id=APKAI6TU7MMXM5DG6EPQ");
            Assert.DoesNotThrow(() => new MonitoredMp3WaveProvider(uri, 1024, 20));
        }
    }
}
