using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class InvocationTest
{
    [Fact]
    public void Invoke()
    {
        const string methodName = nameof(ITest.A);

        var x = Cache<ITest>.CreateInstance();
        var m = ((IDelegateInterface)x).Methods;

        m[methodName] = static (string s) => s;
        Assert.Equal("abc", x.A("abc"));

        m[methodName] = static (string s) => s + s;
        Assert.Equal("abcabc", x.A("abc"));
    }
}
