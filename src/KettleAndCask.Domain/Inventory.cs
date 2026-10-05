namespace KettleAndCask.Domain;

public class Inventory
{
    // ingredient name -> amount on the shelf (malts in kg, hops in grams)
    public Dictionary<string, double> Stock { get; } = new();

    public void Add(string name, double amount)
    {
        Stock.TryGetValue(name, out var current);
        Stock[name] = current + amount;
    }

    // Takes everything the recipe needs off the shelf.
    public bool Use(Recipe r)
    {
        foreach (var m in r.Malts)
        {
            if (!Stock.ContainsKey(m.Name) || Stock[m.Name] < m.Amount)
                return false;
            Stock[m.Name] -= m.Amount;
        }
        foreach (var h in r.Hops)
        {
            if (!Stock.ContainsKey(h.Name) || Stock[h.Name] < h.Amount)
                return false;
            Stock[h.Name] -= h.Amount;
        }
        return true;
    }

    public List<string> Missing(Recipe r)
    {
        var result = new List<string>();
        foreach (var m in r.Malts)
        {
            Stock.TryGetValue(m.Name, out var have);
            if (have < m.Amount)
                result.Add(m.Name + ": need " + (m.Amount - have).ToString("0.00") + " more");
        }
        foreach (var h in r.Hops)
        {
            Stock.TryGetValue(h.Name, out var have);
            if (have < h.Amount)
                result.Add(h.Name + ": need " + (h.Amount - have).ToString("0") + " more");
        }
        return result;
    }
}
