using System.Collections.Concurrent;

namespace AlgoMaster.ConcurrencyPractice;

public class VersionedSignal
{
    private int _version = 0;
    private readonly object _lock = new();
    private ConcurrentDictionary<int, ManualResetEventSlim> _events = new();

    public VersionedSignal() { }

    public int CurrentVersion()
    {
        lock (_lock)
        {
            return _version;
        }
    }

    public int AwaitNext(int observedVersion)
    {
        int currentVersion = CurrentVersion();
        if (currentVersion > observedVersion)
        {
            return currentVersion;
        }

        GetEvent(observedVersion).Wait();
        return CurrentVersion();
    }

    public void Signal()
    {
        int version;
        lock (_lock)
        {
            version = _version++;
        }
        GetEvent(version).Set();
    }

    private ManualResetEventSlim GetEvent(int version)
    {
        return _events.GetOrAdd(version, v => new ManualResetEventSlim(false));
    }
}