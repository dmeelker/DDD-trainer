namespace KettleAndCask.Domain;

// A quantity of actual beer sitting in a fermenter, tank or barrel.
public class Batch
{
    public string Name { get; set; } = "";

    // liters
    public double Volume { get; set; }

    // measured specific gravity right now
    public double Gravity { get; set; }

    // percent
    public double Abv { get; set; }

    public void BlendIn(Batch other)
    {
        double total = Volume + other.Volume;
        Gravity = (Gravity * Volume + other.Gravity * other.Volume) / total;
        Abv = (Abv * Volume + other.Abv * other.Volume) / total;
        Volume = total;
        Name = Name + " / " + other.Name;
        other.Volume = 0;
    }
}
