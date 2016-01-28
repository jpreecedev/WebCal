namespace TachographReader.Shared.Core
{
    using System;
    using System.Threading;
    using Properties;

    public class Resilient
    {
        private const double DEFAULT_EXPONENTIAL_BASE = 2;
        private const double DEFAULT_RANDOM_FACTOR = 1.1;
        private const int MAX_RETRY_COUNT = 5;
        private readonly TimeSpan _defaultCoefficient = TimeSpan.FromSeconds(1);
        private readonly TimeSpan _maxDelay = TimeSpan.FromSeconds(60);
        private readonly Random _random = new Random();
        private int _attempts;
        private readonly Action<string> _progress;

        public Resilient()
        {

        }

        public Resilient(Action<string> progress)
        {
            _progress = progress;
        }

        public void ExecuteWithRetry(Action operation)
        {
            while (true)
            {
                TimeSpan? delay;
                try
                {
                    ReportProgress(Resources.TXT_STARTING);
                    operation.Invoke();
                    return;
                }
                catch (Exception ex)
                {
                    delay = GetNextDelay();
                    if (delay == null)
                    {
                        throw new MaxAttemptsReachedException(Resources.TXT_MAX_RETRIES_REACHED, ex);
                    }
                }

                Thread.Sleep(delay.Value);
            }
        }

        public T ExecuteWithRetry<T>(Func<T> operation)
        {
            while (true)
            {
                TimeSpan? delay;
                try
                {
                    ReportProgress(Resources.TXT_STARTING);
                    return operation();
                }
                catch (Exception ex)
                {
                    delay = GetNextDelay();
                    if (delay == null)
                    {
                        throw new MaxAttemptsReachedException(Resources.TXT_MAX_RETRIES_REACHED, ex);
                    }
                }

                Thread.Sleep(delay.Value);
            }
        }

        private TimeSpan? GetNextDelay()
        {
            _attempts += 1;
            if (_attempts < MAX_RETRY_COUNT)
            {
                var delta = (Math.Pow(DEFAULT_EXPONENTIAL_BASE, _attempts) - 1.0) * (1.0 + _random.NextDouble() * (DEFAULT_RANDOM_FACTOR - 1.0));
                var delay = Math.Min(_defaultCoefficient.TotalMilliseconds * delta, _maxDelay.TotalMilliseconds);

                var actualDelay = TimeSpan.FromMilliseconds(delay);
                ReportProgress(string.Format(Resources.TXT_ATTEMPT_FAILED, actualDelay.Seconds));
                
                return actualDelay;
            }

            ReportProgress(Resources.TXT_FAILED);

            return null;
        }

        private void ReportProgress(string progress)
        {
            if (_progress != null && !string.IsNullOrEmpty(progress))
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _progress(progress);
                });
            }
        }
    }
}