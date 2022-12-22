using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Attributes
{
    public class Column : Attribute
    {
        public string Name;
        public bool PrimaryKey;

        public Column(bool primaryKey = false)
        {
            Name = GetType().Name;
            PrimaryKey = primaryKey;
        }

        public Column(string name, bool primaryKey = false)
        {
            Name = name;
            PrimaryKey = primaryKey;
        }
    }
}
