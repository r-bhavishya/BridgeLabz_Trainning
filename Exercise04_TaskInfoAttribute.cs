using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
class TaskInfoAttribute : Attribute
{
    public string Priority;
    public string AssignedTo;

    public TaskInfoAttribute(string priority, string assignedTo)
    {
        Priority = priority;
        AssignedTo = assignedTo;
    }
}

class TaskManager
{
    [TaskInfo("High", "Sam")]
    public void FixLogin()
    {
        Console.WriteLine("Login fixed");
    }
}

class Program
{
    static void Main()
    {
        MethodInfo method = typeof(TaskManager).GetMethod("FixLogin");
        TaskInfoAttribute info = method.GetCustomAttribute<TaskInfoAttribute>();

        Console.WriteLine("Priority: " + info.Priority);
        Console.WriteLine("Assigned to: " + info.AssignedTo);
    }
}
