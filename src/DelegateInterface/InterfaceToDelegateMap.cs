using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DelegateInterface;

public class InterfaceToDelegateMap : IDictionary<string, Delegate>
{
    private readonly Dictionary<string, Delegate> _map = new();

    public Delegate this[string key]
    {
        get => _map[key];
        set => _map[key] = value; //todo: type check
    }

    public ICollection<string> Keys => _map.Keys;
    public ICollection<Delegate> Values => _map.Values;
    public int Count => _map.Count;
    public bool IsReadOnly => false;
    public void Add(string key, Delegate value) => _map.Add(key, value);
    public void Add(KeyValuePair<string, Delegate> item) => ((ICollection<KeyValuePair<string, Delegate>>)_map).Add(item);
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
