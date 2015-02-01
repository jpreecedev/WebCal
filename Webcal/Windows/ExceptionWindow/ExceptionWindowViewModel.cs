namespace Webcal.Windows.ExceptionWindow
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using Core;
    using DataModel.Core;
    using Library;
    using Properties;
    using Shared;
    using Shared.Helpers;
    using Shared.Models;

    public class ExceptionWindowViewModel : BaseModalWindowViewModel
    {
        public ExceptionWindowViewModel()
        {
            Repository = ContainerBootstrapper.Container.GetInstance<IRepository<DetailedException>>();
            Exceptions = new ObservableCollection<DetailedException>(Repository.GetAll());

            ExportCommand = new DelegateCommand<object>(OnExport);
            CloseCommand = new DelegateCommand<Window>(OnClose);
        }

        public IRepository<DetailedException> Repository { get; set; }
        public ObservableCollection<DetailedException> Exceptions { get; set; }
        public DelegateCommand<object> ExportCommand { get; set; }
        public DelegateCommand<Window> CloseCommand { get; set; }

        private void OnExport(object obj)
        {
            if (!Exceptions.IsNullOrEmpty())
            {
                DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.Data, string.Format("{0}\\Technical Information.dat", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
                if (result.Result == true)
                {
                    ExceptionPolicy.Serialize(Exceptions.Select(t => new DiscreteException
                    {
                        ExceptionDetails = t.ExceptionDetails,
                        Occurred = t.Occurred.ToString("dd-MMM-yyyy HH:mm")
                    })
                    .ToList(),
                    result.FileName);
                }

                MessageBoxHelper.ShowMessage(Resources.TXT_EXPORT_COMPLETE, Window);
            }
            else
            {
                MessageBoxHelper.ShowMessage(Resources.TXT_NOTHING_TO_EXPORT, Window);
            }
        }

        private static void OnClose(Window window)
        {
            if (window == null)
            {
                return;
            }

            window.Close();
        }
    }
}