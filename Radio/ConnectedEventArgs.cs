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
        public IWaveProvider WaveProvider { get; }
        public ConnectedEventArgs(IWaveProvider s)
        {
            this.WaveProvider = s;
        }
    }
}
