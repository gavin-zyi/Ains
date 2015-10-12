using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains.Logic
{
    public partial class Arena
    {
        [Serializable]
        private class ResetAction : ArenaAction
        {
            private Hex[] positions;

            public ResetAction(IPlayer player, int round, int turn) : base(player, round, turn)
            {

            }

            protected override Task OnPrepare(Arena arena)
            {
                var set = new HashSet<Hex>();
                var size = arena.State.Terrain.Size;

                var playerCount = arena.State.Players.Count;
                while (set.Count < playerCount)
                {
                    var q = new Random(Guid.NewGuid().GetHashCode()).Next(-size, size);
                    var r = new Random(Guid.NewGuid().GetHashCode()).Next(-size, size);
                    set.Add(new Hex(q, r));
                }

                positions = set.ToArray();
                return TaskDone.Done;
            }

            protected override Task OnExecute(Arena arena)
            {
                Console.WriteLine("ResetAction");
                var playerCount = arena.State.Players.Count;

                if (playerCount != positions.Length)
                {
                    throw new Exception("Arena state changed!");
                }

                for (var i = 0; i < playerCount; i++)
                {
                    arena.State.PlayerStates[arena.State.Players[i]].Position = positions[i];
                }
                return TaskDone.Done;
            }
        }

        [Serializable]
        private class MoveAction : ArenaAction
        {
            public Hex Offset { get; private set; }

            public MoveAction(IPlayer player, int round, int turn, Hex offset) : base(player, round, turn)
            {
                Offset = offset;
            }

            protected override Task OnExecute(Arena arena)
            {
                arena.State.PlayerStates[Player].Position += Offset;
                return TaskDone.Done;
            }
        }
    }
}
