using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains.Logic
{
    public partial class Hero : Grain, IHero
    {
        private string name;
        private PlayerStat stat;
        private Traits traits;
        private Dictionary<HeroAction, int> actionScores;

        public Task<string> GetName()
        {
            return Task.FromResult(name);
        }

        public Task SetName(string name)
        {
            this.name = name;
            return TaskDone.Done;
        }

        public Task Ready()
        {
            return TaskDone.Done;
        }

        public Task<PlayerStat> GetStat()
        {
            return Task.FromResult(stat);
        }

        public Task SetStat(PlayerStat stat)
        {
            this.stat = stat;
            return TaskDone.Done;
        }

        public Task<Traits> GetTraits() => Task.FromResult(traits);

        public Task SetTraits(Traits traits)
        {
            this.traits = traits;
            return TaskDone.Done;
        }

        public Task<Dictionary<HeroAction, int>> GetActionScores() => Task.FromResult(actionScores);

        public Task SetActionScores(Dictionary<HeroAction, int> actionScores)
        {
            this.actionScores = actionScores;
            return TaskDone.Done;
        }

        public async Task Step(IArena arena)
        {
            var pos = await arena.GetPlayerPosition(this);
            Console.WriteLine($"Position = Q:{pos.Q} R:{pos.R}");

            var path = FindPath(await arena.GetTerrain(), pos, new Hex());

            await arena.Move(this, new Hex(-1, -1));
        }

        public static List<Hex> FindPath(IDictionary<Hex, Tile> data, Hex start, Hex end)
        {
            var frontier = new Queue<Hex>();
            frontier.Enqueue(start);
            var trace = new Dictionary<Hex, Hex>();
            trace[start] = start;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current == end)
                {
                    break;
                }

                foreach (var next in Near(data, current).Where(n => !trace.ContainsKey(n)))
                {
                    frontier.Enqueue(next);
                    trace[next] = current;
                }
            }

            var path = new List<Hex> { end };
            while (end != start)
            {
                end = trace[end];
                path.Add(end);
            }

            path.Reverse();
            return path;
        }

        public static IEnumerable<Hex> Near(IDictionary<Hex, Tile> data, Hex start)
        {
            Hex[] directions = { new Hex(+1, -1), new Hex(+1, 0), new Hex(0, +1), new Hex(-1, +1), new Hex(-1, 0), new Hex(0, -1) };

            foreach (var direction in directions)
            {
                var pos = start + direction;

                if (data.ContainsKey(pos))
                {
                    yield return pos;
                }
            }
        }
    }
}
