using System;

class Problem8
{
    static void Method1()
    {
        int first = 10;
        int second = 0;
        int result = first / second;
    }

    static void Method2()
    {
        Method1();
    }

    static void Main()
    {
        try
        {
            Method2();
        }
        catch (ArithmeticException)
        {
            Console.WriteLine("Handled exception in Main");
        }
    }
}
