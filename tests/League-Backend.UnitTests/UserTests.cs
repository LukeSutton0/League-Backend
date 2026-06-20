namespace League_Backend.UnitTests;

public class UnitTests
{
    [Fact]
    public void Test1()
    {
        Assert.Equal(4.0, 4.0, 1);
    }

    [Fact]
    public void Test2()
    {
        int x = 3;
        Assert.True(x == 1 + 2);
    }
}
