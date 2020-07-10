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
    /* implement INotifyPropertyChanged in order to notify the view model 
    whether the play command can be executed */
    class RadioViewModel : INotifyPropertyChanged 

    {
        // create and set up logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // required for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        //constructor
        public RadioViewModel()
        {
            this.PlayCommand = new DelegateCommand(ExecutePlayCommand);
        }

        private void ExecutePlayCommand(object obj)
        {
            // TODO
            Logger.Info("Executing Play Command");
        }

        // ICommand implementations
        public ICommand PlayCommand { protected set; get; }
    }
}
