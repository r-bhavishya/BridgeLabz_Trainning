using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class)]
class RoleAllowedAttribute : Attribute
{
    public string Role;

    public RoleAllowedAttribute(string role)
    {
        Role = role;
    }
}

[RoleAllowed("ADMIN")]
class AdminPanel
{
    public void DeleteUser()
    {
        Console.WriteLine("User deleted");
    }
}

class Program
{
    static void Main()
    {
        AdminPanel panel = new AdminPanel();
        Call(panel, "ADMIN");
        Call(panel, "USER");
    }

    static void Call(object item, string userRole)
    {
        RoleAllowedAttribute rule = item.GetType().GetCustomAttribute<RoleAllowedAttribute>();
        if (rule.Role != userRole)
        {
            Console.WriteLine("Access Denied!");
            return;
        }

        MethodInfo method = item.GetType().GetMethod("DeleteUser");
        method.Invoke(item, null);
    }
}
