namespace TachographReader.DataModel.Repositories
{
    using System;
    using System.Data.Entity.Validation;
    using Core;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public class BaseRepository
    {
        protected void Safely(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (DbEntityValidationException entityEx)
            {
                MessageBoxHelper.ShowError(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, entityEx)));
            }
            catch (InvalidOperationException invalidEx)
            {
                MessageBoxHelper.ShowError(string.Format(Resources.EXC_UNLOGGABLE_EXCEPTION, invalidEx.Message));
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
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
                MessageBoxHelper.ShowError(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, entityEx)));
            }
            catch (InvalidOperationException invalidEx)
            {
                MessageBoxHelper.ShowError(string.Format(Resources.EXC_UNLOGGABLE_EXCEPTION, invalidEx.Message));
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(string.Format(Resources.EXC_UNABLE_TO_COMPLETE_REQUEST, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
            }

            return default(T);
        }
    }
}