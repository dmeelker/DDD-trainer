using KettleAndCask.Domain;

namespace KettleAndCask.Domain.Tests;

internal static class TestRecipes
{
    public static Recipe HopHarbourIpa() => new()
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

    public static Recipe HarbourDryStout() => new()
    {
        Name = "Harbour Dry Stout",
        Style = "Irish Stout",
        BatchSize = 25,
        Efficiency = 0.72,
        Malts =
        {
            new Malt { Name = "Maris Otter", Amount = 3.6, Potential = 313, Color = 3 },
            new Malt { Name = "Flaked Barley", Amount = 0.9, Potential = 267, Color = 2 },
            new Malt { Name = "Roasted Barley", Amount = 0.6, Potential = 209, Color = 300 },
        },
        Hops =
        {
            new Hop { Name = "East Kent Goldings", Amount = 55, Alpha = 5, Time = 60, Use = "boil" },
        },
        Yeast = new Yeast { Name = "Irish Ale", Lab = "Wyeast", Attenuation = 75 },
    };
}
