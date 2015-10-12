using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains.Logic
{
    public class Player : Grain, IPlayer
    {
        public Task<string> GetName()
        {
            throw new NotImplementedException();
        }

        public Task SetName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<PlayerStat> GetStat()
        {
            throw new NotImplementedException();
        }

        public Task SetStat(PlayerStat stat)
        {
            throw new NotImplementedException();
        }

        public Task<Traits> GetTraits()
        {
            throw new NotImplementedException();
        }

        public Task SetTraits(Traits traits)
        {
            throw new NotImplementedException();
        }
    }
}
