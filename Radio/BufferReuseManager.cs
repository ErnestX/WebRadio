using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    /// <summary>
    /// Manage a collection of buffers so that they can be reused and minimize alloc
    /// </summary>
    class BufferReuseManager
    {
        public int BufferSize { get; }
        public int NumOfBuffers
        {
            get
            {
                return allBuffers.Count;
            }
        }
        public int NumOfAvailableBuffers
        {
            get
            {
                return availableBufferIndexes.Count;
            }
        }

        private List<byte[]> allBuffers;
        private List<int> availableBufferIndexes;

        public BufferReuseManager(int bufferSize, int numOfBuffers)
        {
            if (bufferSize < 1 || numOfBuffers < 1)
            {
                throw new ArgumentException("Buffer size and buffer number cannot be zero or negative");
            }

            allBuffers = new List<byte[]>(numOfBuffers);
            availableBufferIndexes = new List<int>(0);

            for (int i = 0; i < numOfBuffers; i++)
            {
                allBuffers.Add(new byte[bufferSize]);
                availableBufferIndexes.Add(i);
            }

            BufferSize = bufferSize;
        }

        public byte[] CheckoutNewBuffer()
        {
            if (availableBufferIndexes.Count < 1)
            {
                this.CreateAndRegisterBuffer();
                return CheckoutNewBuffer();
            } 
            else
            {
                int bIdx = availableBufferIndexes.Last();
                availableBufferIndexes.RemoveAt(availableBufferIndexes.Count - 1);
                return allBuffers[bIdx];
            }
        }

        public void RecycleUsedBuffer(byte[] ubf)
        {
            if (ubf == null)
            {
                throw new ArgumentException("Buffer cannot be null");
            }

            int bIdx = allBuffers.IndexOf(ubf);
            if (bIdx > -1)
            {
                if (availableBufferIndexes.IndexOf(bIdx) == -1)
                {
                    // do recycling
                    Debug.Assert(ubf.Length == BufferSize); // always true because array cannot be resized and the address can't be reallocated to another array because buffersmanager keeps the reference
                    availableBufferIndexes.Add(bIdx);
                }
                else
                {
                    Console.WriteLine("buffer is already recycled");
                }
            } 
            else
            {
                throw new ArgumentException("This buffer doesn't belong to the buffer manager");
            }
        }

        private void CreateAndRegisterBuffer()
        {
            allBuffers.Add(new byte[BufferSize]);
            availableBufferIndexes.Add(allBuffers.Count - 1);
        }

        public bool BelongToTheManager(byte[] bf)
        {
            int bIdx = allBuffers.IndexOf(bf);
            return bIdx > -1;
        }
    }
}
