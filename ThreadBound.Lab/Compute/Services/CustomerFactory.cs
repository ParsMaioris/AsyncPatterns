using ThreadBound.Compute.Domain;

namespace ThreadBound.Compute.Services;

public static class CustomerFactory
{
    public static List<Customer> Build(int count)
    {
        var result = new List<Customer>();
        for (int i = 0; i < count; i++)
        {
            result.Add(new Customer(i, $"Customer_{i}", (i % 10) + 1));
        }
        return result;
    }
}
