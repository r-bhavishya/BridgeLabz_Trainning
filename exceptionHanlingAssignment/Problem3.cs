using System;

class InvalidAgeException : Exception
{
    public InvalidAgeException() : base("Age must be 18 or above") { }
}

class Problem3
{
    static void ValidateAge(int age)
    {
        if (age < 18)
            throw new InvalidAgeException();
    }

    static void Main()
    {
        try
        {
            Console.Write("Enter your age: ");
            int age = int.Parse(Console.ReadLine());
            ValidateAge(age);
            Console.WriteLine("Access granted!");
        }
        catch (InvalidAgeException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (FormatException)
        {
            Console.WriteLine("Please enter a valid age");
        }
    }
}
