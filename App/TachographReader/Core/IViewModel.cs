using System;
using System.ComponentModel;
using Webcal.Windows;

namespace Webcal.Core
{
    public interface IViewModel : INotifyPropertyChanged, IDisposable
    {
        bool HasChanged { get; }

        MainWindowViewModel MainWindow { get; set; }

        Action<bool, object> DoneCallback { get; set; }

        void OnClosing(bool cancelled);
    }
}
