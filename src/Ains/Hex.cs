using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains
{
    [Serializable, Immutable]
    public struct Hex
    {
        public readonly int Q;
        public readonly int R;
        public readonly int S;

        public Hex(int q, int r)
        {
            Q = q;
            R = r;
            S = -q - r;
        }

        public Hex(int q, int r, int s)
        {
            if (q + r + s != 0)
            {
                throw new ArgumentException("Hex(q + r + s != 0)");
            }

            Q = q;
            R = r;
            S = s;
        }

        public int Length => (Math.Abs(Q) + Math.Abs(R) + Math.Abs(S)) / 2;

        public bool Equals(Hex other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return this == other;
        }

        public override bool Equals(object a)
        {
            if (ReferenceEquals(null, a))
                return false;
            if (ReferenceEquals(this, a))
                return true;
            if (a.GetType() != GetType())
                return false;

            return Equals((Hex)a);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ Q;
                result = (result * 397) ^ R;
                result = (result * 397) ^ S;
                return result;
            }
        }

        public override string ToString() => $"{{Q:{Q} R:{R} S:{S}}}";

        public static int Distance(Hex a, Hex b) => (a - b).Length;

        public static bool operator ==(Hex a, Hex b) => a.Q == b.Q && a.R == b.R && a.S == b.S;

        public static bool operator !=(Hex a, Hex b) => !(a == b);

        public static Hex operator +(Hex a, Hex b) => new Hex(a.Q + b.Q, a.R + b.R, a.S + b.S);

        public static Hex operator -(Hex a, Hex b) => new Hex(a.Q - b.Q, a.R - b.R, a.S - b.S);

        public static Hex operator *(Hex a, int k) => new Hex(a.Q * k, a.R * k, a.S * k);
    }
}
