using NUnit.Framework;
using System;

namespace Radio.UnitTests
{
    public class SoundCloudResourceFinderTests
    {
        private const string YOUTUBE_LINK_VALID_1 = "https://www.youtube.com/watch?v=9ORO3cnPu7k";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateNewProviderFromSoundcloud_ValidAndSuccessful_NoExceptions()
        {
            // TODO: move to SoundCloudResourceFinderTest and use a SoundCloud url
            Uri uri = new Uri("https://cf-media.sndcdn.com/w9nHQlnktPK7.128.mp3?Policy=eyJTdGF0ZW1lbnQiOlt7IlJlc291cmNlIjoiKjovL2NmLW1lZGlhLnNuZGNkbi5jb20vdzluSFFsbmt0UEs3LjEyOC5tcDMiLCJDb25kaXRpb24iOnsiRGF0ZUxlc3NUaGFuIjp7IkFXUzpFcG9jaFRpbWUiOjE1OTg0MTI1NTh9fX1dfQ__&Signature=MlDwwgfG-Myv8udL0A~5lHMc7e7Yf-OGzIir6YpBXkMdrfiXiux0TbdWkkG6bkSBXFzwO6oZE-PhLeAejW7drTRgvJtA5wxCxRjSfGRZ2OTsYN5RwK7OrxiKRYeWz8aYK8exFy2fYAEdvAJvpykupxLAV-Jen3I3kpVIciZCwqyVPeAIlVOBMmp9ERvRgFEE8XI5n0Da00l8spQuhjT7XstWYaFXEGraBUNkoiNxbCASRelneV5MOw3IURTctoe9x3-xx5KaOhPPnWLSnjNihivG0m56u2bQ6VkcM1H3IvzQJMpSslQby4ucn~G3jUqq~0vmPKB6~jmIz0CjuUGoFQ__&Key-Pair-Id=APKAI6TU7MMXM5DG6EPQ");
            Assert.DoesNotThrow(() => new Mp3WaveProvider(uri, 20480));
        }

        [Test]
        public void FindVideoResByYoutubeLink_ValidAndSuccessful_ReturnVideoRes()
        {
            try
            {
                SoundCloudResourceFinder.FindAudioResBySCLink(new Uri(YOUTUBE_LINK_VALID_1));
            }
            catch
            {
                Assert.Fail();
            }

            Assert.Pass();
        }
    }
}