namespace Webcal.Connect
{
    using System;
    using System.ServiceModel;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Webcal.Shared.Connect;

    public class BaseConnectClient
    {
        public void CallAsync<TResult>(Func<IConnectClient, TResult> beginCall, Action<TResult> endCall, Action<Exception> exceptionHandler)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                TaskScheduler synchronizationContext = TaskScheduler.FromCurrentSynchronizationContext();

                Task.Factory.StartNew(() =>
                {
                    return beginCall((IConnectClient) this);
                })
                .ContinueWith(mainTask =>
                {
                    if (mainTask.IsFaulted)
                    {
                        BuildException(mainTask, exceptionHandler);
                    }
                    else
                    {
                        endCall(mainTask.Result);
                    }
                    Mouse.OverrideCursor = null;  
              
                }, synchronizationContext)
                .ContinueWith(exceptionTask =>
                {
                    if (exceptionHandler != null)
                    {
                        BuildException(exceptionTask, exceptionHandler);
                    }
                    Mouse.OverrideCursor = null;

                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                exceptionHandler(ex);
            }
        }

        protected IConnectOperationResult Try(Action action)
        {
            try
            {
                action();
            }
            catch (CommunicationException e)
            {
                ForceClose();

                FaultException fe = null;
                Exception tmp = e;

                while (tmp != null)
                {
                    fe = tmp as FaultException;
                    if (fe != null)
                    {
                        break;
                    }
                    tmp = tmp.InnerException;
                }

                if (fe != null)
                {
                    return new FailureOperationResult(string.Format("The server sent back a fault: {0}", fe.CreateMessageFault().Reason.GetMatchingTranslation().Text));
                }

                return new FailureOperationResult(string.Format("The request failed with exception: {0}", e));
            }
            catch (TimeoutException)
            {
                ForceClose();
                return new FailureOperationResult("The request timed out");
            }
            catch (Exception e)
            {
                ForceClose();
                return new FailureOperationResult(string.Format("The request failed with unexpected exception: {0}", e));
            }

            return new SuccessOperationResult();
        }

        public virtual void ForceClose()
        {
        }

        private static void BuildException(Task task, Action<Exception> exceptionHandler)
        {
            if (task.Exception == null)
                return;

            AggregateException aggregateException = task.Exception.Flatten();
            var builder = new StringBuilder();

            for (int i = 0; i < aggregateException.InnerExceptions.Count; i++)
            {
                Exception innerException = aggregateException.InnerExceptions[i];
                builder.AppendLine(string.Format("{0}. {1}", i + 1, innerException.Message));
            }

            exceptionHandler(new Exception(string.Format("One or more errors occurred whilst performing the operation.  See below for details;\r\n\r\n{0}", builder)));
        }
    }
}