namespace TachographReader.Core
{
    using System.Windows;
    using Connect.Shared.Models;
    using DataModel.Core;
    using Shared.Core;

    public class BaseModalWindowViewModel : BaseNotification
    {
        public Window Window { get; set; }

        public TService GetInstance<TService>()
        {
            return ContainerBootstrapper.Container.GetInstance<TService>();
        }

        public virtual void OnClosing()
        {
        }
    }
}