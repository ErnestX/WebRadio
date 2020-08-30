using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    class BuffersManager
    {
        public int BufferSize { get; }
        public int NumOfBuffers
        {
            get
            {
                return allBuffers.Count;
            }
        }

        private List<byte[]> allBuffers;
        private List<int> availableBufferIndexes;

        public BuffersManager(int bufferSize, int numOfBuffers)
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
                int idx = availableBufferIndexes.Count - 1;
                availableBufferIndexes.RemoveAt(availableBufferIndexes.Count - 1);
                return allBuffers[idx];
            }
        }

        public void RecycleUsedBuffer(byte[] ubf)
        {
            if (ubf == null)
            {
                throw new ArgumentException("Buffer cannot be null");
            }

            int idx = allBuffers.IndexOf(ubf);
            if (idx > -1)
            {
                if (allBuffers[idx].Length != BufferSize)
                {
                    // allBuffers[idx] = new byte[BufferSize];
                    byte[] b = allBuffers[idx];
                    Array.Resize(ref b, BufferSize);
                }
                availableBufferIndexes.Add(idx);
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

        private void CheckAndFixAvailBfIdxs()
        {
            // delete index if out of bound
            List<int> indexToRemove = new List<int>();
            for (int i = 0; i < availableBufferIndexes.Count; i++)
            {
                if (availableBufferIndexes[i] < 0 || availableBufferIndexes[i] >= allBuffers.Count)
                {
                    indexToRemove.Add(i);
                }
            }
            foreach (int idx in indexToRemove)
            {
                availableBufferIndexes.RemoveAt(idx);
                Console.WriteLine("out-of-bound index removed");
            }

            // remove duplicates
            for (int i = 0; i < availableBufferIndexes.Count; i++)
            {
                int dupIdx = availableBufferIndexes.FindIndex(i + 1, idx => idx == availableBufferIndexes[i]);
                if (dupIdx > -1)
                {
                    availableBufferIndexes.RemoveAt(dupIdx);
                    Console.WriteLine("duplicate removed");
                }
            }
        }

        //public void InitWithStreamAndFillFirstBuffer(Stream st)
        //{
        //    throw new NotImplementedException();
        //}

        //public byte[] GetFilledBufferAndFillNextBuffer()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
