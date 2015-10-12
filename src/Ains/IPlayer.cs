using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains
{
    public interface IPlayer : IGrainWithGuidKey
    {
        Task<string> GetName();
        Task SetName(string name);

        Task<PlayerStat> GetStat();
        Task SetStat(PlayerStat stat);

        Task<Traits> GetTraits();
        Task SetTraits(Traits traits);
    }
}
