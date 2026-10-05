namespace KettleAndCask.Domain;

// Ranges are approximations of the BJCP 2021 guidelines.
public class StyleChecker
{
    private readonly BrewCalculator _calc = new();

    public List<string> Check(Recipe r)
    {
        if (!r.Calculated)
            _calc.Calculate(r);

        var problems = new List<string>();

        switch (r.Style)
        {
            case "American IPA":
                if (r.OG < 1.056 || r.OG > 1.070) problems.Add("OG out of range");
                if (r.FG < 1.008 || r.FG > 1.014) problems.Add("FG out of range");
                if (r.IBU < 40 || r.IBU > 70) problems.Add("IBU out of range");
                if (r.SRM < 6 || r.SRM > 14) problems.Add("SRM out of range");
                if (r.ABV < 5.5 || r.ABV > 7.5) problems.Add("ABV out of range");
                break;
            case "American Pale Ale":
                if (r.OG < 1.045 || r.OG > 1.060) problems.Add("OG out of range");
                if (r.FG < 1.010 || r.FG > 1.015) problems.Add("FG out of range");
                if (r.IBU < 30 || r.IBU > 50) problems.Add("IBU out of range");
                if (r.SRM < 5 || r.SRM > 10) problems.Add("SRM out of range");
                if (r.ABV < 4.5 || r.ABV > 6.2) problems.Add("ABV out of range");
                break;
            case "German Pils":
                if (r.OG < 1.044 || r.OG > 1.050) problems.Add("OG out of range");
                if (r.FG < 1.008 || r.FG > 1.013) problems.Add("FG out of range");
                if (r.IBU < 22 || r.IBU > 40) problems.Add("IBU out of range");
                if (r.SRM < 2 || r.SRM > 4) problems.Add("SRM out of range");
                if (r.ABV < 4.4 || r.ABV > 5.2) problems.Add("ABV out of range");
                break;
            case "Irish Stout":
                if (r.OG < 1.036 || r.OG > 1.044) problems.Add("OG out of range");
                if (r.FG < 1.007 || r.FG > 1.011) problems.Add("FG out of range");
                if (r.IBU < 25 || r.IBU > 45) problems.Add("IBU out of range");
                if (r.SRM < 25 || r.SRM > 40) problems.Add("SRM out of range");
                if (r.ABV < 4.0 || r.ABV > 4.5) problems.Add("ABV out of range");
                break;
            default:
                problems.Add("Unknown style: " + r.Style);
                break;
        }

        return problems;
    }
}
