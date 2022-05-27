# DelegateInterface

## Library usage:

```cs
public interface IA
{
    int A(int x);
    int B(string x);
    (int x, int y) C(TimeSpan x);
    P D(P x, P y);
}
```

```cs
using DelegateInterface;

// Create a proxy instance.
var a = Cache<IA>.CreateInstance();

// Add delegates to the proxy.
var d = (IDelegateInterface)a;
int factor = 10;
d.Methods["A"] = (int x) => factor * x;
d.Methods["B"] = static (string x) => x.Length;
d.Methods["C"] = new C(1, 2).M;
d.Methods["D"] = static P (P x, P y) => new(x.X + y.X, x.Y + y.Y);

// Invoke interface methods.
Console.WriteLine(a.A(3));
Console.WriteLine(a.B("abc"));
Console.WriteLine(a.C(TimeSpan.FromSeconds(9999)));
Console.WriteLine(a.D(new(2, 3), new(5, 7)));
```

```
30
3
(2, 46)
P { X = 7, Y = 10 }
```

## How to work

`DelegateInterfaceTypeBuilder` class build a proxy class by using `System.Reflection.Emit`.

If you have a interface:

```cs
public interface IA
{
    int A(int x);
    int B(string x);
    (int x, int y) C(TimeSpan x);
    P D(P x, P y);
}
```

The builder class create a class:

```cs
class IA_Proxy : IDynamicInterface, IA
{
    public IDictionary<string, Delegate> Methods { get; }
    public void M1() => Methods["M1"].DynamicInvoke();
    public string M2() => (string)Methods["M2"].DynamicInvoke();
    public void M3(TimeSpan x) => Methods["M3"].DynamicInvoke(x);
    public int M4(int x, int y) => (int)Methods["M4"].DynamicInvoke(x, y);
}
```
