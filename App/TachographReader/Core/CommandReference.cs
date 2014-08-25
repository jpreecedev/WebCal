namespace Webcal.Core
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    public class CommandReference : Freezable, ICommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof (ICommand), typeof (CommandReference), new PropertyMetadata(OnCommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof (object), typeof (CommandReference), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }


        public bool CanExecute(object parameter)
        {
            if (Command != null)
                return Command.CanExecute(CommandParameter);
            return false;
        }

        public void Execute(object parameter)
        {
            Command.Execute(CommandParameter);
        }

        public event EventHandler CanExecuteChanged;
        
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var commandReference = d as CommandReference;
            if (commandReference != null)
            {
                var oldCommand = e.OldValue as ICommand;
                var newCommand = e.NewValue as ICommand;

                if (oldCommand != null)
                    oldCommand.CanExecuteChanged -= commandReference.CanExecuteChanged;
                if (newCommand != null)
                    newCommand.CanExecuteChanged += commandReference.CanExecuteChanged;
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}