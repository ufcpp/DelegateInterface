namespace DelegateInterfaceTest;

public interface ITest
{
    void P00();
    int P01();
    string P02();
    void P10(int x);
    int P11(int x);
    string P12(int x);
    void P20(string x);
    int P21(string x);
    string P22(string x);

    S R11(S x);
    R R12(S x);
    S R21(R x);
    R R22(R x);

    (int A, int B) ManyParameters(int a, int b, int c, int d, params int[] e);
}

public record R(int X, int Y);
public record struct S(int X, int Y);

public interface IRefReturn { ref int M(); }
public interface IRefLikeReturn { Span<int> M(); }
public interface IRefParam { void M(ref int x); }
public interface IRefLikeParam { void M(Span<int> x); }
public interface IInParam { void M(in int x); }
public interface IOutParam { void M(out int x); }

public interface IBase
{
    int M();
}

public interface IDerived : IBase
{
    string N();
}

public abstract class A : IDerived
{
    public int M() => 0;
    public abstract string N();
}
