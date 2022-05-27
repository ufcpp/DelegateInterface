using DelegateInterface;

var a = Cache<IA>.CreateInstance();

int factor = 10;

var d = (IDelegateInterface)a;
d.Methods["A"] = (int x) => factor * x;
d.Methods["B"] = static (string x) => x.Length;
d.Methods["C"] = new C(1, 2).M;
d.Methods["D"] = static P (P x, P y) => new(x.X + y.X, x.Y + y.Y);

Console.WriteLine(a.A(3));
Console.WriteLine(a.B("abc"));
Console.WriteLine(a.C(TimeSpan.FromSeconds(9999)));
Console.WriteLine(a.D(new(2, 3), new(5, 7)));
