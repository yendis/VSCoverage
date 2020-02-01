using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VSCoverage.Model
{
    public class AsyncCommand : IAsyncCommand
    {
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private readonly Func<Task> _command;
        public AsyncCommand(Func<Task> command)
        {
            _command = command;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public Task ExecuteAsync(object parameter)
        {
            return _command();
        }
    }
}
