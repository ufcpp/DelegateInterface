namespace DelegateInterface;

/// <summary>
/// Implemented by types built by <see cref="DelegateInterfaceTypeBuilder"/>.
/// </summary>
public interface IDelegateInterface
{
    /// <summary>
    /// Map method name to delegate instance.
    /// </summary>
    public IDictionary<string, Delegate> Methods { get; }
}
