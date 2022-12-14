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
}
