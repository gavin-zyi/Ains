using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains
{
    [Serializable]
    public class Grid<T>
    {
        private readonly Dictionary<Hex, T> data;

        public ReadOnlyDictionary<Hex, T> Raw => new ReadOnlyDictionary<Hex, T>(data);
        
        public Grid()
        {
            data = new Dictionary<Hex, T>();
        }

        protected void Clear()
        {
            data.Clear();
        }

        public T this[Hex hex]
        {
            get { return data[hex]; }
            set { data[hex] = value; }
        }

        public T this[int q, int r]
        {
            get { return data[new Hex(q, r)]; }
            set { data[new Hex(q, r)] = value; }
        }
    }
}
