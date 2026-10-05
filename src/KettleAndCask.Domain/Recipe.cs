namespace KettleAndCask.Domain;

public class Recipe
{
    public string Name { get; set; } = "";
    public string Style { get; set; } = "";

    // liters
    public double BatchSize { get; set; }

    // 0.72 means 72%. Some older recipes were entered as 72.
    public double Efficiency { get; set; } = 0.72;

    // minutes
    public int BoilTime { get; set; } = 60;

    public List<Malt> Malts { get; set; } = new();
    public List<Hop> Hops { get; set; } = new();
    public Yeast? Yeast { get; set; }

    // Filled in by BrewCalculator.Calculate()
    public double OG;
    public double FG;
    public double IBU;
    public double SRM;
    public double ABV;
    public bool Calculated;
}
