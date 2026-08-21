using System;

class Problem9
{
    static void Main()
    {
        try
        {
            Console.Write("Enter numbers separated by commas: ");
            string? input = Console.ReadLine();
            int[]? numbers = string.IsNullOrWhiteSpace(input) ? null : Array.ConvertAll(input.Split(','), int.Parse);
            Console.Write("Enter an index: ");
            int index = int.Parse(Console.ReadLine()!);
            if (numbers == null)
                throw new NullReferenceException();
            try
            {
                Console.Write("Enter a divisor: ");
                int divisor = int.Parse(Console.ReadLine());
                Console.WriteLine($"Result: {numbers[index] / divisor}");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Cannot divide by zero!");
            }
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Invalid array index!");
        }
        catch (FormatException)
        {
            Console.WriteLine("Please enter valid integers");
        }
    }
}
