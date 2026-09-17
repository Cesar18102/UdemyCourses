using System.Collections.Concurrent;

namespace WebServerSimulation
{
    public class SimulatedWebServerV2 : IWebServer
    {
        private enum Status
        {
            NotRun,
            Initializing,
            Running,
            Stopping,
            Stopped
        }

        private Status _status = Status.NotRun;

        private Thread[] _threads;
        private BlockingCollection<string> _requestQueue;

        private readonly object _lockContext;
        private CancellationTokenSource _cancellationTokenSource;

        public SimulatedWebServerV2(int numberOfThreads)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _lockContext = new object();

            _requestQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());
            _threads = new Thread[numberOfThreads];
        }

        public void Run()
        {
            lock(_lockContext)
            {
                if (_status is not Status.NotRun)
                {
                    throw new InvalidOperationException("Server is already running.");
                }

                _status = Status.Initializing;
            }

            for (int i = 0; i < _threads.Length; ++i)
            {
                _threads[i] = new Thread(() => WorkerLoop(_cancellationTokenSource.Token));
                _threads[i].Start();
            }

            lock (_lockContext)
            {
                if (_status is not Status.Initializing)
                {
                    throw new InvalidOperationException("Server failed to start.");
                }

                _status = Status.Running;
            }
        }

        public void Stop(bool hard)
        {
            lock(_lockContext)
            {
                if (_status is not Status.Running)
                {
                    throw new InvalidOperationException("Server failed to start stopping.");
                }

                _status = Status.Stopping;
                _requestQueue.CompleteAdding();
            }

            if (hard)
            {
                _cancellationTokenSource.Cancel();
                foreach (string message in _requestQueue.GetConsumingEnumerable()) { }
            }

            foreach (Thread thread in _threads)
            {
                thread.Join();
            }

            lock (_lockContext)
            {
                if (_status is not Status.Stopping)
                {
                    throw new InvalidOperationException("Server failed to stop.");
                }
                _status = Status.Stopped;
            }
        }

        public void AcceptRequest(string message)
        {
            lock (_lockContext)
            {
                if (_status is not Status.Running || _requestQueue.IsAddingCompleted)
                {
                    throw new InvalidOperationException("Server is not running");
                }

                _requestQueue.Add(message);
            }
        }

        private void WorkerLoop(CancellationToken token)
        {
            try
            {
                foreach (string message in _requestQueue.GetConsumingEnumerable(token))
                {
                    ProcessRequest(message);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void ProcessRequest(string message)
        {
            Thread.Sleep(2000); //simulate hard work
            Console.WriteLine($"Processed input: {message}; thread id: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
