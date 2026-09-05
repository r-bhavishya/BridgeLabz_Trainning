using System;
using System.Reflection;
using System.Text;

[AttributeUsage(AttributeTargets.Field)]
class JsonFieldAttribute : Attribute
{
    public string Name { get; set; }
}

class User
{
    [JsonField(Name = "user_name")]
    public string Username = "alex";

    [JsonField(Name = "user_age")]
    public int Age = 20;
}

class Program
{
    static void Main()
    {
        User user = new User();
        StringBuilder json = new StringBuilder("{");
        FieldInfo[] fields = typeof(User).GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            JsonFieldAttribute info = fields[i].GetCustomAttribute<JsonFieldAttribute>();
            if (info == null) continue;
            if (json.Length > 1) json.Append(", ");

            json.Append('"').Append(info.Name).Append("\": ");
            object value = fields[i].GetValue(user);
            if (value is string) json.Append('"').Append(value).Append('"');
            else json.Append(value);
        }

        json.Append("}");
        Console.WriteLine(json);
    }
}
