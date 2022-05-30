using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class CacheTest
{
    [Fact]
    public void CanGetTypeAndInstanceOfT()
    {
        Assert.NotNull(Cache<ITest>.ProxyType);
        Assert.NotNull(Cache<ITest>.CreateInstance());
    }

    [Fact]
    public void CanGetTypeAndInstance()
    {
        Assert.NotNull(Cache.GetProxyType(typeof(ITest)));
        Assert.NotNull(Cache.CreateInstance(typeof(ITest)));
    }
}
