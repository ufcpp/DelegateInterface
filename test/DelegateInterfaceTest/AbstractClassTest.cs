using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class AbstractClassTest
{
    [Fact]
    public void DerivedInterface()
    {
        var x = Cache<A>.CreateInstance();
        var m = ((IDelegateInterface)x).Methods;

        m["N"] = () => "abcd";

        Assert.Equal("abcd", x.N());
    }
}
