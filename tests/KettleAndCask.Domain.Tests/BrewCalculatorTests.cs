using KettleAndCask.Domain;

namespace KettleAndCask.Domain.Tests;

// Characterization tests: they pin down the numbers the brewers rely on today,
// so you can refactor towards a supple design without changing the beer.
public class BrewCalculatorTests
{
    private readonly BrewCalculator _calc = new();

    [Fact]
    public void Calculates_the_vital_statistics_of_an_ipa()
    {
        var r = TestRecipes.HopHarbourIpa();

        _calc.Calculate(r);

        Assert.Equal(1.0607, r.OG, 4);
        Assert.Equal(1.0134, r.FG, 4);
        Assert.Equal(46.05, r.IBU, 2);
        Assert.Equal(8.43, r.SRM, 2);
        Assert.Equal(6.21, r.ABV, 2);
    }

    [Fact]
    public void Calculates_the_vital_statistics_of_a_stout()
    {
        var r = TestRecipes.HarbourDryStout();

        _calc.Calculate(r);

        Assert.Equal(1.0430, r.OG, 4);
        Assert.Equal(1.0107, r.FG, 4);
        Assert.Equal(27.02, r.IBU, 2);
        Assert.Equal(25.94, r.SRM, 2);
        Assert.Equal(4.23, r.ABV, 2);
    }

    [Fact]
    public void Brewhouse_efficiency_does_not_apply_to_sugar()
    {
        var r = new Recipe
        {
            BatchSize = 10,
            Efficiency = 0.5,
            Malts = { new Malt { Name = "Dextrose", Amount = 1, Potential = 384, Type = "sugar" } },
        };

        _calc.Calculate(r);

        Assert.Equal(1.0384, r.OG, 4);
    }

    [Fact]
    public void Dry_hops_add_no_bitterness()
    {
        var r = TestRecipes.HopHarbourIpa();
        _calc.Calculate(r);
        var before = r.IBU;

        r.Hops.Add(new Hop { Name = "Mosaic", Amount = 200, Alpha = 12, Use = "dry hop" });
        _calc.Calculate(r);

        Assert.Equal(before, r.IBU, 6);
    }

    [Fact]
    public void Efficiency_entered_as_a_percentage_is_understood()
    {
        var asFraction = TestRecipes.HopHarbourIpa();
        var asPercentage = TestRecipes.HopHarbourIpa();
        asPercentage.Efficiency = 72;

        _calc.Calculate(asFraction);
        _calc.Calculate(asPercentage);

        Assert.Equal(asFraction.OG, asPercentage.OG, 6);
    }

    [Fact]
    public void Scaling_a_recipe_keeps_its_vital_statistics()
    {
        var r = TestRecipes.HopHarbourIpa();
        _calc.Calculate(r);
        var (og, ibu, srm) = (r.OG, r.IBU, r.SRM);

        _calc.Adjust(r, 1200);
        _calc.Calculate(r);

        Assert.Equal(1200, r.BatchSize);
        Assert.Equal(264, r.Malts[0].Amount, 6);
        Assert.Equal(og, r.OG, 4);
        Assert.Equal(ibu, r.IBU, 1);
        Assert.Equal(srm, r.SRM, 2);
    }
}
