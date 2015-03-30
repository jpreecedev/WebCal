﻿namespace TachographReader.Shared
{
    using System;
    using System.ServiceModel;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Connect;
    using global::Connect.Shared;
    using Properties;

    public static class Extensions
    {
        public static string DoubleEscape(this string input)
        {
            return input.Replace("\"", "'");
        }
        
        public static void CallAsync<TResult>(this IConnectClient client, IConnectKeys connectKeys, Func<IConnectClient, TResult> beginCall, Action<ConnectOperationResult> endCall = null, Action<Exception> exceptionHandler = null, Action alwaysCall = null)
        {
            if (connectKeys == null)
            {
                if (alwaysCall != null)
                {
                    alwaysCall();
                }
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                TaskScheduler synchronizationContext = TaskScheduler.FromCurrentSynchronizationContext();

                Task.Factory.StartNew(() =>
                {
                    client.Open(connectKeys);
                    return Try(() => beginCall(client));
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
                        Mouse.OverrideCursor = null;
                        if (alwaysCall != null)
                        {
                            alwaysCall();
                        }
                    }, DispatcherPriority.Normal);
                });
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                if (exceptionHandler != null)
                {
                    exceptionHandler(ex);
                }
            }
        }

        public static void CallAsync(this IConnectClient client, IConnectKeys connectKeys, Action<IConnectClient> beginCall, Action<ConnectOperationResult> endCall = null, Action<Exception> exceptionHandler = null, Action alwaysCall = null)
        {
            if (connectKeys == null)
            {
                if (alwaysCall != null)
                {
                    alwaysCall();
                }
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                TaskScheduler synchronizationContext = TaskScheduler.FromCurrentSynchronizationContext();

                Task.Factory.StartNew(() =>
                {
                    client.Open(connectKeys);
                    return Try(() => beginCall(client));
                })
                .ContinueWith(mainTask =>
                {
                    if (mainTask.IsFaulted)
                    {
                        BuildException(mainTask, exceptionHandler);
                    }
                    else if (!mainTask.Result.IsSuccess)
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
                .ContinueWith(alwaysTask => Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                    if (alwaysCall != null)
                    {
                        alwaysCall();
                    }
                }, DispatcherPriority.Normal));
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;

                if (exceptionHandler != null)
                {
                    exceptionHandler(ex);
                }
            }
        }

        private static ConnectOperationResult Try(Action action)
        {
            try
            {
                action();
                return new ConnectOperationResult();
            }
            catch (CommunicationException communicationException)
            {
                FaultException fe = null;
                Exception tmp = communicationException;

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
                    return new ConnectOperationResult(fe, string.Format(Resources.TXT_SERVER_SENT_A_FAULT, fe.CreateMessageFault().Reason.GetMatchingTranslation().Text));
                }

                return new ConnectOperationResult(communicationException, string.Format(Resources.TXT_REQUEST_FAIlED_WITH_EXCEPTION, communicationException));
            }
            catch (TimeoutException timeoutException)
            {
                return new ConnectOperationResult(timeoutException, Resources.TXT_REQUEST_TIMED_OUT);
            }
            catch (Exception ex)
            {
                return new ConnectOperationResult(ex, string.Format(Resources.TXT_REQUEST_FAILED_WITH_UNEXPECTED_EXCEPTION, ex));
            }
        }

        private static ConnectOperationResult Try<TResult>(Func<TResult> func)
        {
            try
            {
                return new ConnectOperationResult(func());
            }
            catch (CommunicationException communicationException)
            {
                FaultException fe = null;
                Exception tmp = communicationException;

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
                    return new ConnectOperationResult(fe, string.Format(Resources.TXT_SERVER_SENT_A_FAULT, fe.CreateMessageFault().Reason.GetMatchingTranslation().Text));
                }

                return new ConnectOperationResult(communicationException, string.Format(Resources.TXT_REQUEST_FAIlED_WITH_EXCEPTION, communicationException));
            }
            catch (TimeoutException timeoutException)
            {
                return new ConnectOperationResult(timeoutException, Resources.TXT_REQUEST_TIMED_OUT);
            }
            catch (Exception ex)
            {
                return new ConnectOperationResult(ex, string.Format(Resources.TXT_REQUEST_FAILED_WITH_UNEXPECTED_EXCEPTION, ex));
            }
        }

        private static void BuildException(Task<ConnectOperationResult> task, Action<Exception> exceptionHandler)
        {
            Exception exception = task.Exception ?? task.Result.Exception;
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