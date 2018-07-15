using TradingBot.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TradingBot.Pipes
{
    public class FakeMarketSubject : IMarketSubject
    {
        private List<IObserver<MarketUpdate>> _observers;
        private readonly CancellationTokenSource cancellationSource;

        public FakeMarketSubject()
        {
            _observers = new List<IObserver<MarketUpdate>>();
            cancellationSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    var update = new MarketUpdate(8, DateTime.UtcNow);
                    if (_observers != null)
                    {
                        _observers.ForEach((observer) => observer.OnNext(update));
                    }
                }
            },
            cancellationSource.Token);
        }

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
            private readonly FakeMarketSubject _subject;
            private IObserver<MarketUpdate> _observer;

            public Subscription(FakeMarketSubject subject, IObserver<MarketUpdate> observer)
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
