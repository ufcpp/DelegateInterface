using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class InheritanceTest
{
    [Fact]
    public void DerivedInterface()
    {
        var x = Cache<IDerived>.CreateInstance();
        var m = ((IDelegateInterface)x).Methods;

        m["M"] = () => 10;
        m["N"] = () => "abcd";

        Assert.Equal(10, x.M());
        Assert.Equal("abcd", x.N());
    }
}
