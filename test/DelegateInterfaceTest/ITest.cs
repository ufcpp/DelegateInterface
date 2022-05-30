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
}

public record R(int X, int Y);
public record struct S(int X, int Y);
