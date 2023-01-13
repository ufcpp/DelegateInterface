using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class MultipleInterfacesTest
{
    [Fact]
    public void TwoInterfaces()
    {
        var x = Cache.CreateInstance(typeof(ITest), typeof(IDerived));
        var m = ((IDelegateInterface)x).Methods;

        m[nameof(ITest.P22)] = static (string s) => s;
        Assert.Equal("abc", ((ITest)x).P22("abc"));

        m[nameof(IDerived.M)] = () => 10;
        m[nameof(IDerived.N)] = () => "abcd";

        Assert.Equal(10, ((IDerived)x).M());
        Assert.Equal("abcd", ((IDerived)x).N());
    }
}
