using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

class Person
{
    private int age = 20;
}

class Calculator
{
    private int Multiply(int a, int b)
    {
        return a * b;
    }
}

class Student
{
    public string Name;

    public Student(string name)
    {
        Name = name;
    }
}

class MathOperations
{
    public int Add(int a, int b) { return a + b; }
    public int Subtract(int a, int b) { return a - b; }
    public int Multiply(int a, int b) { return a * b; }
}

[AttributeUsage(AttributeTargets.Class)]
class AuthorAttribute : Attribute
{
    public string Name { get; }

    public AuthorAttribute(string name)
    {
        Name = name;
    }
}

[Author("Student Author")]
class Book
{
    public string Title = "C# Basics";
}

class Configuration
{
    private static string API_KEY = "old-key";

    public static string GetKey()
    {
        return API_KEY;
    }
}

class Product
{
    public string Name;
    public double Price;
}

interface IGreeting
{
    void SayHello();
}

class Greeting : IGreeting
{
    public void SayHello()
    {
        Console.WriteLine("Hello!");
    }
}

class GreetingProxy : DispatchProxy
{
    private IGreeting target;

    public void SetTarget(IGreeting value)
    {
        target = value;
    }

    protected override object Invoke(MethodInfo method, object[] args)
    {
        Console.WriteLine("Calling: " + method.Name);
        return method.Invoke(target, args);
    }
}

[AttributeUsage(AttributeTargets.Field)]
class InjectAttribute : Attribute
{
}

class MessageService
{
    public string GetMessage()
    {
        return "Message from service";
    }
}

class MessageController
{
    [Inject]
    private MessageService service;

    public void Show()
    {
        Console.WriteLine(service.GetMessage());
    }
}

class SlowWork
{
    public void Run()
    {
        System.Threading.Thread.Sleep(100);
        Console.WriteLine("Work finished");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Reflection Practice");

        ClassInfo();
        PrivateField();
        PrivateMethod();
        CreateStudent();
        DynamicCall();
        ReadAuthor();
        StaticField();
        ObjectMapper();
        ToJson();
        ProxyTest();
        DiTest();
        TimeMethod();
    }

    static void ClassInfo()
    {
        Console.Write("Enter a class name (blank = Person): ");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name)) name = "Person";

        Type type = Type.GetType(name);
        if (type == null)
        {
            Console.WriteLine("Class not found");
            return;
        }

        Console.WriteLine("Methods:");
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            Console.WriteLine(method.Name);

        Console.WriteLine("Fields:");
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            Console.WriteLine(field.Name);

        Console.WriteLine("Constructors:");
        foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.Instance))
            Console.WriteLine(constructor);
    }

    static void PrivateField()
    {
        Person person = new Person();
        FieldInfo field = typeof(Person).GetField("age", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(person, 30);
        Console.WriteLine("Person age: " + field.GetValue(person));
    }

    static void PrivateMethod()
    {
        Calculator calculator = new Calculator();
        MethodInfo method = typeof(Calculator).GetMethod("Multiply", BindingFlags.NonPublic | BindingFlags.Instance);
        object result = method.Invoke(calculator, new object[] { 4, 5 });
        Console.WriteLine("Private multiply: " + result);
    }

    static void CreateStudent()
    {
        Student student = (Student)Activator.CreateInstance(typeof(Student), new object[] { "Sam" });
        Console.WriteLine("Student: " + student.Name);
    }

    static void DynamicCall()
    {
        MathOperations math = new MathOperations();
        Console.Write("Choose Add, Subtract, or Multiply (blank = Add): ");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name)) name = "Add";

        MethodInfo method = typeof(MathOperations).GetMethod(name);
        if (method == null)
        {
            Console.WriteLine("Method not found");
            return;
        }

        Console.WriteLine("Result: " + method.Invoke(math, new object[] { 10, 2 }));
    }

    static void ReadAuthor()
    {
        AuthorAttribute author = typeof(Book).GetCustomAttribute<AuthorAttribute>();
        Console.WriteLine("Author: " + author.Name);
    }

    static void StaticField()
    {
        FieldInfo field = typeof(Configuration).GetField("API_KEY", BindingFlags.NonPublic | BindingFlags.Static);
        field.SetValue(null, "new-key");
        Console.WriteLine("API key: " + Configuration.GetKey());
    }

    static void ObjectMapper()
    {
        Dictionary<string, object> values = new Dictionary<string, object>
        {
            { "Name", "Pen" },
            { "Price", 2.5 }
        };

        Product product = ToObject<Product>(values);
        Console.WriteLine("Mapped object: " + product.Name + ", " + product.Price);
    }

    static T ToObject<T>(Dictionary<string, object> values) where T : new()
    {
        T item = new T();
        foreach (KeyValuePair<string, object> pair in values)
        {
            FieldInfo field = typeof(T).GetField(pair.Key,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) field.SetValue(item, pair.Value);
        }
        return item;
    }

    static void ToJson()
    {
        Product product = new Product { Name = "Book", Price = 10 };
        StringBuilder json = new StringBuilder("{");
        FieldInfo[] fields = product.GetType().GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            if (i > 0) json.Append(", ");
            json.Append('"').Append(fields[i].Name).Append("\": ");
            object value = fields[i].GetValue(product);
            if (value is string) json.Append('"').Append(value).Append('"');
            else json.Append(value);
        }

        json.Append("}");
        Console.WriteLine("JSON: " + json);
    }

    static void ProxyTest()
    {
        IGreeting greeting = DispatchProxy.Create<IGreeting, GreetingProxy>();
        ((GreetingProxy)greeting).SetTarget(new Greeting());
        greeting.SayHello();
    }

    static void DiTest()
    {
        MessageController controller = new MessageController();
        foreach (FieldInfo field in typeof(MessageController).GetFields(BindingFlags.NonPublic |
            BindingFlags.Instance))
        {
            if (field.GetCustomAttribute<InjectAttribute>() != null)
                field.SetValue(controller, Activator.CreateInstance(field.FieldType));
        }
        controller.Show();
    }

    static void TimeMethod()
    {
        SlowWork work = new SlowWork();
        MethodInfo method = typeof(SlowWork).GetMethod("Run");
        Stopwatch watch = Stopwatch.StartNew();
        method.Invoke(work, null);
        watch.Stop();
        Console.WriteLine("Time: " + watch.ElapsedMilliseconds + " ms");
    }
}
