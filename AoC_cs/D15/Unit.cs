using System;
using System.Collections.Generic;
using System.Text;

namespace AoC_cs.D15
{


    public class Unit : Entity
    {
        public int AttackPower { get; set; } = 3;
        public int Health { get; set; } = 200;
        public bool Alive { get; set; } = true;
        public Unit(int x, int y) : base(x, y)
        {
            
        }
        public Unit(int x, int y, int attackPower) : base(x, y)
        {
            AttackPower = attackPower;
        }

        public void Move(Entity entity)
        {
           if (!InRange(entity)) throw new ArgumentException();
           X = entity.X;
           Y = entity.Y;
        }

        public bool InRange(Entity other)
        {
            return 
                !(Math.Abs(X - other.X) > 1 ||
                  Math.Abs(Y - other.Y) > 1 ||
                 (Math.Abs(X - other.X) == 0 && Math.Abs(Y - other.Y) == 0) ||
                 (Math.Abs(X - other.X) == 1 && Math.Abs(Y - other.Y) == 1));
        }

        public void Attack(Unit victim)
        {
            victim.Health -= AttackPower;
        }

        public override string ToString()
        {
            return X + " " + Y;
        }
    }
}
