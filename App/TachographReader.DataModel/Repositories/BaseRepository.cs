using System;
using System.Data.Entity.Validation;
using System.Windows;
using Webcal.Shared;
using Webcal.DataModel.Properties;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Webcal.DataModel.Repositories
{
    public class BaseRepository
    {
        #region Constructor

        public BaseRepository()
        {
            Context = new TachographContext();
        }

        #endregion

        #region Protected Properties

        protected TachographContext Context { get; set; }

        #endregion

        #region Protected Methods

        protected void Safely(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (DbEntityValidationException entityEx)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(entityEx)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException invalidEx)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNLOGGABLE_EXCEPTION, invalidEx.Message), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ex)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected T Safely<T>(Func<T> action)
        {
            try
            {
                return action.Invoke();
            }
            catch (DbEntityValidationException entityEx)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(entityEx)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException invalidEx)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNLOGGABLE_EXCEPTION, invalidEx.Message), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ex)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return default(T);
        }

        #endregion

        #region Public Methods

        public void Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", Resources.EXC_UNABLE_SAVE_CHANGES, ExceptionPolicy.HandleException(ex)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            Context = null;
        }

        #endregion
    }
}