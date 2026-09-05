using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
class TodoAttribute : Attribute
{
    public string Task;
    public string Person;

    public TodoAttribute(string task, string person)
    {
        Task = task;
        Person = person;
    }
}

class Features
{
    [Todo("Add search", "Sam")]
    [Todo("Add tests", "Mia")]
    public void BuildPage() { }

    [Todo("Fix menu", "Alex")]
    public void BuildMenu() { }
}

class Program
{
    static void Main()
    {
        foreach (MethodInfo method in typeof(Features).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            foreach (TodoAttribute todo in method.GetCustomAttributes<TodoAttribute>())
                Console.WriteLine(method.Name + ": " + todo.Task + " - " + todo.Person);
        }
    }
}
