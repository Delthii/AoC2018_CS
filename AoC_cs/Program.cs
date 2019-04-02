using System;
using AoC_cs.D15;

namespace AoC_cs
{
    class Program
    {
        static void Main(string[] args)
        {
            IAoC day15 = new Day15();
            Console.WriteLine(day15.PartA());
            Console.WriteLine(day15.PartB());
        }
    }
}
