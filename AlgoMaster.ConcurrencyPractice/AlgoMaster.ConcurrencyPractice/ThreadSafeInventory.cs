namespace AlgoMaster.ConcurrencyPractice;

public class ThreadSafeInventory {
    private int _stock;
    private readonly object _lock = new();

    public ThreadSafeInventory(int initialStock) {
        _stock = initialStock;
    }

    public bool TryPurchase(int quantity, Action onPurchased)
    {
        lock (_lock)
        {
            if (quantity > _stock)
            {
                return false;
            }

            _stock -= quantity;
        }

        onPurchased();
        return true;
    }

    public void Restock(int quantity)
    {
        lock (_lock)
        {
            _stock += quantity;
        }
    }

    public int Available()
    {
        lock (_lock) 
        {
            return _stock;
        }
    }
}