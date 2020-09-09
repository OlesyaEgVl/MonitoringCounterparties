using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ContragentAnalyse.ViewModel.Commands
{
    public class SearchCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Action action { get; set; }
        
        public SearchCommand(Action action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action.Invoke();
        }
    }
}
