using System;
using System.IO;

class Problem1
{
    static void Main()
    {
        try
        {
            Console.WriteLine(File.ReadAllText("data.txt"));
        }
        catch (IOException)
        {
            Console.WriteLine("File not found");
        }
    }
}
