using System;

class Problem6
{
    static double CalculateInterest(double amount, double rate, int years)
    {
        if (amount < 0 || rate < 0)
            throw new ArgumentException();
        return amount * rate * years / 100;
    }

    static void Main()
    {
        try
        {
            Console.Write("Enter amount: ");
            double amount = double.Parse(Console.ReadLine());
            Console.Write("Enter rate: ");
            double rate = double.Parse(Console.ReadLine());
            Console.Write("Enter years: ");
            int years = int.Parse(Console.ReadLine());
            Console.WriteLine($"Interest: {CalculateInterest(amount, rate, years)}");
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Invalid input: Amount and rate must be positive");
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid input: enter numbers only");
        }
    }
}
