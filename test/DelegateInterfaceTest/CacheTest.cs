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
        Assert.NotNull(Cache<A>.ProxyType);
        Assert.NotNull(Cache<A>.CreateInstance());
    }

    [Fact]
    public void CanGetTypeAndInstance()
    {
        Assert.NotNull(Cache.GetProxyType(typeof(ITest)));
        Assert.NotNull(Cache.CreateInstance(typeof(ITest)));
        Assert.NotNull(Cache.GetProxyType(typeof(A)));
        Assert.NotNull(Cache.CreateInstance(typeof(A)));
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
    public void MustBeInterfaceOrAbstract()
    {
        Assert.Throws<InvalidOperationException>(() => _ = Cache<R>.ProxyType);
        Assert.Throws<InvalidOperationException>(() => _ = Cache<S>.ProxyType);
    }
}
