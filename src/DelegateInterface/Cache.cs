using System.Runtime.InteropServices;

namespace DelegateInterface;

/// <summary>
/// Static cache for <see cref="DelegateInterfaceTypeBuilder"/>.
/// </summary>
public static class Cache
{
    private static readonly DelegateInterfaceTypeBuilder _builder = new();

    internal static Type Create(Type interfaceType)
    {
        lock (_builder)
        {
            return _builder.Build(interfaceType);
        }
    }

    private readonly static Dictionary<Type, Type> _interfaceToProsy = new();
    private readonly static Dictionary<Type[], Type> _interfacesToProsy = new(new TypesComparer());

    private class TypesComparer : IEqualityComparer<Type[]>
    {
        public bool Equals(Type[] x, Type[] y)
        {
            if (x.Length != y.Length) return false;
            for (int i = 0; i < x.Length; i++) if (x[i] != y[i]) return false;
            return true;
        }

        public int GetHashCode(Type[] obj)
        {
            var c = new HashCode();
            foreach (var x in obj) c.Add(x);
            return c.ToHashCode();
        }
    }

    public static Type GetProxyType(params Type[] interfaceTypes)
    {
        if (interfaceTypes.Length == 0) throw new InvalidOperationException();
        if (interfaceTypes.Length == 1) return GetProxyType(interfaceTypes[0]);

        lock (_interfacesToProsy)
        {
            if (!_interfacesToProsy.TryGetValue(interfaceTypes, out var proxyType))
            {
                var t = _builder.AppendInterfaces(interfaceTypes);
                _interfacesToProsy[interfaceTypes] = proxyType = Create(t);
            }
            return proxyType!;
        }
    }

    public static Type GetProxyType(Type interfaceType)
    {
        lock (_interfaceToProsy)
        {
#if NET6_0_OR_GREATER
            ref var proxyType = ref CollectionsMarshal.GetValueRefOrAddDefault(_interfaceToProsy, interfaceType, out var exists);
            if (!exists) proxyType = Create(interfaceType);
            return proxyType!;
#else
            if (!_interfaceToProsy.TryGetValue(interfaceType, out var proxyType))
                _interfaceToProsy[interfaceType] = proxyType = Create(interfaceType);
            return proxyType!;
#endif
        }
    }

    public static object CreateInstance(Type interfaceType) => Activator.CreateInstance(GetProxyType(interfaceType))!;
    public static object CreateInstance(params Type[] interfaceTypes) => Activator.CreateInstance(GetProxyType(interfaceTypes))!;
}
