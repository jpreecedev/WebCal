using System;
using System.Drawing;
using System.Windows;
using StructureMap;
using Webcal.Shared;
using Webcal.Core;
using Webcal.DataModel.Library;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Properties;
using BaseNotification = Webcal.Core.BaseNotification;

namespace Webcal.Windows.SignatureCaptureWindow
{
    public class SignatureCaptureWindowViewModel : BaseNotification
    {
        #region Constructor

        public SignatureCaptureWindowViewModel()
        {
            InitialiseCommands();
            InitialseRepositories();
            Initialise();
        }

        private void Initialise()
        {
            //Must ensure we always have the most up-to-date instance of the user object
            User = Repository.First(user => string.Equals(user.Username, UserManagement.SelectedUser.Username, StringComparison.CurrentCultureIgnoreCase));
        }

        #endregion

        #region Public Properties

        public IRepository<User> Repository { get; set; }

        public User User { get; private set; }
        
        #endregion

        #region Commands

        #region Command : Close

        public DelegateCommand<Window> CloseCommand { get; set; }

        private static void OnClose(Window window)
        {
            if (window == null)
                return;

            window.Close();
        }

        #endregion

        #region Command : Save

        public DelegateCommand<Window> SaveCommand { get; set; }

        private void OnSave(Window window)
        {
            if (window == null)
                return;

            Repository.AddOrUpdate(User);
            Repository.Save();
            window.Close();
        }

        #endregion

        #region Command : Browse

        public DelegateCommand<object> BrowseCommand { get; set; }

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
                        string path = Imaging.Signature.Transform(new Bitmap(User.Image));
                        User.Image = ImageHelper.LoadImageSafely(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.ERR_UNABLE_TO_APPLY_TRANSFORMATIONS, ExceptionPolicy.HandleException(ex)));
                    }
                }
                catch
                {
                    MessageBoxHelper.ShowError(Resources.ERR_UNABLE_TO_LOAD_THE_SPECIFIED_FILE);
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        private void InitialiseCommands()
        {
            BrowseCommand = new DelegateCommand<object>(OnBrowse);
            CloseCommand = new DelegateCommand<Window>(OnClose);
            SaveCommand = new DelegateCommand<Window>(OnSave);
        }

        private void InitialseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IRepository<User>>();
        }

        #endregion
    }
}
