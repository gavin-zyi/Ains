using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains
{
    [Serializable]
    public class Terrain : Grid<Tile>
    {
        public int Size { get; private set; }

        public Terrain(int size)
        {
            Size = size;
        }

        public void Populate(Tile tile)
        {
            Clear();
            for (int q = -Size; q <= Size; q++)
            {
                int r1 = Math.Max(-Size, -q - Size);
                int r2 = Math.Min(Size, -q + Size);
                for (int r = r1; r <= r2; r++)
                {
                    this[q, r] = tile;
                }
            }
        }

    }
}
