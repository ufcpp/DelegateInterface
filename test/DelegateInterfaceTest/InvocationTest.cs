using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class InvocationTest
{
    [Fact]
    public void Invoke()
    {
        var x = Cache<ITest>.CreateInstance();
        var m = ((IDelegateInterface)x).Methods;

        m["A"] = static (string s) => s;
        Assert.Equal("abc", x.A("abc"));

        m["A"] = static (string s) => s + s;
        Assert.Equal("abcabc", x.A("abc"));
    }
}
