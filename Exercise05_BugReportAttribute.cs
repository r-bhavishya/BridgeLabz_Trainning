using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
class BugReportAttribute : Attribute
{
    public string Description;

    public BugReportAttribute(string description)
    {
        Description = description;
    }
}

class BugTracker
{
    [BugReport("Button does not work")]
    [BugReport("Text is too small")]
    public void OpenScreen()
    {
        Console.WriteLine("Screen opened");
    }
}

class Program
{
    static void Main()
    {
        MethodInfo method = typeof(BugTracker).GetMethod("OpenScreen");
        BugReportAttribute[] bugs = method.GetCustomAttributes<BugReportAttribute>();

        foreach (BugReportAttribute bug in bugs)
            Console.WriteLine("Bug: " + bug.Description);
    }
}
