using System;

class LegacyAPI
{
    [Obsolete("Use NewFeature instead")]
    public void OldFeature()
    {
        Console.WriteLine("Old feature");
    }

    public void NewFeature()
    {
        Console.WriteLine("New feature");
    }
}

class Program
{
    static void Main()
    {
        LegacyAPI api = new LegacyAPI();
        api.OldFeature();
        api.NewFeature();
    }
}
