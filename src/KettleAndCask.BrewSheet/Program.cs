using KettleAndCask.Domain;

var recipe = new Recipe
{
    Name = "Hop Harbour IPA",
    Style = "American IPA",
    BatchSize = 25,
    Efficiency = 0.72,
    BoilTime = 60,
    Malts =
    {
        new Malt { Name = "Pale Malt (2-row)", Amount = 5.5, Potential = 309, Color = 2 },
        new Malt { Name = "Munich Malt", Amount = 0.6, Potential = 292, Color = 9 },
        new Malt { Name = "Crystal 60", Amount = 0.35, Potential = 284, Color = 60 },
        new Malt { Name = "Dextrose", Amount = 0.25, Potential = 384, Color = 0, Type = "sugar" },
    },
    Hops =
    {
        new Hop { Name = "Citra", Amount = 60, Alpha = 12, Time = 0, Use = "dry hop" },
        new Hop { Name = "Magnum", Amount = 20, Alpha = 14, Time = 60, Use = "boil" },
        new Hop { Name = "Citra", Amount = 40, Alpha = 12, Time = 20, Use = "whirlpool" },
        new Hop { Name = "Centennial", Amount = 25, Alpha = 10, Time = 15, Use = "boil" },
    },
    Yeast = new Yeast { Name = "US-05", Lab = "Fermentis", Attenuation = 78 },
};

var calc = new BrewCalculator();
var checker = new StyleChecker();

calc.Calculate(recipe);
Print(recipe);
PrintStyle(checker.Check(recipe));

Console.WriteLine();
Console.WriteLine("=== Scaling up from the 25 L pilot kit to the 1200 L brewhouse ===");
calc.Adjust(recipe, 1200);
calc.Calculate(recipe);
Print(recipe);
PrintStyle(checker.Check(recipe));

Console.WriteLine();
Console.WriteLine("=== Checking the shelf ===");
var inventory = new Inventory();
inventory.Add("Pale Malt (2-row)", 300);
inventory.Add("Munich Malt", 25);
inventory.Add("Crystal 60", 20);
inventory.Add("Dextrose", 10);
inventory.Add("Magnum", 1000);
inventory.Add("Centennial", 1000);
inventory.Add("Citra", 3000);

foreach (var line in inventory.Missing(recipe))
    Console.WriteLine("  MISSING " + line);
Console.WriteLine(inventory.Use(recipe) ? "  Ingredients pulled for brew day." : "  Could not pull ingredients!");

static void Print(Recipe r)
{
    Console.WriteLine($"{r.Name} ({r.Style}) - {r.BatchSize} L");
    Console.WriteLine($"  OG {r.OG:0.000}  FG {r.FG:0.000}  ABV {r.ABV:0.0}%  IBU {r.IBU:0}  SRM {r.SRM:0.0}");
    foreach (var m in r.Malts)
        Console.WriteLine($"  {m.Amount,8:0.00} kg  {m.Name}");
    foreach (var h in r.Hops)
        Console.WriteLine($"  {h.Amount,8:0} g   {h.Name} ({h.Use}, {h.Time} min)");
}

static void PrintStyle(List<string> problems)
{
    if (problems.Count == 0)
        Console.WriteLine("  Within style guidelines.");
    foreach (var p in problems)
        Console.WriteLine("  STYLE: " + p);
}
