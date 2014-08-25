namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Data.Entity.Validation;
    using System.Windows;
    using Core;
    using Properties;
    using Shared;
    using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

    public class BaseRepository
    {
        public BaseRepository()
        {
            Context = new TachographContext();
        }
        
        protected TachographContext Context { get; set; }
        
        protected void Safely(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (DbEntityValidationException entityEx)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, entityEx)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException invalidEx)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNLOGGABLE_EXCEPTION, invalidEx.Message), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, entityEx)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException invalidEx)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNLOGGABLE_EXCEPTION, invalidEx.Message), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return default(T);
        }

        public void Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", Resources.EXC_UNABLE_SAVE_CHANGES, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        public void Dispose()
        {
            Context = null;
        }
    }
}