using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    class ConnectedEventArgs : EventArgs
    {
        public MyBufferedWaveProvider mbwp { get; }
        public ConnectedEventArgs(MyBufferedWaveProvider s)
        {
            this.mbwp = s;
        }
    }
}
