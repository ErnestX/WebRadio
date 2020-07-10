using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using NLog.Fluent;
using System.Windows.Input;

namespace Radio
{
    class RadioViewModel : INotifyPropertyChanged

    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CanExecuteChanged;

        //constructor
        public RadioViewModel()
        {
            this.PlayCommand = new DelegateCommand(ExecutePlayCommand);
        }

        private void ExecutePlayCommand(object obj)
        {
            Logger.Info("Executing Play Command");
        }

        // ICommand implementations
        public ICommand PlayCommand { protected set; get; }
    }
}
