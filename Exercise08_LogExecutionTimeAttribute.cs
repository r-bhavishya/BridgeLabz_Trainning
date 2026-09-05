using System;
using System.Diagnostics;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
class LogExecutionTimeAttribute : Attribute
{
}

class Work
{
    [LogExecutionTime]
    public void FastWork()
    {
        for (int i = 0; i < 1000; i++) { }
    }

    [LogExecutionTime]
    public void SlowWork()
    {
        System.Threading.Thread.Sleep(100);
    }
}

class Program
{
    static void Main()
    {
        Work work = new Work();
        Run(work, "FastWork");
        Run(work, "SlowWork");
    }

    static void Run(object item, string name)
    {
        MethodInfo method = item.GetType().GetMethod(name);
        if (method.GetCustomAttribute<LogExecutionTimeAttribute>() == null) return;

        Stopwatch watch = Stopwatch.StartNew();
        method.Invoke(item, null);
        watch.Stop();
        Console.WriteLine(name + ": " + watch.ElapsedMilliseconds + " ms");
    }
}
