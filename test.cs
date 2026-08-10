using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Diagnostics;

class Program
{
    static void Main()
    {
        foreach (var prop in typeof(CoreEventId).GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            if (prop.Name.Contains("QueryFilter") || prop.Name.Contains("Navigation") || prop.Name.Contains("Required"))
            {
                var val = (EventId)prop.GetValue(null);
                if (val.Id == 10622)
                {
                    Console.WriteLine("FOUND: " + prop.Name);
                }
            }
        }
    }
}
