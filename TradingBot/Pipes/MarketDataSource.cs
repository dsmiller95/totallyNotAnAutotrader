using TradingBot.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TradingBot.Pipes
{
    public abstract class MarketDataSource : IMarketSubject
    {
        private List<IObserver<MarketUpdate>> _observers;
        private readonly CancellationTokenSource cancellationSource;

        public MarketDataSource()
        {
            _observers = new List<IObserver<MarketUpdate>>();
            cancellationSource = new CancellationTokenSource();
            Task.Run(async () =>
                {
                    await this.PollSource();
                },
                cancellationSource.Token
            );
        }

        /// <summary>
        /// A utility method used to push updated information to the observers
        /// </summary>
        /// <param name="update">The updated data snippit</param>
        protected void NotifyObservers(MarketUpdate update)
        {
            if (_observers != null)
            {
                _observers.ForEach((observer) => observer.OnNext(update));
            }
        }

        /// <summary>
        /// A long-running task which should be set up to long-poll the data source
        ///     and push any updated values to the NotifyObservers method
        /// </summary>
        /// <returns></returns>
        protected abstract Task PollSource();

        public IDisposable Subscribe(IObserver<MarketUpdate> observer)
        {
            lock (_observers)
            {
                _observers.Add(observer);
            }
            return new Subscription(this, observer);
        }

        private sealed class Subscription : IDisposable
        {
            private readonly MarketDataSource _subject;
            private IObserver<MarketUpdate> _observer;

            public Subscription(MarketDataSource subject, IObserver<MarketUpdate> observer)
            {
                _subject = subject;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                {
                    lock (_subject._observers)
                    {
                        _subject._observers.Remove(_observer);
                    }
                    _observer = null;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationSource.Cancel();
                }
                _observers = null;
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
