using System;
using System.Collections.Generic;
using System.Text;

namespace AoC_cs.D15
{
    public class Entity : IEquatable<Entity>, IComparable<Entity>
    {
        public Entity(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public bool Equals(Entity other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X + Y*1024*1024;
        }

        public int CompareTo(Entity other)
        {
            return X == other.X ? Y.CompareTo(other.Y) : X.CompareTo(other.X);
        }
    }

}
