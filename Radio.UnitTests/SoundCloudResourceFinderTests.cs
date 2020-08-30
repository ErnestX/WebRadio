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