using System.Collections.Concurrent;

namespace WebServerSimulation
{
    public class SimulatedWebServer : IWebServer
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

        private ConcurrentQueue<string> _requestQueue = new();

        public SimulatedWebServer(int numberOfThreads)
        {
            _threads = new Thread[numberOfThreads];
        }

        public void Run()
        {
            if (_status is not Status.NotRun or Status.Stopped)
            {
                throw new InvalidOperationException("Server is already running.");
            }

            _status = Status.Initializing;

            for (int i = 0; i < _threads.Length; ++i)
            {
                _threads[i] = new Thread(Listen);
                _threads[i].Start();
            }

            _status = Status.Running;
        }

        public void Stop(bool hard)
        {
            _status = Status.Stopping;

            if (hard)
            {
                _requestQueue.Clear();
            }

            foreach (Thread thread in _threads)
            {
                thread.Join();
            }

            _status = Status.Stopped;
        }

        public void AcceptRequest(string message)
        {
            if (_status is not Status.Running)
            {
                throw new InvalidOperationException("Server is not running");
            }

            _requestQueue.Enqueue(message);
        }

        private void Listen()
        {
            while (_status is not Status.Running)
            {
                Thread.Sleep(100);
            }

            while (_status is Status.Running || (_status is Status.Stopping && _requestQueue.Count > 0))
            {
                if (_requestQueue.TryDequeue(out string? message))
                {
                    ProcessRequest(message);
                }
            }
        }

        private void ProcessRequest(string message)
        {
            Thread.Sleep(2000); //simulate hard work
            Console.WriteLine($"Processed input: {message}; thread id: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
