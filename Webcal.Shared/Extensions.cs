namespace Webcal.Shared
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Connect;

    public static class Extensions
    {
        public static string DoubleEscape(this string input)
        {
            return input.Replace("\"", "'");
        }

        public static void CallAsync<TResult>(this IAsyncClient client, Func<IConnectClient, TResult> beginCall, Action<TResult> endCall, Action<Exception> exceptionHandler)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                TaskScheduler synchronizationContext = TaskScheduler.FromCurrentSynchronizationContext();

                Task.Factory.StartNew(() =>
                {
                    return beginCall((IConnectClient)client);
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
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mouse.OverrideCursor = null;
                        if (exceptionHandler != null)
                        {
                            BuildException(exceptionTask, exceptionHandler);
                        }
                    }, DispatcherPriority.Normal);

                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                exceptionHandler(ex);
            }
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
                builder.AppendLine(String.Format("{0}. {1}", i + 1, innerException.Message));
            }

            exceptionHandler(new Exception(string.Format("One or more errors occurred whilst performing the operation.  See below for details;\r\n\r\n{0}", builder)));
        }
    }
}