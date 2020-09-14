using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ContragentAnalyse.ViewModel.Commands
{
    public class MyCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> Action { get; set; }
        public MyCommand(Action<T> action)
        {
            Action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is T && parameter != null)
            {
                Action?.Invoke((T)parameter);
            }
        }
    }
}
