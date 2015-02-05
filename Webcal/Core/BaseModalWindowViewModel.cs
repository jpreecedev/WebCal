﻿namespace Webcal.Core
{
    using System.Windows;
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