using Orleans;
using Orleans.TestingHost;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Runtime;

namespace Ains.Silo
{
    class Program : TestingSiloHost
    {
        public Program()
        {

        }

        static async Task Run()
        {
            var arena = GrainClient.GrainFactory.GetGrain<IArena>(Guid.NewGuid().GetHashCode().ToString());

            const int count = 2;
            var heroes = new List<IPlayer>();

            for (int i = 0; i < count; i++)
            {
                var hero = GrainClient.GrainFactory.GetGrain<IHero>(Guid.NewGuid());
                await hero.SetName($"Hero-{i}");
                heroes.Add(hero);
            }

            await arena.Config(heroes[0], 5, count);
            for (int i = 1; i < heroes.Count; i++)
            {
                await arena.Join(heroes[i]);
            }

            Debug.Assert(await arena.GetPhase() == ArenaPhase.Ready);

            while (await arena.GetPhase() != ArenaPhase.Finished)
            {
                var info = await arena.GetInfo();
                Console.WriteLine($"Round {info.Round} Turn {info.Turn}");
                var hero = info.Player.Cast<IHero>();
                Console.WriteLine($"{await hero.GetName()}'s turn...");
                await hero.Step(arena);
                await arena.Commit(hero);
                await Task.Delay(3000);
            }
        }

        static void Main(string[] args)
        {
            var tt = Type.GetType("Orleans.Storage.AzureTableStorage");

            var silo = new Program();

            Console.WriteLine();
            Console.WriteLine();

            Run().Wait();
            Console.WriteLine("<<Press enter to terminate>>");
            Console.ReadLine();
        }
    }
}
