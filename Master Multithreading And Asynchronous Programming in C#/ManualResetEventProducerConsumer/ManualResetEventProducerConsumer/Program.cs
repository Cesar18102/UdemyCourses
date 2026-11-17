using System.Collections.Concurrent;

public class FeedingStation
{
    private readonly Random _rand = new();

    private readonly int _capacity;
    private ConcurrentQueue<int> _food = new ConcurrentQueue<int>();
    public ManualResetEventSlim FullSignal { get; private set; } = new ManualResetEventSlim(false);
    public ManualResetEventSlim EmptySignal { get; private set; } = new ManualResetEventSlim(true);

    public FeedingStation(int capacity)
    {
        _capacity = capacity;
    }

    public void Fill()
    {
        for (int i = 0; i < _capacity; ++i)
        {
            _food.Enqueue(i + 1);

            Console.WriteLine($"Added to feeding station: {i + 1}");
            Thread.Sleep(100);
        }

        EmptySignal.Reset();
        FullSignal.Set();
    }

    public bool TryTakeFood(out int food)
    {
        if (_food.TryDequeue(out food))
        {
            return true;
        }

        if (_food.Count == 0)
        {
            FullSignal.Reset();
            EmptySignal.Set();
        }

        return false;
    }
}

public class Farmer
{
    private readonly List<Animal> _animals = new List<Animal>();
    private readonly FeedingStation _feedingStation;

    public Farmer(FeedingStation feedingStation)
    {
        _feedingStation = feedingStation;
    }
    
    public void BuyAnimal()
    {
        _animals.Add(new Animal(_animals.Count + 1, _feedingStation));
    }

    public void DoBusiness()
    {
        while (true)
        {
            _feedingStation.EmptySignal.Wait();

            Console.WriteLine("Press P to produce some food...");

            if (Console.ReadLine().ToLower() == "p")
            {
                _feedingStation.Fill();
            }
        }
    }
}

public class Animal
{
    private readonly string _name;
    private Thread _life;
    private FeedingStation _feedingStation;

    public Animal(int index, FeedingStation feedingStation)
    {
        _name = $"Animal #{index}";
        _feedingStation = feedingStation;

        _life = new Thread(Live);
        _life.Start();
    }

    private void Live() 
    {
        while (true)
        {
            _feedingStation.FullSignal.Wait();
            if (_feedingStation.TryTakeFood(out int food))
            {
                Console.WriteLine($"{_name} eats {food}");
            }
            Thread.Sleep(100);
        }
    }
}

public static class Program
{
    public static void Main()
    {
        Console.Write("Enter number of animals at the farm: ");
        int animalsCount = int.Parse(Console.ReadLine());

        Console.Write("Enter desired food batch size: ");
        int foodBatchSize = int.Parse(Console.ReadLine());

        FeedingStation feedingStation = new FeedingStation(foodBatchSize);
        Farmer farmer = new Farmer(feedingStation);

        for (int i = 0; i < animalsCount; ++i)
        {
            farmer.BuyAnimal();
        }

        farmer.DoBusiness();
    }
}