namespace ThreadBound.Compute.Domain;

public class Customer
{
    private readonly int _id;
    private readonly string _name;
    private readonly int _value;
    private long _computedScore;

    public Customer(int id, string name, int value)
    {
        _id = id;
        _name = name;
        _value = value;
    }

    public int Id => _id;
    public string Name => _name;
    public int Value => _value;

    public long ComputedScore
    {
        get => _computedScore;
        set => _computedScore = value;
    }

    public void CalculateScore(Func<int, long> scoringFunction)
    {
        _computedScore = scoringFunction(_value);
    }
}

