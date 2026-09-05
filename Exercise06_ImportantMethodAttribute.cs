using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
class ImportantMethodAttribute : Attribute
{
    public string Level;

    public ImportantMethodAttribute(string level = "HIGH")
    {
        Level = level;
    }
}

class Work
{
    [ImportantMethod]
    public void Save() { }

    [ImportantMethod("LOW")]
    public void Print() { }

    public void Test() { }
}

class Program
{
    static void Main()
    {
        foreach (MethodInfo method in typeof(Work).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            ImportantMethodAttribute info = method.GetCustomAttribute<ImportantMethodAttribute>();
            if (info != null)
                Console.WriteLine(method.Name + " - " + info.Level);
        }
    }
}
