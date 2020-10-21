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
        public MonitoredStream Stream { get; }
        public ConnectedEventArgs(MonitoredStream s)
        {
            this.Stream = s;
        }
    }
}
