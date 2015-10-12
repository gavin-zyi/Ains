using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains
{
    [Serializable]
    public class Traits
    {
        public Ability Move { get; set; }
        public Ability Attack { get; set; }
    }

    public class Ability
    {
        public int Value { get; set; }
        public Range Range { get; set; }
    }

    public class Range
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public Func<Hex, bool> Rule { get; set; }

        public bool Validate(Hex pos)
        {
            if (pos.Length < Min || pos.Length > Max)
            {
                return false;
            }

            return Rule(pos);
        }
    }
}
