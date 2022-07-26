using System;
using System.Windows.Input;

namespace TextParserApp
{
    /// <summary>
    /// Implementation of the ICommand interface for relaying commands to execute 
    /// associated actions.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action<object> action;
        private Predicate<object> canExecute;

        /// <summary>
        /// Constructor for a RelayCommand.
        /// </summary>
        /// <param name="action">Action to be executed if command is triggered.</param>
        /// <param name="canExecute">Fuction to evaluate if the associated GUI command is enabled.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RelayCommand(Action<object> action, Predicate<object> canExecute)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            this.action = action;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// This command calls the respective delegate to evaluate if the calling GUI 
        /// object should be enabled.
        /// </summary>
        /// <param name="parameter">parameter needed for evaluation</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
            {
                return canExecute(parameter);
            }

            return true;
        }

        /// <summary>
        /// Event triggered to evaluate if the activity (enable) of a GUI element changed,
        /// with adder (+=) and remover (-=) redirected to the RequerySuggested Event 
        /// property of the CommandManager.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Executes the action defined for the calling GUI object.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            action(parameter);
        }
    }
}
