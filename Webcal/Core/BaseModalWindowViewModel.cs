namespace Webcal.Core
{
    using System.Windows;
    using Shared;

    public class BaseModalWindowViewModel : BaseNotification
    {
        public Window Window { get; set; }

        public virtual void OnClosing()
        {
        }
    }
}