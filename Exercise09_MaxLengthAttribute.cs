using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
class MaxLengthAttribute : Attribute
{
    public int Value;

    public MaxLengthAttribute(int value)
    {
        Value = value;
    }
}

class User
{
    [MaxLength(8)]
    public string Username;

    public User(string username)
    {
        FieldInfo field = typeof(User).GetField("Username");
        MaxLengthAttribute max = field.GetCustomAttribute<MaxLengthAttribute>();

        if (username.Length > max.Value)
            throw new ArgumentException("Username is too long");

        Username = username;
    }
}

class Program
{
    static void Main()
    {
        User user = new User("alex");
        Console.WriteLine(user.Username);

        try
        {
            new User("verylongname");
        }
        catch (ArgumentException error)
        {
            Console.WriteLine(error.Message);
        }
    }
}
