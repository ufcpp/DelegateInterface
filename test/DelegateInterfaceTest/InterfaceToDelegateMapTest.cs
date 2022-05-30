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
            map.Add("A", (string s) => s);
        }

        {
            var map = new InterfaceToDelegateMap<ITest>();
            map["A"] = (string s) => s;
        }

        {
            ICollection<KeyValuePair<string, Delegate>> map = new InterfaceToDelegateMap<ITest>();
            map.Add(new("A", (string s) => s));
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
