using KettleAndCask.Domain;

namespace KettleAndCask.Domain.Tests;

public class StyleCheckerTests
{
    private readonly StyleChecker _checker = new();

    [Fact]
    public void An_ipa_brewed_to_style_has_no_problems()
    {
        Assert.Empty(_checker.Check(TestRecipes.HopHarbourIpa()));
    }

    [Fact]
    public void A_stout_brewed_to_style_has_no_problems()
    {
        Assert.Empty(_checker.Check(TestRecipes.HarbourDryStout()));
    }

    [Fact]
    public void An_ipa_entered_as_a_pils_is_out_of_style_on_every_statistic()
    {
        var r = TestRecipes.HopHarbourIpa();
        r.Style = "German Pils";

        var problems = _checker.Check(r);

        Assert.Equal(5, problems.Count);
    }

    [Fact]
    public void An_unknown_style_is_reported()
    {
        var r = TestRecipes.HopHarbourIpa();
        r.Style = "Hazy Lager";

        Assert.Contains("Unknown style: Hazy Lager", _checker.Check(r));
    }
}
