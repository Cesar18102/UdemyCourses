namespace AlgoMaster.ConcurrencyPractice;

public class CountDownLatch {
    private int _count;
    private readonly object _lock = new();
    private readonly ManualResetEventSlim _manualResetEvent = new(false);

    public CountDownLatch(int initialCount) {
        _count = initialCount;
    }

    public void CountDown() {
        lock(_lock) {
            if (_count == 0) {
                return;
            }

            if(--_count == 0) {
                _manualResetEvent.Set();
            }
        }
    }

    public void Await() {
        _manualResetEvent.Wait();
    }

    public int GetCount() {
        lock(_lock) {
            return _count;
        }
    }
}
