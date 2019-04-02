using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AoC_cs.D15
{
    public class Day15 : IAoC
    {
        private List<Unit> _goblins;
        private List<Unit> _elves;
        private bool[,] _map;

        public Day15()
        {
            ParseAndInit(3);
        }

        private void ParseAndInit(int ap)
        {
            var lines = File.ReadAllLines("C:\\Users\\Peter\\source\\repos\\AoC_cs\\AoC_cs\\input15.txt");
            _goblins = new List<Unit>();
            _elves = new List<Unit>();
            _map = new bool[lines.Length, lines.First().Length];
            for (var i = 0; i < lines.Length; i++)
            {
                for (var j = 0; j < lines[i].Length; j++)
                {
                    _map[i, j] = true;
                    switch (lines[i][j])
                    {
                        case '#':
                            _map[i, j] = false;
                            break;
                        case 'E':
                            _elves.Add(new Elfe(i, j, ap));
                            break;
                        case 'G':
                            _goblins.Add(new Goblin(i, j));
                            break;
                    }
                }
            }
        }

        //Helper
        private void PrintMap()
        {
            var map = IndividualMap;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if(map[i, j]) Console.Write(".");
                    else
                    {
                        var unit = Units.Where(u => u.X == i && u.Y == j);
                        if (unit.Any()) Console.Write(unit.First() is Goblin ? "G" : "E");
                        else Console.Write("#");
                    }
                }
                Console.WriteLine();
            }
        }

        public string PartA()
        {
            return "" + SolveA();
        }

        private int SolveA()
        {
            var count = 0;
            while (true)
            {
                foreach (var unit in Units.OrderBy(e => e))
                {
                    if (!unit.Alive) continue;
                    Move(unit);
                    Attack(unit);
                    if (!_elves.Any(u => u.Alive) || !_goblins.Any(u => u.Alive))
                        return Units.Where(u => u.Alive).Sum(u => u.Health) * count;
                }
                count++;
            }
        }

        public string PartB()
        {
            for (var i = 4;; i++)
            {
                ParseAndInit(i);
                var ans = SolveA();
                if (_elves.All(e => e.Alive))
                {
                    return "" + ans;
                }
            }
        }

        private bool Move(Unit unit)
        {
            var enemies = GetEnemies(unit);
            if (enemies.Any(unit.InRange)) return false;
            var map = IndividualMap;
            var reachableTiles = GetReachableTilesAdjacentToEnemies(unit, enemies, map);

            if (!reachableTiles.Any()) return false;

            var nearestTile = GetNearestTile(reachableTiles);
            var nextMove = GetFirstMoveOnPath(unit, nearestTile, map);
            unit.Move(nextMove);

            return true;
        }

        private bool Attack(Unit unit)
        {
            var enemies = GetEnemies(unit);
            var inRange = enemies.Where(enemy => enemy.InRange(unit)).ToList();
            if (!inRange.Any()) return false;
            var victim = inRange.OrderBy(e => e.Health).ThenBy(e => e).First();
            unit.Attack(victim);
            if (victim.Health <= 0)
            {
                victim.Alive = false;
            }

            return true;

        }

        private List<Unit> GetEnemies(Unit unit)
        {
            var enemies = (unit is Goblin ? _elves : _goblins).Where(e => e.Alive).ToList();
            return enemies;
        }

        private Entity GetFirstMoveOnPath(Entity start, Entity goal, bool[,] map)
        {
           return
                 Adjacent(start, map)
                .Select(adjTile => Tuple.Create(adjTile, CalcDistance(adjTile, goal)))
                .Where(tile => tile.Item2 >= 0).ToList()
                .OrderBy(e => e.Item2)
                .ThenBy(e => e.Item1)
                .First()
                .Item1;
        }

        private static Entity GetNearestTile(IEnumerable<Tuple<Entity, int>> nearest)
        {
            var closestTile = nearest.Aggregate((curMin, x) => x.Item2 < curMin.Item2 ? x : curMin).Item1;
            return closestTile;
        }

        private List<Tuple<Entity, int>> GetReachableTilesAdjacentToEnemies(Entity unit, IEnumerable<Unit> enemies, bool[,] map)
        {
            return 
                enemies
                .SelectMany(enemy => Adjacent(enemy, map))
                .Select(adjTile => Tuple.Create(adjTile, CalcDistance(unit, adjTile)))
                .Where(tile => tile.Item2 > 0)
                .OrderBy(e => e.Item2)
                .ThenBy(e => e.Item1)
                .ToList();
        }

        private int CalcDistance(Entity start, Entity goal)
        {
            var visited = new HashSet<Entity>();
            var queue = new Queue<Entity>();
            var distances = new Dictionary<Entity, int>();
            queue.Enqueue(start);
            distances.Add(start, 0);
            var map = IndividualMap;
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                visited.Add(current);
                foreach (var n in AdjacentIncludeGoal(current, goal, map).Where(a => !visited.Contains(a) && !queue.Contains(a)))
                {
                    queue.Enqueue(n);
                    distances.Add(n, distances[current] + 1);
                }
            }
            if (!distances.TryGetValue(goal, out var result)) return -1;
            return result;
        }

        private IEnumerable<Entity> Adjacent(Entity entity, bool[,] map)
        {
            return AdjacentIncludeGoal(entity, new Entity(-1, -1), map);
        }

        private IEnumerable<Entity> AdjacentIncludeGoal(Entity entity, Entity goal, bool[,] map)
        {
            var rv = new List<Entity>
            {
                new Entity(entity.X + 1, entity.Y),
                new Entity(entity.X - 1, entity.Y),
                new Entity(entity.X, entity.Y + 1),
                new Entity(entity.X, entity.Y - 1)
            };
            return rv.Where(e =>
                e.X >= 0 &&
                e.Y >= 0 &&
                e.X < _map.GetLength((1)) &&
                e.Y < _map.GetLength(0) &&
                (map[e.X, e.Y] || (e.X == goal.X && e.Y == goal.Y))
            );
        }

        private List<Unit> Units => _goblins.Concat(_elves).Where(u => u.Alive).ToList();

        private bool[,] IndividualMap
        { get
            {
                var temp = _map.Clone() as bool[,];
                Units.ForEach(unit => temp[unit.X, unit.Y] = false);
                return temp;
            }
        }
 
    }
   
}
