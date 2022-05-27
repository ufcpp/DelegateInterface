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
