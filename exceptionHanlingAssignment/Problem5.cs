using System;
using System.IO;

class Problem5
{
    static void Main()
    {
        try
        {
            using (StreamReader reader = new StreamReader("info.txt"))
            {
                Console.WriteLine(reader.ReadLine());
            }
        }
        catch (IOException)
        {
            Console.WriteLine("Error reading file");
        }
    }
}
