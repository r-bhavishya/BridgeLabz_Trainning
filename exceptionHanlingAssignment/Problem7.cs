using System;

class Problem7
{
    static void Main()
    {
        try
        {
            Console.Write("Enter numerator: ");
            int first = int.Parse(Console.ReadLine());
            Console.Write("Enter denominator: ");
            int second = int.Parse(Console.ReadLine());
            Console.WriteLine($"Result: {first / second}");
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("Cannot divide by zero");
        }
        catch (FormatException)
        {
            Console.WriteLine("Please enter valid integers");
        }
        finally
        {
            Console.WriteLine("Operation completed");
        }
    }
}
