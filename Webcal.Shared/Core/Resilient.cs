namespace Webcal.Shared.Core
{
    using System;
    using System.Threading;
    using Properties;

    public class Resilient
    {
        private const double DEFAULT_EXPONENTIAL_BASE = 2;
        private const double DEFAULT_RANDOM_FACTOR = 1.1;
        private const int MAX_RETRY_COUNT = 5;
        private static readonly TimeSpan DefaultCoefficient = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(60);
        private static readonly Random _random = new Random();
        private static int _attempts;
        private readonly Action<string> _progress;

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
                    if (_progress != null)
                    {
                        _progress(Resources.TXT_STARTING);
                    }

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
                    if (_progress != null)
                    {
                        _progress(Resources.TXT_STARTING);
                    }

                    T result = operation();
                    return result;
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
                double delta = (Math.Pow(DEFAULT_EXPONENTIAL_BASE, _attempts) - 1.0)*(1.0 + _random.NextDouble()*(DEFAULT_RANDOM_FACTOR - 1.0));
                double delay = Math.Min(DefaultCoefficient.TotalMilliseconds*delta, MaxDelay.TotalMilliseconds);

                var actualDelay = TimeSpan.FromMilliseconds(delay);

                if (_progress != null)
                {
                    _progress(string.Format(Resources.TXT_ATTEMPT_FAILED, actualDelay.Seconds));
                }

                return actualDelay;
            }

            if (_progress != null)
            {
                _progress(Resources.TXT_FAILED);
            }

            return null;
        }
    }
}