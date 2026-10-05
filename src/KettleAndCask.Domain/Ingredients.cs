namespace KettleAndCask.Domain;

public class Malt
{
    public string Name { get; set; } = "";

    // kilograms
    public double Amount { get; set; }

    // gravity points per kg per liter (2-row pale malt is about 309)
    public double Potential { get; set; }

    // degrees Lovibond
    public double Color { get; set; }

    // "grain", "sugar" or "extract"
    public string Type { get; set; } = "grain";
}

public class Hop
{
    public string Name { get; set; } = "";

    // grams
    public double Amount { get; set; }

    // percent, e.g. 12.5
    public double Alpha { get; set; }

    // minutes (boil time remaining, or whirlpool steep time)
    public int Time { get; set; }

    // "boil", "whirlpool" or "dry hop"
    public string Use { get; set; } = "boil";
}

public class Yeast
{
    public string Name { get; set; } = "";
    public string Lab { get; set; } = "";

    // percent, e.g. 78
    public double Attenuation { get; set; }
}
