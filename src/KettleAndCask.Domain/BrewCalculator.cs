namespace KettleAndCask.Domain;

public class BrewCalculator
{
    public void Calculate(Recipe r)
    {
        if (r.Efficiency > 1)
            r.Efficiency = r.Efficiency / 100;

        // gravity
        double points = 0;
        foreach (var m in r.Malts)
        {
            if (m.Type == "grain")
                points += m.Amount * m.Potential * r.Efficiency;
            else
                points += m.Amount * m.Potential;
        }
        points = points / r.BatchSize;
        r.OG = 1 + points / 1000;

        // brew sheet lists hops in the order they go into the kettle
        r.Hops.Sort((a, b) => Order(a).CompareTo(Order(b)));

        // bitterness (Tinseth)
        double ibu = 0;
        foreach (var h in r.Hops)
        {
            if (h.Use == "dry hop")
                continue;

            double mgPerLiter = (h.Alpha / 100) * h.Amount * 1000 / r.BatchSize;
            double bigness = 1.65 * Math.Pow(0.000125, r.OG - 1);
            double boilFactor = (1 - Math.Exp(-0.04 * h.Time)) / 4.15;
            double u = bigness * boilFactor;
            if (h.Use == "whirlpool")
                u = u * 0.5;
            ibu += u * mgPerLiter;
        }
        r.IBU = ibu;

        // color (Morey)
        double mcu = 0;
        foreach (var m in r.Malts)
            mcu += m.Amount * 2.20462 * m.Color;
        mcu = mcu / (r.BatchSize * 0.264172);
        r.SRM = 1.4922 * Math.Pow(mcu, 0.6859);

        // final gravity and alcohol
        if (r.Yeast != null)
        {
            r.FG = 1 + (points * (1 - r.Yeast.Attenuation / 100)) / 1000;
            r.ABV = (r.OG - r.FG) * 131.25;
        }

        r.Calculated = true;
    }

    public void Adjust(Recipe r, double x)
    {
        double f = x / r.BatchSize;
        foreach (var m in r.Malts)
            m.Amount = m.Amount * f;
        foreach (var h in r.Hops)
            h.Amount = Math.Round(h.Amount * f);
        r.BatchSize = x;
        r.Calculated = false;
    }

    private static int Order(Hop h)
    {
        if (h.Use == "boil") return 1000 - h.Time;
        if (h.Use == "whirlpool") return 2000;
        return 3000;
    }
}
