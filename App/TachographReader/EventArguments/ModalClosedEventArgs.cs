using System;

namespace Webcal.EventArguments
{
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
