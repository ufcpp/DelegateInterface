using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class InterfaceToDelegateMapTest
{
    [Fact]
    public void ThrowIfMethodNotExsit()
    {
        const string methodName = nameof(ITest.A);
        const string invalidName = "NotExist";

        {
            var map = new InterfaceToDelegateMap<ITest>();
            map.Add(methodName, (string s) => s);
        }

        {
            var map = new InterfaceToDelegateMap<ITest>();
            map[methodName] = (string s) => s;
        }

        {
            ICollection<KeyValuePair<string, Delegate>> map = new InterfaceToDelegateMap<ITest>();
            map.Add(new(methodName, (string s) => s));
        }

        Assert.Throws<InvalidOperationException>(() =>
        {
            var map = new InterfaceToDelegateMap<ITest>();
            map.Add(invalidName, (string s) => s);
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            var map = new InterfaceToDelegateMap<ITest>();
            map[invalidName] = (string s) => s;
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            ICollection<KeyValuePair<string, Delegate>> map = new InterfaceToDelegateMap<ITest>();
            map.Add(new(invalidName, (string s) => s));
        });
    }
}
