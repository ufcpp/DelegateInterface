record class C(int A, int B)
{
    public (int x, int y) M(TimeSpan s) => (M(A, s), M(B, s));
    private static int M(int a, TimeSpan s) => a switch
    {
        0 => s.Days,
        1 => s.Hours,
        2 => s.Minutes,
        3 => s.Seconds,
        _ => s.Milliseconds,
    };
}
