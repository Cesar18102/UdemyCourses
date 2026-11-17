public class BankAccount {
    private long _balance;
    private readonly ReaderWriterLockSlim _lock = new();

    public BankAccount(long initialBalance) {
        _balance = initialBalance;
    }

    public void Deposit(long amount) {
        try {
            _lock.EnterWriteLock();
            _balance += amount;
        }
        finally {
            _lock.ExitWriteLock();
        }
    }

    public bool Withdraw(long amount) {
        try {
            _lock.EnterWriteLock();
            if (_balance >= amount) {
                _balance -= amount;
                return true;
            }
            return false;
        }
        finally {
            _lock.ExitWriteLock();
        }
    }

    public long GetBalance() {
        try {
            _lock.EnterReadLock();
            return _balance;
        }
        finally {
            _lock.ExitReadLock();
        }
    }
}

public class BankAccountNaive {
    private long _balance;
    private readonly object _lock = new();

    public BankAccountNaive(long initialBalance) {
        _balance = initialBalance;
    }

    public void Deposit(long amount) {
        lock (_lock) {
            _balance += amount;
        }
    }

    public bool Withdraw(long amount) {
        lock (_lock) {
            if (_balance >= amount) {
                _balance -= amount;
                return true;
            }
            return false;
        }
    }

    public long GetBalance() {
        lock (_lock) {
            return _balance;
        }
    }
}

public class BankAccountInterlocked
{
    private long _balance;

    public BankAccountInterlocked(long initialBalance) {
        _balance = initialBalance;
    }

    public void Deposit(long amount) {
        while (true)
        {
            long balance = Interlocked.Read(ref _balance);
            if (Interlocked.CompareExchange(ref _balance, balance + amount, balance) == balance)
            {
                break;
            }
        }
    }

    public bool Withdraw(long amount)
    {
        while (true)
        {
            long balance = Interlocked.Read(ref _balance);

            if (balance < amount)
            {
                return false;
            }

            if (Interlocked.CompareExchange(ref _balance, balance - amount, balance) == balance)
            {
                break;
            }
        }

        return true;
    }

    public long GetBalance()
    {
        return Interlocked.Read(ref _balance);
    }
}