using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class InterfaceToDelegateMapTest
{
    [Fact]
    public void ThrowIfMethodNotExist()
    {
        const string methodName = nameof(ITest.P00);
        const string invalidName = "NotExist";

        {
            var map = new InterfaceToDelegateMap<ITest>();
            map.Add(methodName, () => { });
        }

        {
            var map = new InterfaceToDelegateMap<ITest>();
            map[methodName] = () => { };
        }

        {
            ICollection<KeyValuePair<string, Delegate>> map = new InterfaceToDelegateMap<ITest>();
            map.Add(new(methodName, () => { }));
        }

        Assert.Throws<InvalidOperationException>(() =>
        {
            var map = new InterfaceToDelegateMap<ITest>();
            map.Add(invalidName, () => { });
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            var map = new InterfaceToDelegateMap<ITest>();
            map[invalidName] = () => { };
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            ICollection<KeyValuePair<string, Delegate>> map = new InterfaceToDelegateMap<ITest>();
            map.Add(new(invalidName, () => { }));
        });
    }
}
