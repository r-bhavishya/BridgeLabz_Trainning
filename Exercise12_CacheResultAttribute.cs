using System;
using System.Collections.Generic;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
class CacheResultAttribute : Attribute
{
}

class Calculator
{
    [CacheResult]
    public int Square(int number)
    {
        Console.WriteLine("Calculating...");
        return number * number;
    }
}

class Program
{
    static Dictionary<string, object> cache = new Dictionary<string, object>();

    static void Main()
    {
        Calculator calculator = new Calculator();
        Console.WriteLine(Call(calculator, "Square", 5));
        Console.WriteLine(Call(calculator, "Square", 5));
    }

    static object Call(object item, string name, int number)
    {
        MethodInfo method = item.GetType().GetMethod(name);
        string key = name + number;

        if (method.GetCustomAttribute<CacheResultAttribute>() != null && cache.ContainsKey(key))
        {
            Console.WriteLine("Using cache...");
            return cache[key];
        }

        object result = method.Invoke(item, new object[] { number });
        if (method.GetCustomAttribute<CacheResultAttribute>() != null)
            cache[key] = result;
        return result;
    }
}
