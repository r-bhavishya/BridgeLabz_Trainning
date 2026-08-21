using System;

class Problem2
{
    static void Main()
    {
        try
        {
            Console.Write("Enter first number: ");
            double first = double.Parse(Console.ReadLine());
            Console.Write("Enter second number: ");
            double second = double.Parse(Console.ReadLine());
            Console.WriteLine($"Result: {first / second}");
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("Cannot divide by zero");
        }
        catch (FormatException)
        {
            Console.WriteLine("Please enter valid numbers");
        }
    }
}
