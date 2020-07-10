using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Radio
{
    class DelegateCommand : ICommand
    {
        Action<object> execute;

        // required by ICommand
        public event EventHandler CanExecuteChanged;

        // constructor
        public DelegateCommand(Action<Object> execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true; 
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
