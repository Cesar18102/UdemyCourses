namespace AlgoMaster.ConcurrencyPractice; 

public class ConcurrencyLimiter : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public ConcurrencyLimiter(int maxConcurrent)
    {
        _semaphore = new SemaphoreSlim(maxConcurrent, maxConcurrent);
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }

    public void Run(Action task)
    {
        _semaphore.Wait();
        try
        {
            task();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}