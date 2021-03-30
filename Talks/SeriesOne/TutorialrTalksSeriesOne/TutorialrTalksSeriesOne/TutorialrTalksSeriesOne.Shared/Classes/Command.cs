using System;
using System.Windows.Input;

namespace TutorialrTalksSeriesOne
{
    public class Command<TAction> : ICommand
    {
        private readonly Action<TAction> _execute;
        private readonly Predicate<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Action<TAction> execute) : this(execute, null) { }

        public Command(Action<TAction> execute, Predicate<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) =>
            _canExecute == null || _canExecute((bool)parameter);

        public void Execute(object parameter) =>
            _execute((TAction)parameter);

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
