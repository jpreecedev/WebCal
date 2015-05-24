namespace TachographReader.Shared
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Connect;
    using Properties;

    public static class AsyncHelper
    {
        public static void CallAsync<TResult>(Func<TResult> beginCall, Action<TResult> endCall, Action<Exception> exceptionHandler, Action alwaysCall = null)
        {
            try
            {
                TaskScheduler synchronizationContext = TaskScheduler.FromCurrentSynchronizationContext();

                Task.Factory.StartNew(() =>
                {
                    return beginCall();
                })
                .ContinueWith(mainTask =>
                {
                    if (mainTask.IsFaulted)
                    {
                        BuildException(mainTask, exceptionHandler);
                    }
                    else
                    {
                        if (endCall != null)
                        {
                            endCall(mainTask.Result);
                        }
                    }
                }, synchronizationContext)
                .ContinueWith(exceptionTask => Application.Current.Dispatcher.Invoke(() =>
                {
                    BuildException((Task<ConnectOperationResult>)exceptionTask, exceptionHandler);

                }, DispatcherPriority.Normal), TaskContinuationOptions.OnlyOnFaulted)
                .ContinueWith(alwaysTask =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (alwaysCall != null)
                        {
                            alwaysCall();
                        }
                    }, DispatcherPriority.Normal);
                });
            }
            catch (Exception ex)
            {
                if (exceptionHandler != null)
                {
                    exceptionHandler(ex);
                }
            }
        }

        private static void BuildException<TResult>(Task<TResult> task, Action<Exception> exceptionHandler)
        {
            Exception exception = task.Exception;
            BuildException(exception, exceptionHandler);
        }

        private static void BuildException(Exception exception, Action<Exception> exceptionHandler)
        {
            if (exceptionHandler == null)
            {
                return;
            }

            var builder = new StringBuilder();

            var exceptionAsAggregate = exception as AggregateException;
            if (exceptionAsAggregate != null)
            {
                AggregateException aggregateException = exceptionAsAggregate.Flatten();

                for (int i = 0; i < aggregateException.InnerExceptions.Count; i++)
                {
                    Exception innerException = aggregateException.InnerExceptions[i];
                    builder.AppendLine(String.Format("{0}. {1}", i + 1, innerException.Message));
                }
            }
            else
            {
                builder.AppendLine(exception.Message);
            }

            exceptionHandler(new Exception(string.Format(Resources.TXT_ONE_OR_MORE_ERRORS, builder)));
        }
    }
}
