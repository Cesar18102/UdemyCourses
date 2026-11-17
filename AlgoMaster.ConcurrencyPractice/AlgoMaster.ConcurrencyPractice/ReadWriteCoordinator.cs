namespace AlgoMaster.ConcurrencyPractice;

public class ReadWriteCoordinator {
    private readonly ReaderWriterLockSlim _readerWriterLock = new();

    public ReadWriteCoordinator() {

    }

    public void Read(Action readTask)
    {
        _readerWriterLock.EnterReadLock();
        try
        {
            readTask();
        }
        finally
        {
            _readerWriterLock.ExitReadLock();
        }
    }

    public void Write(Action writeTask)
    {
        _readerWriterLock.EnterWriteLock();
        try
        {
            writeTask();
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }
    }
}