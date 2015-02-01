namespace Webcal.Core
{
    using System.Windows;
    using Shared;
    using Shared.Core;

    public class BaseModalWindowViewModel : BaseNotification
    {
        public Window Window { get; set; }

        public virtual void OnClosing()
        {
        }
    }
}