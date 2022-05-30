using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class CacheTest
{
    [Fact]
    public void ReferenceType()
    {
        Assert.NotNull(Cache<ITest>.ProxyType);
        Assert.NotNull(Cache<ITest>.CreateInstance());
    }
}
