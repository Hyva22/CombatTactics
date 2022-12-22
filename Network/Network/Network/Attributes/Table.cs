using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Attributes
{
    internal class Table : Attribute
    {
        public string Name;

        public Table()
        {
            Name = GetType().Name;
        }

        public Table(string name)
        {
            Name = name;
        }
    }
}
