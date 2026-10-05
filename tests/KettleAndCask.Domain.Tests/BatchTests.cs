using KettleAndCask.Domain;

namespace KettleAndCask.Domain.Tests;

public class BatchTests
{
    [Fact]
    public void Blending_takes_a_volume_weighted_average()
    {
        var young = new Batch { Name = "Young sour", Volume = 300, Gravity = 1.010, Abv = 5.0 };
        var aged = new Batch { Name = "Aged sour", Volume = 100, Gravity = 1.002, Abv = 7.0 };

        young.BlendIn(aged);

        Assert.Equal(400, young.Volume);
        Assert.Equal(1.008, young.Gravity, 6);
        Assert.Equal(5.5, young.Abv, 6);
    }
}
