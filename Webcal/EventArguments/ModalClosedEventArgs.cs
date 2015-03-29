namespace TachographReader.EventArguments
{
    using System;

    public class ModalClosedEventArgs : EventArgs
    {
        public ModalClosedEventArgs(bool saved, object parameter)
        {
            Saved = saved;
            Parameter = parameter;
        }

        public bool Saved { get; private set; }
        public object Parameter { get; set; }
    }
}