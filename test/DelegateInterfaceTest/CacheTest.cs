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

    [Fact]
    public void RefLikeTypeNotSupported()
    {
        Assert.Throws<InvalidOperationException>(() => _ = Cache<IRefReturn>.ProxyType);
        Assert.Throws<InvalidOperationException>(() => _ = Cache<IRefLikeReturn>.ProxyType);
        Assert.Throws<InvalidOperationException>(() => _ = Cache<IRefParam>.ProxyType);
        Assert.Throws<InvalidOperationException>(() => _ = Cache<IRefLikeParam>.ProxyType);
        Assert.Throws<InvalidOperationException>(() => _ = Cache<IInParam>.ProxyType);
        Assert.Throws<InvalidOperationException>(() => _ = Cache<IOutParam>.ProxyType);
    }

    [Fact]
    public void MustBeInterface()
    {
        Assert.Throws<InvalidOperationException>(() => _ = Cache<R>.ProxyType);
        Assert.Throws<InvalidOperationException>(() => _ = Cache<S>.ProxyType);
    }
}
