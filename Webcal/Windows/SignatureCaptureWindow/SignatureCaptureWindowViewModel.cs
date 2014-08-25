namespace Webcal.Windows.SignatureCaptureWindow
{
    using System;
    using System.Drawing;
    using System.Windows;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Imaging;
    using Library;
    using Properties;
    using Shared;
    using BaseNotification = Core.BaseNotification;

    public class SignatureCaptureWindowViewModel : BaseNotification
    {
        public SignatureCaptureWindowViewModel()
        {
            InitialiseCommands();
            InitialseRepositories();
            Initialise();
        }

        public IRepository<User> Repository { get; set; }

        public User User { get; private set; }
        
        public DelegateCommand<Window> CloseCommand { get; set; }

        public DelegateCommand<Window> SaveCommand { get; set; }
        
        public DelegateCommand<object> BrowseCommand { get; set; }

        private void Initialise()
        {
            //Must ensure we always have the most up-to-date instance of the user object
            User = Repository.First(user => string.Equals(user.Username, UserManagement.SelectedUser.Username, StringComparison.CurrentCultureIgnoreCase));
        }

        private static void OnClose(Window window)
        {
            if (window == null)
                return;

            window.Close();
        }

        private void OnSave(Window window)
        {
            if (window == null)
                return;

            Repository.AddOrUpdate(User);
            Repository.Save();
            window.Close();
        }

        private void OnBrowse(object arg)
        {
            DialogHelperResult result = DialogHelper.OpenFile(DialogFilter.JPG, string.Empty);
            if (result.Result == true)
            {
                try
                {
                    User.Image = ImageHelper.LoadImageSafely(result.FileName);

                    try
                    {
                        string path = Signature.Transform(new Bitmap(User.Image));
                        User.Image = ImageHelper.LoadImageSafely(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.ERR_UNABLE_TO_APPLY_TRANSFORMATIONS, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
                    }
                }
                catch
                {
                    MessageBoxHelper.ShowError(Resources.ERR_UNABLE_TO_LOAD_THE_SPECIFIED_FILE);
                }
            }
        }
        
        private void InitialiseCommands()
        {
            BrowseCommand = new DelegateCommand<object>(OnBrowse);
            CloseCommand = new DelegateCommand<Window>(OnClose);
            SaveCommand = new DelegateCommand<Window>(OnSave);
        }

        private void InitialseRepositories()
        {
            Repository = ContainerBootstrapper.Container.GetInstance<IRepository<User>>();
        }
    }
}