using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Radio.UnitTests
{
    class BuffersManagerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Initialization_InvalidInput_ThrowArgumentException()
        {
            Assert.That(() => new BuffersManager(128, 0), 
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Buffer size and buffer number cannot be zero or negative"));

            Assert.That(() => new BuffersManager(128, -10), 
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Buffer size and buffer number cannot be zero or negative"));

            Assert.That(() => new BuffersManager(0, 0), 
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Buffer size and buffer number cannot be zero or negative"));

            Assert.That(() => new BuffersManager(-128, 0), 
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Buffer size and buffer number cannot be zero or negative"));
        }

        [Test]
        public void RecycleUsedBuffer_InvalidInput_ThrowArgumentException()
        {
            const int BUFFER_SIZE = 128;
            List<byte[]> bfs = new List<byte[]>(0);
            BuffersManager bfm = new BuffersManager(BUFFER_SIZE, 5);

            for (int i = 0; i < 5; i++)
            {
                bfs.Add(bfm.CheckoutNewBuffer());
            }

            Assert.That(() => bfm.RecycleUsedBuffer(null), 
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Buffer cannot be null"));

            byte[] randomBf1 = new byte[BUFFER_SIZE-1];
            Assert.That(() => bfm.RecycleUsedBuffer(randomBf1),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("This buffer doesn't belong to the buffer manager"));

            byte[] randomBf2 = new byte[BUFFER_SIZE];
            Assert.That(() => bfm.RecycleUsedBuffer(randomBf2),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("This buffer doesn't belong to the buffer manager"));

            byte[] randomBf3 = new byte[BUFFER_SIZE];
            Array.Copy(bfs[1], randomBf3,bfs[1].Length);
            Assert.That(() => bfm.RecycleUsedBuffer(randomBf3),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("This buffer doesn't belong to the buffer manager"));
        }

        [Test]
        public void CheckOutNewBuffers_WhenCalled_ReturnUniqueBuffersOfRightSizeAndAllocOnlyWhenNecessary()
        {
            const int BUFFER_SIZE = 128;
            List<byte[]> bfs = new List<byte[]>(0);

            BuffersManager bfm = new BuffersManager(BUFFER_SIZE, 5);
            Assert.IsTrue(bfm.NumOfBuffers == 5);

            for (int i = 0; i < 5; i++)
            {
                bfs.Add(bfm.CheckoutNewBuffer());
            }
            Assert.IsTrue(bfm.NumOfBuffers == 5);

            for (int i = 5; i < 10; i++)
            {
                bfs.Add(bfm.CheckoutNewBuffer());
            }
            Assert.IsTrue(bfm.NumOfBuffers == 10);

            bfm.RecycleUsedBuffer(bfs[0]);
            bfm.RecycleUsedBuffer(bfs[3]);
            bfm.RecycleUsedBuffer(bfs[7]);
            bfm.RecycleUsedBuffer(bfs[7]);
            bfm.RecycleUsedBuffer(bfs[6]);
            bfm.RecycleUsedBuffer(bfs[0]);
            bfm.RecycleUsedBuffer(bfs[9]);
            bfs.RemoveAt(0);
            bfs.RemoveAt(3);
            bfs.RemoveAt(7);
            bfs.RemoveAt(6);
            bfs.RemoveAt(9);

            for (int i = 5; i < 15; i++)
            {
                bfs.Add(bfm.CheckoutNewBuffer());
            }
            Assert.IsTrue(bfm.NumOfBuffers == 15);

            // check uniqueness
            if (bfs.Distinct().Count() != bfs.Count)
            {
                Assert.Fail("returned buffers are not unique");
            }

            // check size
            foreach (byte[] bf in bfs)
            {
                if (bf.Length != BUFFER_SIZE)
                {
                    Assert.Fail("at least one of the returned buffers is of wrong size");
                }
            }

            Assert.Pass();
        }
    }
}
