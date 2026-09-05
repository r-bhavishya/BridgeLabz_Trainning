using System;
using System.Collections;

#pragma warning disable 0618

class Program
{
    static void Main()
    {
        ArrayList items = new ArrayList();
        items.Add("Apple");
        items.Add(10);

        foreach (object item in items)
            Console.WriteLine(item);
    }
}

#pragma warning restore 0618
