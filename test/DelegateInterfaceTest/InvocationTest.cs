using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class InvocationTest
{
    [Fact]
    public void UpdateDelegate()
    {
        const string methodName = nameof(ITest.P22);

        var x = Cache<ITest>.CreateInstance();
        var m = ((IDelegateInterface)x).Methods;

        m[methodName] = static (string s) => s;
        Assert.Equal("abc", x.P22("abc"));

        m[methodName] = static (string s) => s + s;
        Assert.Equal("abcabc", x.P22("abc"));
    }

    [Fact]
    public void AnyTypeOfPrameterAndReturn()
    {
        var x = Cache<ITest>.CreateInstance();
        var m = ((IDelegateInterface)x).Methods;

        int invoked = 0;

        m[nameof(ITest.P00)] = () => invoked |= 1;
        m[nameof(ITest.P01)] = () => { invoked |= 1 << 1; return 0; };
        m[nameof(ITest.P02)] = () => { invoked |= 1 << 2; return ""; };
        m[nameof(ITest.P10)] = (int _) => invoked |= 1 << 3;
        m[nameof(ITest.P11)] = (int _) => { invoked |= 1 << 4; return 0; };
        m[nameof(ITest.P12)] = (int _) => { invoked |= 1 << 5; return ""; };
        m[nameof(ITest.P20)] = (string _) => invoked |= 1 << 6;
        m[nameof(ITest.P21)] = (string _) => { invoked |= 1 << 7; return 0; };
        m[nameof(ITest.P22)] = (string _) => { invoked |= 1 << 8; return ""; };

        x.P00();
        x.P01();
        x.P02();
        x.P10(0);
        x.P11(0);
        x.P12(0);
        x.P20("");
        x.P21("");
        x.P22("");

        Assert.Equal((1 << 9) - 1, invoked);
    }

    [Fact]
    public void UserDefinedType()
    {
        var x = Cache<ITest>.CreateInstance();
        var m = ((IDelegateInterface)x).Methods;

        m[nameof(ITest.R11)] = S (S x) => x;
        m[nameof(ITest.R12)] = R (S x) => new(x.X, x.Y);
        m[nameof(ITest.R21)] = S (R x) => new(x.X, x.Y);
        m[nameof(ITest.R22)] = R (R x) => x with { };

        Assert.Equal(new(1, 2), x.R11(new(1, 2)));
        Assert.Equal(new(1, 2), x.R12(new(1, 2)));
        Assert.Equal(new(1, 2), x.R21(new(1, 2)));
        Assert.Equal(new(1, 2), x.R22(new(1, 2)));
    }
}
