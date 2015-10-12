using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains
{
    public interface IHero : IPlayer
    {
        Task<Dictionary<HeroAction, int>> GetActionScores();
        Task SetActionScores(Dictionary<HeroAction, int> actionScores);

        Task Step(IArena arena);
    }

    public enum HeroAction
    {
        Chase,
        Evade,
        Idle
    }
}
