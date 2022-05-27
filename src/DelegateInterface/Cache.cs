using System.Runtime.InteropServices;

namespace DelegateInterface;

/// <summary>
/// Static cache for <see cref="DelegateInterfaceTypeBuilder"/>.
/// </summary>
public static class Cache
{
    private static readonly DelegateInterfaceTypeBuilder _builder = new();

    internal static Type Create(Type interfaceType) => _builder.Build(interfaceType);

    private readonly static Dictionary<Type, Type> _interfaceToProsy = new();

    public static Type GetProxyType(Type interfaceType)
    {
        ref var proxyType = ref CollectionsMarshal.GetValueRefOrAddDefault(_interfaceToProsy, interfaceType, out var exists);
        if (!exists) proxyType = Create(interfaceType);
        return proxyType!;
    }

    public static object CreateInstance(Type interfaceType) => Activator.CreateInstance(GetProxyType(interfaceType))!;
}
