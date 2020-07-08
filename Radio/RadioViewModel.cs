using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using NLog.Fluent;

namespace Radio
{
    class RadioViewModel : INotifyPropertyChanged
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler PropertyChanged;

        //constructor
        public RadioViewModel()
        {
        }

        public string PlayCommand()
        {
            Logger.Debug("---------test---------");
            return null;
        }
    }
}
