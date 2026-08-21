using System;

class Problem4
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
            Console.WriteLine($"Value at index {index}: {numbers[index]}");
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Invalid index!");
        }
        catch (NullReferenceException)
        {
            Console.WriteLine("Array is not initialized!");
        }
        catch (FormatException)
        {
            Console.WriteLine("Please enter a valid index");
        }
    }
}
