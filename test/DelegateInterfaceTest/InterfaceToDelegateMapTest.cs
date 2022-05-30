using DelegateInterface;
using Xunit;

namespace DelegateInterfaceTest;

public class InterfaceToDelegateMapTest
{
    [Fact]
    public void ThrowIfMethodNotExsit()
    {
        {
            var map = new InterfaceToDelegateMap<ITest>();
            map.Add("X", (string s) => s);
        }

        {
            var map = new InterfaceToDelegateMap<ITest>();
            map["X"] = (string s) => s;
        }

        {
            ICollection<KeyValuePair<string, Delegate>> map = new InterfaceToDelegateMap<ITest>();
            map.Add(new("X", (string s) => s));
        }

        Assert.Throws<InvalidOperationException>(() =>
        {
            var map = new InterfaceToDelegateMap<ITest>();
            map.Add("NotExist", (string s) => s);
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            var map = new InterfaceToDelegateMap<ITest>();
            map["NotExist"] = (string s) => s;
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            ICollection<KeyValuePair<string, Delegate>> map = new InterfaceToDelegateMap<ITest>();
            map.Add(new("NotExist", (string s) => s));
        });
    }
}
