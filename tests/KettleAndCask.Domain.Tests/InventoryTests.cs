using KettleAndCask.Domain;

namespace KettleAndCask.Domain.Tests;

public class InventoryTests
{
    private static Inventory FullyStocked()
    {
        var inv = new Inventory();
        inv.Add("Maris Otter", 25);
        inv.Add("Flaked Barley", 5);
        inv.Add("Roasted Barley", 2);
        inv.Add("East Kent Goldings", 500);
        return inv;
    }

    [Fact]
    public void Nothing_is_missing_when_the_shelf_is_stocked()
    {
        Assert.Empty(FullyStocked().Missing(TestRecipes.HarbourDryStout()));
    }

    [Fact]
    public void Reports_the_shortfall_for_each_ingredient()
    {
        var inv = FullyStocked();
        inv.Stock["Roasted Barley"] = 0.5;

        var missing = inv.Missing(TestRecipes.HarbourDryStout());

        Assert.Equal(new[] { "Roasted Barley: need 0.10 more" }, missing.Select(m => m.Replace(',', '.')));
    }

    [Fact]
    public void Using_a_recipe_takes_its_ingredients_off_the_shelf()
    {
        var inv = FullyStocked();

        Assert.True(inv.Use(TestRecipes.HarbourDryStout()));

        Assert.Equal(21.4, inv.Stock["Maris Otter"], 6);
        Assert.Equal(445, inv.Stock["East Kent Goldings"], 6);
    }
}
