using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DelegateInterface;

public class InterfaceToDelegateMap<T> : IDictionary<string, Delegate>
{
    private readonly Dictionary<string, Delegate> _map = new();

    private static void Check(MethodInfo method, Type delegateType)
    {
        var invoke = delegateType.GetMethod("Invoke")!;

        if (method.ReturnType != invoke.ReturnType) throw new InvalidOperationException("return type not matched");

        var mp = method.GetParameters();
        var ip = invoke.GetParameters();

        if (mp.Length != ip.Length) throw new InvalidOperationException("#parameter not matched");

        for (int i = 0; i < mp.Length; i++)
        {
            if (mp[i].ParameterType != ip[i].ParameterType) throw new InvalidOperationException($"parameter type not matched: {mp[i].Name}, {ip[i].Name}");
        }
    }

    private static KeyValuePair<string, Delegate> Check(KeyValuePair<string, Delegate> item)
    {
        InterfaceToDelegateMap<T>.Check(item.Key, item.Value);
        return item;
    }

    private static MethodInfo? GetMethod(string key, Type interfaceType)
    {
        if (interfaceType.GetMethod(key) is { } m) return m;

        foreach (var baseInterface in interfaceType.GetInterfaces())
        {
            if (baseInterface.GetMethod(key) is { } m1) return m1;
        }

        return null;
    }

    private static Delegate Check(string key, Delegate d)
    {
        var interfaceType = typeof(T);

        var m = GetMethod(key, interfaceType) ?? throw new InvalidOperationException($"{interfaceType.Name} does not have a method {key}.");
        var delegateType = d.GetType();
        Check(m, delegateType);
        return d;
    }

    public Delegate this[string key] { get => _map[key]; set => _map[key] = InterfaceToDelegateMap<T>.Check(key, value); }
    public ICollection<string> Keys => _map.Keys;
    public ICollection<Delegate> Values => _map.Values;
    public int Count => _map.Count;
    public bool IsReadOnly => false;
    public void Add(string key, Delegate value) => _map.Add(key, InterfaceToDelegateMap<T>.Check(key, value));
    public void Add(KeyValuePair<string, Delegate> item) => ((ICollection<KeyValuePair<string, Delegate>>)_map).Add(InterfaceToDelegateMap<T>.Check(item));
    public void Clear() => _map.Clear();
    public bool Contains(KeyValuePair<string, Delegate> item) => ((ICollection<KeyValuePair<string, Delegate>>)_map).Contains(item);
    public bool ContainsKey(string key) => _map.ContainsKey(key);
    public void CopyTo(KeyValuePair<string, Delegate>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, Delegate>>)_map).CopyTo(array, arrayIndex);
    public IEnumerator<KeyValuePair<string, Delegate>> GetEnumerator() => _map.GetEnumerator();
    public bool Remove(string key) => _map.Remove(key);
    public bool Remove(KeyValuePair<string, Delegate> item) => ((ICollection<KeyValuePair<string, Delegate>>)_map).Remove(item);
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Delegate value) => _map.TryGetValue(key, out value);
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_map).GetEnumerator();
}
