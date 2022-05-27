namespace DelegateInterface;

/// <summary>
/// Strongly-typed static cache for <see cref="DelegateInterfaceTypeBuilder"/>.
/// </summary>
public class Cache<TInterface>
{
    public static Type ProxyType => _proxyType ??= Cache.Create(typeof(TInterface));
    private static Type? _proxyType;

    public static TInterface CreateInstance() => (TInterface)Activator.CreateInstance(ProxyType)!;
}
